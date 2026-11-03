using KampusKayipEsya.Web.Data;
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
public class ItemsController(AppDbContext context, IPhotoService photoService, UserManager<AppUser> userManager) : Controller
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

        if (model.CategoryId is int kategoriId && !await context.Categories.AnyAsync(c => c.Id == kategoriId))
            ModelState.AddModelError(nameof(model.CategoryId), "Geçerli bir kategori seçin.");
        if (model.LocationId is int konumId && !await context.Locations.AnyAsync(l => l.Id == konumId))
            ModelState.AddModelError(nameof(model.LocationId), "Geçerli bir konum seçin.");

        if (!ModelState.IsValid)
        {
            await ListeleriDoldur(model);
            return View(model);
        }

        var ilan = new Item
        {
            Type = model.Type!.Value,
            Title = model.Title.Trim(),
            Description = model.Description.Trim(),
            EventDate = model.EventDate!.Value.Date,
            VerificationQuestion = model.Type == ItemType.Found ? model.VerificationQuestion!.Trim() : null,
            CategoryId = model.CategoryId!.Value,
            LocationId = model.LocationId!.Value,
            OwnerId = userManager.GetUserId(User)!,
            Status = ItemStatus.Pending
        };

        await using var islem = await context.Database.BeginTransactionAsync();
        context.Items.Add(ilan);
        await context.SaveChangesAsync();
        if (model.Photos.Count > 0)
            await photoService.SaveAsync(ilan.Id, model.Photos);
        await islem.CommitAsync();

        TempData["Basari"] = "İlanınız alındı. Görevli onayladıktan sonra herkes görebilecek.";
        return RedirectToAction(nameof(Details), new { id = ilan.Id });
    }

    [AllowAnonymous]
    [HttpGet("/items/{id:int}")]
    public async Task<IActionResult> Details(int id)
    {
        var ilan = await context.Items
            .AsNoTracking()
            .Include(i => i.Category)
            .Include(i => i.Location)
            .Include(i => i.Owner)
            .Include(i => i.Photos.OrderBy(p => p.Order))
            .FirstOrDefaultAsync(i => i.Id == id);
        if (ilan is null) return NotFound();

        var sahibiMi = ilan.OwnerId == userManager.GetUserId(User);
        var yetkiliMi = User.IsInRole(Roles.Gorevli) || User.IsInRole(Roles.Admin);
        if (ilan.Status != ItemStatus.Published && !sahibiMi && !yetkiliMi) return NotFound();

        return View(new ItemDetailsViewModel(ilan, sahibiMi, sahibiMi || yetkiliMi, KisaAd(ilan.Owner.FullName)));
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

    private static string KisaAd(string adSoyad)
    {
        var parcalar = adSoyad.Split(' ', StringSplitOptions.RemoveEmptyEntries);
        return parcalar.Length switch
        {
            0 => "Kullanıcı",
            1 => parcalar[0],
            _ => $"{parcalar[0]} {char.ToUpper(parcalar[^1][0], new System.Globalization.CultureInfo("tr-TR"))}."
        };
    }
}
