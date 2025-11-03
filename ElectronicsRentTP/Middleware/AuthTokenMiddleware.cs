using DataAccess.Data;
using DataAccess.Data.Entities;
using DataAccess.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
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
                path.StartsWithSegments("/css") ||
                path.StartsWithSegments("/js") ||
                path.StartsWithSegments("/images"))
            {
                await _next(context);
                return;
            }


            if (context.Request.Cookies.TryGetValue("sessionToken", out var token) && !string.IsNullOrEmpty(token))
            {
                var handler = new JwtSecurityTokenHandler();
                var key = Encoding.UTF8.GetBytes(_config["JwtOptions:Key"]);
                var parameters = new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = true,
                    ValidIssuer = _config["JwtOptions:Issuer"],
                    ValidateAudience = false,
                    ValidateLifetime = true,
                    ClockSkew = TimeSpan.Zero
                };

                ClaimsPrincipal principal = null;
                string userId = null;

                try
                {
                    var jwt = handler.ReadJwtToken(token);
                    Console.WriteLine(jwt.ValidTo);

                    principal = handler.ValidateToken(token, parameters, out _);
                    userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    if (string.IsNullOrEmpty(userId))
                    {
                        await RedirectToRegister(context);
                        return;
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

                    var user = await db.Users.Include(u => u.RefreshTokens)
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


                    var newJwt = GenerateNewJwtToken(user, _config);


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


                    context.Response.Cookies.Append("sessionToken", newJwt, new CookieOptions
                    {
                        HttpOnly = true,
                        Secure = true,
                        SameSite = SameSiteMode.Strict,
                        Expires = DateTime.UtcNow.AddMinutes(1) 
                    });

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

        private string GenerateNewJwtToken(User user, IConfiguration config)
        {
            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(config["JwtOptions:Key"]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName)
            };

            var token = new JwtSecurityToken(
                issuer: config["JwtOptions:Issuer"],
                audience: config["JwtOptions:Audience"],
                claims: claims,
                expires: DateTime.UtcNow.AddMinutes(1),
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private async Task RedirectToRegister(HttpContext context)
        {
            if (context.Request.Cookies.ContainsKey("sessionToken"))
                context.Response.Cookies.Delete("sessionToken");

            if (context.Features.Get<ISessionFeature>() != null)
                context.Session.Clear();

            if (!context.Response.HasStarted)
            {
                context.Response.Clear();
                context.Response.Redirect("/Account/Register", false);
            }

            await Task.CompletedTask;
        }
    }
}
