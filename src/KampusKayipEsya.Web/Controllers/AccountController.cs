using KampusKayipEsya.Web.Models;
using KampusKayipEsya.Web.Services;
using KampusKayipEsya.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KampusKayipEsya.Web.Controllers;

public class AccountController(
    UserManager<AppUser> userManager,
    SignInManager<AppUser> signInManager,
    IEmailDomainValidator emailDomainValidator) : Controller
{
    [HttpGet]
    [AllowAnonymous]
    public IActionResult Register()
    {
        if (signInManager.IsSignedIn(User)) return RedirectToAction("Index", "Home");
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!emailDomainValidator.IsAllowed(model.Email))
        {
            ModelState.AddModelError(nameof(model.Email),
                $"Yalnızca okul e-postasıyla kayıt olabilirsiniz ({string.Join(", ", emailDomainValidator.AllowedDomains.Select(d => "@" + d))}).");
        }

        if (!ModelState.IsValid) return View(model);

        var user = new AppUser
        {
            UserName = model.Email.Trim(),
            Email = model.Email.Trim(),
            FullName = model.FullName.Trim(),
            StudentNo = string.IsNullOrWhiteSpace(model.StudentNo) ? null : model.StudentNo.Trim()
        };

        var result = await userManager.CreateAsync(user, model.Password);
        if (!result.Succeeded)
        {
            foreach (var description in result.Errors.Select(e => e.Description).Distinct())
                ModelState.AddModelError(string.Empty, description);
            return View(model);
        }

        // Yeni kayıt her zaman öğrenci; görevli/admin rolünü yalnızca admin verir.
        await userManager.AddToRoleAsync(user, Roles.Ogrenci);
        await signInManager.SignInAsync(user, isPersistent: false);

        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult Login(string? returnUrl = null)
    {
        if (signInManager.IsSignedIn(User)) return RedirectToAction("Index", "Home");
        ViewData["ReturnUrl"] = returnUrl;
        return View();
    }

    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model, string? returnUrl = null)
    {
        ViewData["ReturnUrl"] = returnUrl;
        if (!ModelState.IsValid) return View(model);

        // UserName = Email olarak kaydedildiği için e-postayla giriş yapılır.
        var result = await signInManager.PasswordSignInAsync(
            model.Email.Trim(), model.Password, model.RememberMe, lockoutOnFailure: true);

        if (result.Succeeded)
        {
            if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                return LocalRedirect(returnUrl);
            return RedirectToAction("Index", "Home");
        }

        ModelState.AddModelError(string.Empty, result.IsLockedOut
            ? "Hesabınız kilitli. Çok sayıda hatalı deneme yaptıysanız birkaç dakika sonra tekrar deneyin; hesabınız pasifleştirildiyse yöneticiye başvurun."
            : "E-posta veya şifre hatalı.");
        return View(model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await signInManager.SignOutAsync();
        return RedirectToAction("Index", "Home");
    }

    [HttpGet]
    [AllowAnonymous]
    public IActionResult AccessDenied()
    {
        Response.StatusCode = StatusCodes.Status403Forbidden;
        return View();
    }
}
