using KampusKayipEsya.Web.Data;
using KampusKayipEsya.Web.Dtos;
using KampusKayipEsya.Web.Models;
using KampusKayipEsya.Web.Models.Enums;
using KampusKayipEsya.Web.Services;
using KampusKayipEsya.Web.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace KampusKayipEsya.Web.Controllers;

[Authorize]
public class ItemsController(AppDbContext context, IItemService itemService, IPhotoService photoService, UserManager<AppUser> userManager) : Controller
{
    public async Task<IActionResult> Create()
    {
        var model = new ItemCreateViewModel { EventDate = DateTime.Today };
        await ListeleriDoldur(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(30_000_000)]
    public async Task<IActionResult> Create(ItemCreateViewModel model)
    {
        foreach (var hata in photoService.Validate(model.Photos))
            ModelState.AddModelError(nameof(model.Photos), hata);

        await ReferanslariKontrolEt(model);

        if (!ModelState.IsValid)
        {
            await ListeleriDoldur(model);
            return View(model);
        }

        var ilan = await itemService.CreateAsync(GirdiOlustur(model), userManager.GetUserId(User)!, model.Photos);

        TempData["Basari"] = "İlanınız alındı. Görevli onayladıktan sonra herkes görebilecek.";
        return RedirectToAction(nameof(Details), new { id = ilan.Id });
    }

    [AllowAnonymous]
    [HttpGet("/items/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var izleyen = ItemViewer.From(User);
        var ilan = await itemService.GetVisibleAsync(id, izleyen);
        if (ilan is null) return NotFound();

        var sahibiMi = izleyen.Owns(ilan);
        return View(new ItemDetailsViewModel(ilan, sahibiMi, sahibiMi || izleyen.IsStaff, ilan.Owner.ShortName));
    }

    [HttpGet("/profilim")]
    public async Task<IActionResult> Mine(string? sekme)
    {
        var ilanlar = await itemService.GetOwnedAsync(userManager.GetUserId(User)!, null, 1, 1000);
        return View(new MyItemsViewModel(ilanlar.Items, sekme));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var ilan = await SahibiOlunanIlan(id);
        if (ilan is null) return NotFound();
        if (DegistirilemezMi(ilan)) return KapanmisIlanaDon();

        var model = new ItemEditViewModel
        {
            Id = ilan.Id,
            Status = ilan.Status,
            Type = ilan.Type,
            Title = ilan.Title,
            CategoryId = ilan.CategoryId,
            LocationId = ilan.LocationId,
            EventDate = ilan.EventDate,
            Description = ilan.Description,
            VerificationQuestion = ilan.VerificationQuestion,
            ExistingPhotos = ilan.Photos.OrderBy(p => p.Order).ToList()
        };
        await ListeleriDoldur(model);
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [RequestSizeLimit(30_000_000)]
    public async Task<IActionResult> Edit(int id, ItemEditViewModel model)
    {
        if (id != model.Id) return BadRequest();

        var ilan = await SahibiOlunanIlan(id);
        if (ilan is null) return NotFound();
        if (DegistirilemezMi(ilan)) return KapanmisIlanaDon();

        var kalanFotoSayisi = ilan.Photos.Count(p => !model.RemovePhotoIds.Contains(p.Id));
        foreach (var hata in photoService.Validate(model.Photos, kalanFotoSayisi))
            ModelState.AddModelError(nameof(model.Photos), hata);

        await ReferanslariKontrolEt(model);

        if (!ModelState.IsValid)
        {
            model.Status = ilan.Status;
            model.ExistingPhotos = ilan.Photos.OrderBy(p => p.Order).ToList();
            await ListeleriDoldur(model);
            return View(model);
        }

        await using (var islem = await context.Database.BeginTransactionAsync())
        {
            var sonuc = await itemService.UpdateAsync(id, GirdiOlustur(model), ItemViewer.From(User));
            if (sonuc != ItemChangeResult.Success) return KapanmisIlanaDon();

            await photoService.DeleteAsync(id, model.RemovePhotoIds);
            if (model.Photos.Count > 0)
                await photoService.SaveAsync(id, model.Photos);
            await islem.CommitAsync();
        }

        TempData["Basari"] = ilan.Status is ItemStatus.Published or ItemStatus.Rejected
            ? "İlanınız güncellendi ve yeniden görevli onayına gönderildi."
            : "İlanınız güncellendi.";
        return RedirectToAction(nameof(Details), new { id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Resolve(int id)
    {
        var sonuc = await itemService.MarkResolvedAsync(id, ItemViewer.From(User));
        TempData[sonuc == ItemChangeResult.Success ? "Basari" : "Hata"] = sonuc switch
        {
            ItemChangeResult.Success => "İlan \"Teslim edildi\" olarak kapatıldı.",
            ItemChangeResult.Conflict => "Yalnızca yayındaki bir ilan \"Teslim edildi\" olarak kapatılabilir.",
            _ => "İlan bulunamadı."
        };
        return RedirectToAction(nameof(Mine), new { sekme = sonuc == ItemChangeResult.Success ? MyItemsViewModel.Kapanmis : null });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Delete(int id, string? sekme)
    {
        var sonuc = await itemService.DeleteAsync(id, ItemViewer.From(User));
        TempData[sonuc == ItemChangeResult.Success ? "Basari" : "Hata"] = sonuc switch
        {
            ItemChangeResult.Success => "İlan ve fotoğrafları silindi.",
            ItemChangeResult.Conflict => "Teslim edilmiş ya da arşivlenmiş ilan silinemez.",
            _ => "İlan bulunamadı."
        };
        return RedirectToAction(nameof(Mine), new { sekme });
    }

    private async Task<Item?> SahibiOlunanIlan(int id)
    {
        var izleyen = ItemViewer.From(User);
        var ilan = await itemService.GetVisibleAsync(id, izleyen);
        return ilan is not null && izleyen.Owns(ilan) ? ilan : null;
    }

    private static bool DegistirilemezMi(Item ilan) =>
        ilan.Status is ItemStatus.Resolved or ItemStatus.Archived;

    private IActionResult KapanmisIlanaDon()
    {
        TempData["Hata"] = "Teslim edilmiş ya da arşivlenmiş ilan değiştirilemez.";
        return RedirectToAction(nameof(Mine), new { sekme = MyItemsViewModel.Kapanmis });
    }

    private async Task ReferanslariKontrolEt(ItemCreateViewModel model)
    {
        if (model.CategoryId is int kategoriId && model.LocationId is int konumId)
        {
            foreach (var (alan, hata) in await itemService.ValidateReferencesAsync(kategoriId, konumId))
                ModelState.AddModelError(alan, hata);
        }
    }

    private static ItemInput GirdiOlustur(ItemCreateViewModel model) => new(
        model.Type!.Value,
        model.Title,
        model.Description,
        model.EventDate!.Value,
        model.CategoryId!.Value,
        model.LocationId!.Value,
        model.VerificationQuestion);

    private async Task ListeleriDoldur(ItemCreateViewModel model)
    {
        model.Categories = await context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new SelectListItem(c.Name, c.Id.ToString()))
            .ToListAsync();

        model.Locations = await context.Locations
            .AsNoTracking()
            .OrderBy(l => l.Building).ThenBy(l => l.FloorOrRoom)
            .Select(l => new SelectListItem(l.FloorOrRoom == null ? l.Building : l.Building + " – " + l.FloorOrRoom, l.Id.ToString()))
            .ToListAsync();
    }
}
