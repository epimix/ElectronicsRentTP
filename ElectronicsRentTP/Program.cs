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
// Azure App Service uses different formats for connection strings
// Try multiple ways to get the connection string
var connectionString = 
    // Azure Connection Strings section (format: SQLCONNSTR_DefaultConnection or SQLAZURECONNSTR_DefaultConnection)
    Environment.GetEnvironmentVariable("SQLCONNSTR_DefaultConnection")
    ?? Environment.GetEnvironmentVariable("SQLAZURECONNSTR_DefaultConnection")
    // Standard environment variable format
    ?? Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection")
    // Configuration (appsettings.json or Azure App Settings)
    ?? builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new Exception("No connection string found. Please configure DefaultConnection in Azure App Service Connection Strings or Application Settings.");

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
builder.Services.AddScoped<IRentalService, RentalService>();
builder.Services.AddScoped<IRentalRepository, RentalRepository>();
builder.Services.AddScoped<IAdvertsService, AdvertsService>();
builder.Services.AddScoped<ICartService, CartService>();
builder.Services.AddScoped<IBalanceService, BalanceService>();
builder.Services.AddScoped<IAdminService, AdminService>();
builder.Services.AddScoped<IChatService, ChatService>();
builder.Services.AddScoped<IComplaintService, ComplaintService>();
builder.Services.AddScoped<IAccountService, AccountService>();

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

// -------------------- LOG CONNECTION STRING (for debugging) --------------------
using (var scope = app.Services.CreateScope())
{
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    // Log which source was used
    if (Environment.GetEnvironmentVariable("SQLCONNSTR_DefaultConnection") != null)
        logger.LogInformation("Connection string source: Azure Connection Strings (SQLCONNSTR)");
    else if (Environment.GetEnvironmentVariable("SQLAZURECONNSTR_DefaultConnection") != null)
        logger.LogInformation("Connection string source: Azure Connection Strings (SQLAZURECONNSTR)");
    else if (Environment.GetEnvironmentVariable("ConnectionStrings__DefaultConnection") != null)
        logger.LogInformation("Connection string source: Environment variable (ConnectionStrings__DefaultConnection)");
    else
        logger.LogInformation("Connection string source: Configuration (appsettings.json)");
    
    // Log connection string (masked)
    var connectionStringForLog = connectionString;
    if (connectionString.Contains("Password="))
    {
        var pwdIndex = connectionString.IndexOf("Password=");
        var pwdEnd = connectionString.IndexOf(";", pwdIndex);
        if (pwdEnd == -1) pwdEnd = connectionString.Length;
        connectionStringForLog = connectionString.Substring(0, pwdIndex) + "Password=***" + connectionString.Substring(pwdEnd);
    }
    logger.LogInformation("Using connection string: {ConnectionString}", connectionStringForLog);
}

// -------------------- MIDDLEWARE --------------------
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

// -------------------- FIX MISSING LastOnline COLUMN (MUST BE BEFORE SEEDING) --------------------
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EquipmentRentalDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Перевіряємо, чи можемо підключитися до бази (обгортаємо в try-catch, щоб не блокувати запуск)
        bool canConnect = false;
        try
        {
            canConnect = dbContext.Database.CanConnect();
        }
        catch (Exception connectEx)
        {
            logger.LogWarning(connectEx, "Cannot connect to database during initialization. Connection string might be incorrect. Error: {Error}", connectEx.Message);
            logger.LogWarning("Skipping database initialization. Please check your connection string in Azure App Service Configuration.");
        }

        if (canConnect)
        {
            // Додаємо колонку LastOnline до таблиці AspNetUsers, якщо її немає
            // Це має бути виконано ПЕРЕД SeedRolesAndAdmin, щоб уникнути помилок
            dbContext.Database.ExecuteSqlRaw(@"
                IF NOT EXISTS (SELECT * FROM sys.columns 
                               WHERE object_id = OBJECT_ID(N'[dbo].[AspNetUsers]') 
                               AND name = 'LastOnline')
                BEGIN
                    ALTER TABLE [AspNetUsers] 
                    ADD [LastOnline] datetime2 NULL;
                END
            ");
            logger.LogInformation("LastOnline column checked and added if needed.");
        }
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Не вдалося додати колонку LastOnline: {Message}", ex.Message);
    }
}

// Seed roles and admin - не блокує запуск, якщо база недоступна
app.SeedRolesAndAdmin();

