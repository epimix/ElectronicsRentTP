using AutoMapper;
using BusinessLogic;
using BusinessLogic.Configure;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DataAccess.Data;
using DataAccess.Data.Entities;
using DataAccess.Repositories;
using ElectronicsRentTP.Extensions;
using ElectronicsRentTP.Helpers;
using ElectronicsRentTP.Hubs;
using ElectronicsRentTP.Interfaces;
using ElectronicsRentTP.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;

var builder = WebApplication.CreateBuilder(args);

// -------------------- DATABASE --------------------
var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new Exception("No connection string found.");

builder.Services.AddDbContext<EquipmentRentalDbContext>(options =>
    options.UseSqlServer(connectionString));

// -------------------- IDENTITY --------------------
builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<EquipmentRentalDbContext>()
    .AddDefaultTokenProviders();


// -------------------- REPOSITORIES & SERVICES --------------------
builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<IFavService, FavoriteService>();
builder.Services.AddScoped<IUserServices, UserServices>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<IReviewService, ReviewService>();
builder.Services.AddScoped<IFavoriteEquipmentDBService, FavoriteEquipmentDBService>();
builder.Services.AddScoped<IRentalService, RentalService>();
builder.Services.AddScoped<IRentalRepository, RentalRepository>();
builder.Services.AddScoped<IAdvertsService, AdvertsService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IBalanceService, BalanceService>();

// -------------------- JWT OPTIONS --------------------

builder.Services.Configure<JwtOptions>(
    builder.Configuration.GetSection(nameof(JwtOptions)));
builder.Services.AddScoped<IJwtService, JwtService>();

// -------------------- AUTOMAPPER --------------------
// Для AutoMapper 13.0.1
/* (✌ﾟ∀ﾟ)☞ */
builder.Services.AddAutoMapper(typeof(MapperProfile)); /*凸(⊙▂⊙✖ )*/ /*i love git hub copilot*/ /*he ended this phrase, it wasn't me (´⊙ω⊙`)*/

// -------------------- CONTROLLERS --------------------
builder.Services.AddControllersWithViews();

// -------------------- SESSION --------------------
builder.Services.AddDistributedMemoryCache();
builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

builder.Services.AddSignalR();

// -------------------- BUILD APP --------------------
var app = builder.Build();

// -------------------- MIDDLEWARE --------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.SeedRolesAndAdmin();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();


app.UseAuthentication();


app.UseMiddleware<ElectronicsRentTP.Middleware.AuthTokenMiddleware>();

app.UseAuthorization();
app.UseSession();

// -------------------- ENDPOINTS --------------------
app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapHub<ChatHub>("/chathub");

app.Run();
