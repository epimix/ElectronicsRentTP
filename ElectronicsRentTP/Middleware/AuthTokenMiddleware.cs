using DataAccess.Data;
using DataAccess.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace ElectronicsRentTP.Middleware
{
    public class AuthTokenMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly IConfiguration _config;

        public AuthTokenMiddleware(RequestDelegate next, IConfiguration config)
        {
            _next = next;
            _config = config;
        }

        public async Task InvokeAsync(HttpContext context, EquipmentRentalDbContext db)
        {
            var path = context.Request.Path;

            if (path == "/" ||
                path.StartsWithSegments("/Account/Login") ||
                path.StartsWithSegments("/Account/Register") ||
                path.StartsWithSegments("/Home") ||
                path.StartsWithSegments("/Equipment") ||
                path.StartsWithSegments("/css") ||
                path.StartsWithSegments("/js") ||
                path.StartsWithSegments("/images") ||
                path.StartsWithSegments("/uploads") ||
                path.StartsWithSegments("/lib"))
            {
                if (context.Request.Cookies.TryGetValue("sessionToken", out var t) && !string.IsNullOrEmpty(t))
                {
                    var jwtKey = _config["JwtOptions:Key"];
                    var jwtIssuer = _config["JwtOptions:Issuer"];

                    var handler = new JwtSecurityTokenHandler();
                    var key = Encoding.UTF8.GetBytes(jwtKey ?? string.Empty);
                    var parameters = new TokenValidationParameters
                    {
                        ValidateIssuerSigningKey = true,
                        IssuerSigningKey = new SymmetricSecurityKey(key),
                        ValidateIssuer = true,
                        ValidIssuer = jwtIssuer,
                        ValidateAudience = false,
                        ValidateLifetime = true,
                        ClockSkew = TimeSpan.Zero
                    };
                    ClaimsPrincipal? principal = null;

                    try
                    {
                        principal = handler.ValidateToken(t, parameters, out _);
                        context.User = principal;
                    }
                    catch (SecurityTokenExpiredException)
                    {
                        context.Response.Cookies.Delete("sessionToken");
                    }
                    catch
                    {
                        // Інші помилки валідації - ігноруємо для публічних шляхів
                    }
                }

                await _next(context);
                return;
            }

            // Перевіряємо стандартну автентифікацію ASP.NET Identity
            if (context.User?.Identity?.IsAuthenticated == true)
            {
                await _next(context);
                return;
            }

            // Перевіряємо JWT токен з cookie
            if (context.Request.Cookies.TryGetValue("sessionToken", out var token) && !string.IsNullOrEmpty(token))
            {
                var jwtKey = _config["JwtOptions:Key"];
                var jwtIssuer = _config["JwtOptions:Issuer"];

                if (string.IsNullOrEmpty(jwtKey) || string.IsNullOrEmpty(jwtIssuer))
                {
                    await RedirectToRegister(context);
                    return;
                }

                var handler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(jwtKey);
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = jwtIssuer,
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                ClaimsPrincipal? principal = null;
                string? userId = null;

                try
                {
                    var jwt = handler.ReadJwtToken(token);
                    principal = handler.ValidateToken(token, parameters, out _);
                    userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    if (string.IsNullOrEmpty(userId))
                    {
                        await RedirectToRegister(context);
                        return;
                    }

                    if (principal.Identity is ClaimsIdentity claimsIdentity && !claimsIdentity.IsAuthenticated)
                    {
                        principal = new ClaimsPrincipal(new ClaimsIdentity(claimsIdentity.Claims, "JWT", ClaimTypes.Name, ClaimTypes.Role));
                    }

                    context.User = principal;
                    await _next(context);
                    return;
                }
                catch (SecurityTokenExpiredException)
                {
                    var expiredJwt = handler.ReadJwtToken(token);
                    userId = expiredJwt.Claims.FirstOrDefault(c => c.Type == ClaimTypes.NameIdentifier)?.Value;

                    if (string.IsNullOrEmpty(userId))
                    {
                        await RedirectToRegister(context);
                        return;
                    }

                    var user = await db.Users
                        .Include(u => u.RefreshTokens)
                        .FirstOrDefaultAsync(u => u.Id == userId);

                    if (user == null)
                    {
                        await RedirectToRegister(context);
                        return;
                    }

                    var refreshToken = user.RefreshTokens
                        .OrderByDescending(r => r.Created)
                        .FirstOrDefault();

                    if (refreshToken == null || refreshToken.Expires < DateTime.UtcNow)
                    {
                        await RedirectToRegister(context);
                        return;
                    }

                    var userManager = context.RequestServices.GetRequiredService<UserManager<User>>();
                    var newJwt = await GenerateNewJwtToken(user, context, userManager);

                    var ipAddress = context.Connection.RemoteIpAddress?.ToString() ?? "unknown";
                    var newRefresh = new RefreshToken
                    {
                        Token = Guid.NewGuid().ToString(),
                        Expires = DateTime.UtcNow.AddDays(7),
                        Created = DateTime.UtcNow,
                        CreatedByIp = ipAddress,
                        UserId = user.Id
                    };

                    user.RefreshTokens.Add(newRefresh);
                    await db.SaveChangesAsync();

                    var isHttps = context.Request.IsHttps;
                    context.Response.Cookies.Append("sessionToken", newJwt, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = isHttps,
                        SameSite = SameSiteMode.Lax,
                        Expires = DateTime.UtcNow.AddMinutes(60)
                    });

                    var newPrincipal = handler.ValidateToken(newJwt, parameters, out _);
                    if (newPrincipal.Identity is ClaimsIdentity newClaimsIdentity && !newClaimsIdentity.IsAuthenticated)
                    {
                        newPrincipal = new ClaimsPrincipal(new ClaimsIdentity(newClaimsIdentity.Claims, "JWT", ClaimTypes.Name, ClaimTypes.Role));
                    }

                    context.User = newPrincipal;
                    await _next(context);
                    return;
                }
                catch
                {
                    await RedirectToRegister(context);
                    return;
                }
            }

            await RedirectToRegister(context);
        }

        private async Task<string> GenerateNewJwtToken(User
