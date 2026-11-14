using KampusKayipEsya.Web.Services;
using KampusKayipEsya.Web.ViewModels.Gorevli;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using KampusKayipEsya.Web.Models;

namespace KampusKayipEsya.Web.Areas.Gorevli.Controllers;

// İlan onay kuyruğu: en eski bekleyen ilan en üstte.
public class ApprovalsController(IItemService itemService, UserManager<AppUser> userManager) : GorevliControllerBase
{
    private const int SayfaBoyutu = 10;

    public async Task<IActionResult> Index(int page = 1)
    {
        var bekleyenler = await itemService.GetPendingAsync(Math.Max(1, page), SayfaBoyutu);
        return View(new ApprovalQueueViewModel(bekleyenler));
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Approve(int id, int page = 1)
    {
        var sonuc = await itemService.ApproveAsync(id, userManager.GetUserId(User)!);
        BildirimYaz(sonuc, "İlan onaylandı ve yayına alındı.");
        return RedirectToAction(nameof(Index), new { page });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Reject(int id, RejectItemViewModel model, int page = 1)
    {
        if (!ModelState.IsValid)
        {
            TempData["Hata"] = ModelState.Values.SelectMany(v => v.Errors).First().ErrorMessage;
            return RedirectToAction(nameof(Index), new { page });
        }

        var sonuc = await itemService.RejectAsync(id, userManager.GetUserId(User)!, model.Reason);
        BildirimYaz(sonuc, "İlan reddedildi; sebep ilan sahibine gösterilecek.");
        return RedirectToAction(nameof(Index), new { page });
    }

    private void BildirimYaz(ItemChangeResult sonuc, string basariMesaji)
    {
        if (sonuc == ItemChangeResult.Success)
            TempData["Basari"] = basariMesaji;
        else
            TempData["Hata"] = sonuc == ItemChangeResult.Conflict
                ? "Bu ilan artık onay beklemiyor; başka bir görevli işlem yapmış ya da ilan sahibi değiştirmiş olabilir."
                : "İlan bulunamadı.";
    }
}
