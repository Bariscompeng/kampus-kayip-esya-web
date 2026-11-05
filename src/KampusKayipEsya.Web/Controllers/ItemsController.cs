using KampusKayipEsya.Web.Data;
using KampusKayipEsya.Web.Dtos;
using KampusKayipEsya.Web.Models;
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

        if (model.CategoryId is int kategoriId && model.LocationId is int konumId)
        {
            foreach (var (alan, hata) in await itemService.ValidateReferencesAsync(kategoriId, konumId))
                ModelState.AddModelError(alan, hata);
        }

        if (!ModelState.IsValid)
        {
            await ListeleriDoldur(model);
            return View(model);
        }

        var ilan = await itemService.CreateAsync(
            new ItemInput(
                model.Type!.Value,
                model.Title,
                model.Description,
                model.EventDate!.Value,
                model.CategoryId!.Value,
                model.LocationId!.Value,
                model.VerificationQuestion),
            userManager.GetUserId(User)!,
            model.Photos);

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
