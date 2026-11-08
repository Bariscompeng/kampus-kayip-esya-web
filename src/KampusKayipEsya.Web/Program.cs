using KampusKayipEsya.Web.Data;
using KampusKayipEsya.Web.Models;
using KampusKayipEsya.Web.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;

var builder = WebApplication.CreateBuilder(args);

var connectionString = builder.Configuration.GetConnectionString("DefaultConnection")
    ?? throw new InvalidOperationException("'ConnectionStrings:DefaultConnection' bulunamadı. README'deki user-secrets adımına bakın.");
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlServer(connectionString));
builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// Hazır Identity UI sayfaları yok; kayıt/giriş kendi AccountController'ımızda (okul e-postası kontrolü için).
builder.Services.AddIdentity<AppUser, IdentityRole>(options =>
    {
        options.SignIn.RequireConfirmedAccount = false;
        options.User.RequireUniqueEmail = true;
        options.Password.RequiredLength = 8;
        // AddDefaultIdentity bunu 128 yapıyordu; mevcut migration şeması değişmesin.
        options.Stores.MaxLengthForKeys = 128;
    })
    .AddEntityFrameworkStores<AppDbContext>()
    .AddDefaultTokenProviders()
    .AddErrorDescriber<TurkishIdentityErrorDescriber>();

builder.Services.ConfigureApplicationCookie(options =>
{
    options.LoginPath = "/Account/Login";
    options.LogoutPath = "/Account/Logout";
    options.AccessDeniedPath = "/Account/AccessDenied";
});

var jwtOptions = builder.Configuration.GetSection("Jwt").Get<JwtOptions>() ?? new JwtOptions();
if (Encoding.UTF8.GetByteCount(jwtOptions.Key) < JwtOptions.MinKeyBytes)
    throw new InvalidOperationException($"'Jwt:Key' en az {JwtOptions.MinKeyBytes} bayt olmalı. README'deki user-secrets adımına bakın.");
builder.Services.AddSingleton(jwtOptions);
builder.Services.AddScoped<ITokenService, TokenService>();

// Varsayılan şema Identity çerezi kalır; JWT yalnızca ApiControllerBase'den türeyen uçlarda devreye girer.
builder.Services.AddAuthentication()
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidIssuer = jwtOptions.Issuer,
            ValidAudience = jwtOptions.Audience,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtOptions.Key)),
            ValidateIssuerSigningKey = true,
            ClockSkew = TimeSpan.FromMinutes(1)
        };
        options.Events = new ApiJwtBearerEvents();
    });

builder.Services.AddSingleton<IEmailDomainValidator>(new EmailDomainValidator(
    builder.Configuration.GetSection("AllowedEmailDomains").Get<string[]>() ?? []));
builder.Services.AddScoped<IPhotoService, PhotoService>();
builder.Services.AddScoped<IItemService, ItemService>();
builder.Services.AddProblemDetails();
builder.Services.AddControllersWithViews()
    .AddJsonOptions(options =>
    {
        options.JsonSerializerOptions.Converters.Add(new JsonStringEnumConverter(allowIntegerValues: false));
        // ProblemDetails.errors anahtarları da camelCase çıksın.
        options.JsonSerializerOptions.DictionaryKeyPolicy = JsonNamingPolicy.CamelCase;
    });

builder.Services.AddSwaggerGen(options =>
{
    options.SwaggerDoc("v1", new OpenApiInfo { Title = "Kampüs Kayıp Eşya API", Version = "v1" });
    options.DocInclusionPredicate((_, api) => api.RelativePath?.StartsWith("api/") == true);
    options.AddSecurityDefinition(JwtBearerDefaults.AuthenticationScheme, new OpenApiSecurityScheme
    {
        Type = SecuritySchemeType.Http,
        Scheme = "bearer",
        BearerFormat = "JWT",
        Description = "/api/auth/login yanıtındaki token"
    });
    options.AddSecurityRequirement(document => new OpenApiSecurityRequirement
    {
        [new OpenApiSecuritySchemeReference(JwtBearerDefaults.AuthenticationScheme, document)] = []
    });
});

var app = builder.Build();

// dotnet ef komutları da Program.cs'i çalıştırır; o sırada veritabanına dokunma.
if (!EF.IsDesignTime)
    await IdentitySeeder.SeedRolesAndAdminAsync(app.Services);

if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
    app.UseSwagger();
    app.UseSwaggerUI();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
    // API hataları HTML sayfası yerine ProblemDetails döner.
    app.UseWhen(c => c.Request.Path.StartsWithSegments("/api"), api => api.UseExceptionHandler());
}

app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();

app.UseAuthentication();
app.UseAuthorization();

app.MapStaticAssets();

app.MapControllerRoute(
    name: "areas",
    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}")
    .WithStaticAssets();

app.Run();
