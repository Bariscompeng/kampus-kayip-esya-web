using KampusKayipEsya.Web.Models;
using Microsoft.AspNetCore.Identity;

namespace KampusKayipEsya.Web.Data;

public static class IdentitySeeder
{
    // Roller her açılışta garanti edilir. Admin ve test görevlisinin şifreleri user-secrets'tan gelir;
    // şifre girilmemişse o hesap atlanır ve uyarı yazılır (uygulama yine açılır).
    public static async Task SeedRolesAndAdminAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<AppUser>>();
        var config = scope.ServiceProvider.GetRequiredService<IConfiguration>();
        var log = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger(nameof(IdentitySeeder));

        foreach (var role in Roles.All)
        {
            if (!await roleManager.RoleExistsAsync(role))
                await roleManager.CreateAsync(new IdentityRole(role));
        }

        await EnsureUserAsync(userManager, log, config.GetSection("Seed:Admin"), Roles.Admin);
        await EnsureUserAsync(userManager, log, config.GetSection("Seed:Gorevli"), Roles.Gorevli);
    }

    private static async Task EnsureUserAsync(UserManager<AppUser> userManager, ILogger log,
        IConfigurationSection section, string role)
    {
        var email = section["Email"];
        var password = section["Password"];
        if (string.IsNullOrWhiteSpace(email) || string.IsNullOrWhiteSpace(password))
        {
            log.LogWarning("{Role} hesabı oluşturulmadı: user-secrets'ta {Section}:Email ve {Section}:Password girilmeli.",
                role, section.Path, section.Path);
            return;
        }

        var user = await userManager.FindByEmailAsync(email);
        if (user is null)
        {
            user = new AppUser
            {
                UserName = email,
                Email = email,
                EmailConfirmed = true,
                FullName = section["FullName"] ?? role
            };
            var result = await userManager.CreateAsync(user, password);
            if (!result.Succeeded)
            {
                log.LogError("{Role} hesabı oluşturulamadı: {Errors}", role,
                    string.Join(" ", result.Errors.Select(e => e.Description)));
                return;
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
            await userManager.AddToRoleAsync(user, role);
    }
}
