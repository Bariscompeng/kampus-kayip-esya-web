using System.Security.Claims;
using KampusKayipEsya.Web.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KampusKayipEsya.Web.Services;

public class ApiJwtBearerEvents : JwtBearerEvents
{
    // Pasifleştirilen ya da silinen kullanıcının eski token'ı da 401 alır.
    public override async Task TokenValidated(TokenValidatedContext context)
    {
        var userManager = context.HttpContext.RequestServices.GetRequiredService<UserManager<AppUser>>();
        var userId = context.Principal?.FindFirstValue(ClaimTypes.NameIdentifier);
        var user = userId is null ? null : await userManager.FindByIdAsync(userId);

        if (user is null || await userManager.IsLockedOutAsync(user))
            context.Fail("Kullanıcı bulunamadı ya da kilitli.");
    }

    public override async Task Challenge(JwtBearerChallengeContext context)
    {
        context.HandleResponse();
        context.Response.Headers.WWWAuthenticate = "Bearer";
        await WriteProblemAsync(context.Response, StatusCodes.Status401Unauthorized, "Oturum açmanız gerekiyor.");
    }

    public override Task Forbidden(ForbiddenContext context) =>
        WriteProblemAsync(context.Response, StatusCodes.Status403Forbidden, "Bu işlem için yetkiniz yok.");

    private static Task WriteProblemAsync(HttpResponse response, int status, string title)
    {
        response.StatusCode = status;
        return response.WriteAsJsonAsync(
            new ProblemDetails { Status = status, Title = title }, options: null, contentType: "application/problem+json");
    }
}
