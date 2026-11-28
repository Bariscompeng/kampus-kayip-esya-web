using KampusKayipEsya.Web.Models;
using KampusKayipEsya.Web.Services;
using KampusKayipEsya.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KampusKayipEsya.Web.Controllers;

[Authorize]
public class ClaimsController(IClaimService claimService, UserManager<AppUser> userManager) : Controller
{
    [HttpGet("/items/{id:int}/talep")]
    public async Task<IActionResult> Create(int id)
    {
        var ilan = await claimService.GetClaimableItemAsync(id);
        if (ilan is null) return NotFound();

        var engel = await TalepEngeli(ilan);
        if (engel is not null) return engel;

        return View(new ClaimCreateViewModel { Item = ilan });
    }

    [HttpPost("/items/{id:int}/talep")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(int id, ClaimCreateViewModel model)
    {
        var ilan = await claimService.GetClaimableItemAsync(id);
        if (ilan is null) return NotFound();

        var engel = await TalepEngeli(ilan);
        if (engel is not null) return engel;

        if (!ModelState.IsValid)
        {
            model.Item = ilan;
            return View(model);
        }

        var sonuc = await claimService.CreateAsync(id, KullaniciId, new ClaimInput(model.VerificationAnswer, model.Message));
        switch (sonuc.Result)
        {
            case ClaimCreateResult.Success:
                TempData["Basari"] = "Talebiniz ilan sahibine iletildi. Cevabınızı inceleyip size dönecek.";
                return RedirectToAction(nameof(Mine));
            case ClaimCreateResult.AlreadyClaimed:
                TempData["Hata"] = "Bu ilan için zaten bir talebiniz var.";
                return RedirectToAction(nameof(Mine));
            case ClaimCreateResult.OwnItem:
                TempData["Hata"] = "Kendi ilanınız için talep gönderemezsiniz.";
                return Redirect($"/items/{id}");
            default:
                return NotFound();
        }
    }

    [HttpGet("/taleplerim")]
    public async Task<IActionResult> Mine() =>
        View(await claimService.GetMineAsync(KullaniciId));

    [HttpGet("/gelen-talepler")]
    public async Task<IActionResult> Incoming() =>
        View(new IncomingClaimsViewModel(await claimService.GetIncomingAsync(KullaniciId)));

    [HttpPost("/talepler/{id:int}/geri-cek")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Withdraw(int id)
    {
        var sonuc = await claimService.WithdrawAsync(id, KullaniciId);
        TempData[sonuc == ItemChangeResult.Success ? "Basari" : "Hata"] = sonuc switch
        {
            ItemChangeResult.Success => "Talebiniz geri çekildi.",
            ItemChangeResult.Conflict => "Yalnızca yanıt bekleyen bir talep geri çekilebilir.",
            _ => "Talep bulunamadı."
        };
        return RedirectToAction(nameof(Mine));
    }

    private string KullaniciId => userManager.GetUserId(User)!;

    private async Task<IActionResult?> TalepEngeli(Item ilan)
    {
        if (ilan.OwnerId == KullaniciId)
        {
            TempData["Hata"] = "Kendi ilanınız için talep gönderemezsiniz.";
            return Redirect($"/items/{ilan.Id}");
        }

        if (await claimService.GetActiveClaimAsync(ilan.Id, KullaniciId) is not null)
        {
            TempData["Hata"] = "Bu ilan için zaten bir talebiniz var.";
            return RedirectToAction(nameof(Mine));
        }

        return null;
    }
}