// -------------------- FIX MISSING OwnerId COLUMNS --------------------
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<EquipmentRentalDbContext>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();

    try
    {
        // Додаємо колонку OwnerId до таблиці Equipments, якщо її немає
        dbContext.Database.ExecuteSqlRaw(@"
            IF NOT EXISTS (SELECT * FROM sys.columns 
                           WHERE object_id = OBJECT_ID(N'[dbo].[Equipments]') 
                           AND name = 'OwnerId')
            BEGIN
                ALTER TABLE [Equipments] 
                ADD [OwnerId] nvarchar(450) NULL;
            END
        ");

        // Додаємо foreign key для Equipments.OwnerId, якщо його немає
        dbContext.Database.ExecuteSqlRaw(@"
            IF NOT EXISTS (SELECT * FROM sys.foreign_keys 
                           WHERE name = 'FK_Equipments_AspNetUsers_OwnerId')
            BEGIN
                ALTER TABLE [Equipments] 
                ADD CONSTRAINT [FK_Equipments_AspNetUsers_OwnerId] 
                FOREIGN KEY ([OwnerId]) 
                REFERENCES [AspNetUsers] ([Id]) 
                ON DELETE NO ACTION;
            END
        ");

        // Додаємо колонку OwnerId до таблиці Rentals, якщо її немає
        dbContext.Database.ExecuteSqlRaw(@"
            IF NOT EXISTS (SELECT * FROM sys.columns 
                           WHERE object_id = OBJECT_ID(N'[dbo].[Rentals]') 
                           AND name = 'OwnerId')
            BEGIN
                ALTER TABLE [Rentals] 
                ADD [OwnerId] nvarchar(450) NULL;
            END
        ");

        // Додаємо foreign key для Rentals.OwnerId, якщо його немає
        dbContext.Database.ExecuteSqlRaw(@"
            IF NOT EXISTS (SELECT * FROM sys.foreign_keys 
                           WHERE name = 'FK_Rentals_AspNetUsers_OwnerId')
            BEGIN
                ALTER TABLE [Rentals] 
                ADD CONSTRAINT [FK_Rentals_AspNetUsers_OwnerId] 
                FOREIGN KEY ([OwnerId]) 
                REFERENCES [AspNetUsers] ([Id]) 
                ON DELETE NO ACTION;
            END
        ");

        // Створюємо таблицю CartEntities, якщо її немає
        dbContext.Database.ExecuteSqlRaw(@"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[CartEntities]') AND type in (N'U'))
            BEGIN
                CREATE TABLE [CartEntities] (
                    [Id] int NOT NULL IDENTITY,
                    [UserId] nvarchar(450) NOT NULL,
                    [EquipmentId] int NOT NULL,
                    [Quantity] int NOT NULL,
                    [TotalPrice] decimal(18,2) NOT NULL,
                    CONSTRAINT [PK_CartEntities] PRIMARY KEY ([Id]),
                    CONSTRAINT [FK_CartEntities_AspNetUsers_UserId] FOREIGN KEY ([UserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE CASCADE,
                    CONSTRAINT [FK_CartEntities_Equipments_EquipmentId] FOREIGN KEY ([EquipmentId]) REFERENCES [Equipments] ([Id]) ON DELETE NO ACTION
                );
                CREATE INDEX [IX_CartEntities_UserId] ON [CartEntities] ([UserId]);
                CREATE INDEX [IX_CartEntities_EquipmentId] ON [CartEntities] ([EquipmentId]);
            END
        ");

        // Створюємо таблицю ChatRooms, якщо її немає
        dbContext.Database.ExecuteSqlRaw(@"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChatRooms]') AND type in (N'U'))
            BEGIN
                CREATE TABLE [ChatRooms] (
                    [Id] int NOT NULL IDENTITY,
                    [EquipmentId] int NOT NULL,
                    [OwnerId] nvarchar(450) NOT NULL,
                    [RenterId] nvarchar(450) NOT NULL,
                    [IsPinned] bit NOT NULL DEFAULT 0,
                    [IsDeleted] bit NOT NULL DEFAULT 0,
                    CONSTRAINT [PK_ChatRooms] PRIMARY KEY ([Id]),
                    CONSTRAINT [FK_ChatRooms_Equipments_EquipmentId] FOREIGN KEY ([EquipmentId]) REFERENCES [Equipments] ([Id]) ON DELETE CASCADE,
                    CONSTRAINT [FK_ChatRooms_AspNetUsers_OwnerId] FOREIGN KEY ([OwnerId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
                    CONSTRAINT [FK_ChatRooms_AspNetUsers_RenterId] FOREIGN KEY ([RenterId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
                );
                CREATE INDEX [IX_ChatRooms_EquipmentId] ON [ChatRooms] ([EquipmentId]);
                CREATE INDEX [IX_ChatRooms_OwnerId] ON [ChatRooms] ([OwnerId]);
                CREATE INDEX [IX_ChatRooms_RenterId] ON [ChatRooms] ([RenterId]);
            END
        ");

        // Додаємо колонку IsPinned до таблиці ChatRooms, якщо її немає
        dbContext.Database.ExecuteSqlRaw(@"
            IF NOT EXISTS (SELECT * FROM sys.columns 
                           WHERE object_id = OBJECT_ID(N'[dbo].[ChatRooms]') 
                           AND name = 'IsPinned')
            BEGIN
                ALTER TABLE [ChatRooms] 
                ADD [IsPinned] bit NOT NULL DEFAULT 0;
            END
        ");

        // Додаємо колонку IsDeleted до таблиці ChatRooms, якщо її немає
        dbContext.Database.ExecuteSqlRaw(@"
            IF NOT EXISTS (SELECT * FROM sys.columns 
                           WHERE object_id = OBJECT_ID(N'[dbo].[ChatRooms]') 
                           AND name = 'IsDeleted')
            BEGIN
                ALTER TABLE [ChatRooms] 
                ADD [IsDeleted] bit NOT NULL DEFAULT 0;
            END
        ");

        // Створюємо таблицю ChatMessages, якщо її немає
        dbContext.Database.ExecuteSqlRaw(@"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[ChatMessages]') AND type in (N'U'))
            BEGIN
                CREATE TABLE [ChatMessages] (
                    [Id] int NOT NULL IDENTITY,
                    [ChatRoomId] int NOT NULL,
                    [SenderId] nvarchar(450) NOT NULL,
                    [Text] nvarchar(max) NOT NULL,
                    [SentAt] datetime2 NOT NULL,
                    [IsRead] bit NOT NULL,
                    CONSTRAINT [PK_ChatMessages] PRIMARY KEY ([Id]),
                    CONSTRAINT [FK_ChatMessages_ChatRooms_ChatRoomId] FOREIGN KEY ([ChatRoomId]) REFERENCES [ChatRooms] ([Id]) ON DELETE CASCADE,
                    CONSTRAINT [FK_ChatMessages_AspNetUsers_SenderId] FOREIGN KEY ([SenderId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION
                );
                CREATE INDEX [IX_ChatMessages_ChatRoomId] ON [ChatMessages] ([ChatRoomId]);
                CREATE INDEX [IX_ChatMessages_SenderId] ON [ChatMessages] ([SenderId]);
            END
        ");

        // Створюємо таблицю Complaints, якщо її немає
        dbContext.Database.ExecuteSqlRaw(@"
            IF NOT EXISTS (SELECT * FROM sys.objects WHERE object_id = OBJECT_ID(N'[dbo].[Complaints]') AND type in (N'U'))
            BEGIN
                CREATE TABLE [Complaints] (
                    [Id] int NOT NULL IDENTITY,
                    [ReporterId] nvarchar(450) NOT NULL,
                    [TargetUserId] nvarchar(450) NOT NULL,
                    [RentalId] int NOT NULL,
                    [Message] nvarchar(max) NOT NULL,
                    [AdminComment] nvarchar(max) NULL,
                    [Created] datetime2 NOT NULL,
                    [Resolved] bit NOT NULL,
                    CONSTRAINT [PK_Complaints] PRIMARY KEY ([Id]),
                    CONSTRAINT [FK_Complaints_AspNetUsers_ReporterId] FOREIGN KEY ([ReporterId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
                    CONSTRAINT [FK_Complaints_AspNetUsers_TargetUserId] FOREIGN KEY ([TargetUserId]) REFERENCES [AspNetUsers] ([Id]) ON DELETE NO ACTION,
                    CONSTRAINT [FK_Complaints_Rentals_RentalId] FOREIGN KEY ([RentalId]) REFERENCES [Rentals] ([Id]) ON DELETE NO ACTION
                );
                CREATE INDEX [IX_Complaints_RentalId] ON [Complaints] ([RentalId]);
                CREATE INDEX [IX_Complaints_ReporterId] ON [Complaints] ([ReporterId]);
                CREATE INDEX [IX_Complaints_TargetUserId] ON [Complaints] ([TargetUserId]);
            END
        ");

        // Додаємо колонку AdminComment до таблиці Complaints, якщо її немає
        dbContext.Database.ExecuteSqlRaw(@"
            IF NOT EXISTS (SELECT * FROM sys.columns 
                           WHERE object_id = OBJECT_ID(N'[dbo].[Complaints]') 
                           AND name = 'AdminComment')
            BEGIN
                ALTER TABLE [Complaints] 
                ADD [AdminComment] nvarchar(max) NULL;
            END
        ");

        logger.LogInformation("Database columns OwnerId and CartEntities table checked and added if needed.");
    }
    catch (Exception ex)
    {
        logger.LogWarning(ex, "Не вдалося додати колонки OwnerId або таблицю CartEntities: {Message}", ex.Message);
    }
}

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
