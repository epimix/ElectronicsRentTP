using BusinessLogic;
using BusinessLogic.Interfaces;
using BusinessLogic.Services;
using DataAccess.Data;
using DataAccess.Data.Entities;
using DataAccess.Repositories;
using ElectronicsRentTP.Interfaces;
using ElectronicsRentTP.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new Exception("No connection string found.");

builder.Services.AddDbContext<EquipmentRentalDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddIdentity<User, IdentityRole>()
    .AddEntityFrameworkStores<EquipmentRentalDbContext>()
    .AddDefaultTokenProviders();

builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
builder.Services.AddHttpContextAccessor();
builder.Services.AddScoped<IEquipmentService, EquipmentService>();
builder.Services.AddScoped<IFavService, FavoriteService>();
builder.Services.AddScoped<IJwtService, JwtService>();
builder.Services.AddScoped<IUserServices, UserServices>();

builder.Services.AddSingleton(_ => builder.Configuration.GetSection(nameof(JwtOptions)).Get<JwtOptions>()!);


builder.Services.AddControllersWithViews();

builder.Services.AddDistributedMemoryCache();

builder.Services.AddSession(options =>
{
    options.IdleTimeout = TimeSpan.FromMinutes(10);
    options.Cookie.HttpOnly = true;
    options.Cookie.IsEssential = true;
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.UseAuthentication(); 
app.UseAuthorization();

app.UseSession();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
