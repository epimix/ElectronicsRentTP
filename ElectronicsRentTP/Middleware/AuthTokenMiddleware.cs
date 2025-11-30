using DataAccess.Data;
using DataAccess.Data.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection; // <- ДОДАЙ ЦЕ
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
                    string? userId = null;

                    try
                    {
                        principal = handler.ValidateToken(t, parameters, out _);
                        userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                        context.User = principal;
                    }
                    catch (SecurityTokenExpiredException)
                    {
                        context.Response.Cookies.Delete("sessionToken");
                        context.Response.Redirect("/Account/Login");
                        return;
                    }
                    catch
                    {
                        context.Response.Cookies.Delete("sessionToken");
                        context.Response.Redirect("/Account/Login");
                        return;
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
                    Console.WriteLine(jwt.ValidTo);

                    principal = handler.ValidateToken(token, parameters, out _);
                    userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value;

                    if (string.IsNullOrEmpty(userId))
                    {
                        await RedirectToRegister(context);
                        return;
                    }

                    // Встановлюємо principal як поточного користувача
                    // Переконуємося, що Identity має правильний AuthenticationType
                    if (principal.Identity is ClaimsIdentity claimsIdentity)
                    {
                        // Якщо Identity не автентифіковано, встановлюємо правильний тип
                        if (!claimsIdentity.IsAuthenticated)
                        {
                            var newIdentity = new ClaimsIdentity(claimsIdentity.Claims, "JWT", ClaimTypes.Name, ClaimTypes.Role);
                            principal = new ClaimsPrincipal(newIdentity);
                        }
                    }

                    // Сетимо юзера в HttpContext (включно з ролями, якщо вони є в токені)
                    context.User = principal;

                    await _next(context);
                    return;
                }
                catch (SecurityTokenExpiredException)
                {
                    // Токен протух — пробуємо по refresh токену
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

                    // Генеруємо новий access JWT з ролями
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
                        Secure = isHttps, // Secure тільки для HTTPS
                        SameSite = SameSiteMode.Lax, // Lax для кращої сумісності
                        Expires = DateTime.UtcNow.AddMinutes(60) // Збільшуємо час життя токену
                    });

                    // Створюємо новий principal з оновленим токеном
                    var newPrincipal = handler.ValidateToken(newJwt, parameters, out _);
                    if (newPrincipal.Identity is ClaimsIdentity newClaimsIdentity && !newClaimsIdentity.IsAuthenticated)
                    {
                        var authenticatedIdentity = new ClaimsIdentity(newClaimsIdentity.Claims, "JWT", ClaimTypes.Name, ClaimTypes.Role);
                        newPrincipal = new ClaimsPrincipal(authenticatedIdentity);
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

            // Нема токена взагалі
            await RedirectToRegister(context);
        }



        private async Task<string> GenerateNewJwtToken(User user, HttpContext context, UserManager<User> userManager)
        {
            var jwtKey = _config["JwtOptions:Key"] ?? throw new InvalidOperationException("JWT Key is not configured");
            var jwtIssuer = _config["JwtOptions:Issuer"] ?? throw new InvalidOperationException("JWT Issuer is not configured");
            var jwtAudience = _config["JwtOptions:Audience"] ?? throw new InvalidOperationException("JWT Audience is not configured");

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var roles = await userManager.GetRolesAsync(user);

            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(ClaimTypes.Name, user.UserName ?? string.Empty)
            };

            foreach (var role in roles)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
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
