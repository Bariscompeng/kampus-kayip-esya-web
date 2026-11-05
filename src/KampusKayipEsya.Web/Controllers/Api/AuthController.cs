using System.Security.Claims;
using KampusKayipEsya.Web.Dtos;
using KampusKayipEsya.Web.Models;
using KampusKayipEsya.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KampusKayipEsya.Web.Controllers.Api;

[Route("api/auth")]
public class AuthController(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    IEmailDomainValidator emailDomainValidator,
    ITokenService tokenService) : ApiControllerBase
{
    [HttpPost("register")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Register(RegisterRequest request)
    {
        if (!emailDomainValidator.IsAllowed(request.Email))
        {
            ModelState.AddModelError(nameof(request.Email),
                $"Yalnızca okul e-postasıyla kayıt olabilirsiniz ({string.Join(", ", emailDomainValidator.AllowedDomains.Select(d => "@" + d))}).");
            return ValidationProblem(ModelState);
        }

        var user = new AppUser
        {
            UserName = request.Email.Trim(),
            Email = request.Email.Trim(),
            FullName = request.FullName.Trim(),
            StudentNo = string.IsNullOrWhiteSpace(request.StudentNo) ? null : request.StudentNo.Trim()
        };

        var result = await userManager.CreateAsync(user, request.Password);
        if (!result.Succeeded)
        {
            foreach (var description in result.Errors.Select(e => e.Description).Distinct())
                ModelState.AddModelError(string.Empty, description);
            return ValidationProblem(ModelState);
        }

        // Yeni kayıt her zaman öğrenci; görevli/admin rolünü yalnızca admin verir.
        await userManager.AddToRoleAsync(user, Roles.Ogrenci);

        return Created("/api/auth/me", await CreateResponseAsync(user));
    }

    [HttpPost("login")]
    [AllowAnonymous]
    public async Task<ActionResult<AuthResponse>> Login(LoginRequest request)
    {
        var user = await userManager.FindByEmailAsync(request.Email.Trim());
        if (user is null)
            return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "E-posta veya şifre hatalı.");

        var result = await signInManager.CheckPasswordSignInAsync(user, request.Password, lockoutOnFailure: true);
        if (result.IsLockedOut)
            return Problem(statusCode: StatusCodes.Status401Unauthorized,
                title: "Hesabınız kilitli. Çok sayıda hatalı deneme yaptıysanız birkaç dakika sonra tekrar deneyin; hesabınız pasifleştirildiyse yöneticiye başvurun.");
        if (!result.Succeeded)
            return Problem(statusCode: StatusCodes.Status401Unauthorized, title: "E-posta veya şifre hatalı.");

        return await CreateResponseAsync(user);
    }

    [HttpGet("me")]
    public async Task<ActionResult<UserDto>> Me()
    {
        var user = await userManager.FindByIdAsync(User.FindFirstValue(ClaimTypes.NameIdentifier)!);
        if (user is null) return Unauthorized();

        return ToDto(user, await userManager.GetRolesAsync(user));
    }

    private async Task<AuthResponse> CreateResponseAsync(AppUser user)
    {
        var roles = await userManager.GetRolesAsync(user);
        var (token, expiresAt) = tokenService.CreateToken(user, roles);
        return new AuthResponse(token, expiresAt, ToDto(user, roles));
    }

    private static UserDto ToDto(AppUser user, IEnumerable<string> roles) =>
        new(user.Id, user.Email!, user.FullName, user.StudentNo, roles.ToList());
}
