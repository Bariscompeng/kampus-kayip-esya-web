using KampusKayipEsya.Web.Data;
using KampusKayipEsya.Web.Models;
using KampusKayipEsya.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KampusKayipEsya.Web.Areas.Admin.Controllers;

public class LocationsController(AppDbContext context) : AdminControllerBase
{
    public async Task<IActionResult> Index()
    {
        var konumlar = await context.Locations
            .AsNoTracking()
            .OrderBy(l => l.Building).ThenBy(l => l.FloorOrRoom)
            .Select(l => new LocationListItem(l.Id, l.Building, l.FloorOrRoom, l.Latitude, l.Longitude, l.Items.Count))
            .ToListAsync();

        return View(konumlar);
    }

    public IActionResult Create()
    {
        return View(new LocationFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(LocationFormViewModel model)
    {
        await TekrariKontrolEt(model);
        if (!ModelState.IsValid) return View(model);

        var konum = new Location();
        FormuAktar(model, konum);
        context.Locations.Add(konum);
        await context.SaveChangesAsync();

        TempData["Basari"] = $"\"{Etiket(konum)}\" konumu eklendi.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var konum = await context.Locations.FindAsync(id);
        if (konum is null) return NotFound();

        return View(new LocationFormViewModel
        {
            Id = konum.Id,
            Building = konum.Building,
            FloorOrRoom = konum.FloorOrRoom,
            Latitude = LocationFormViewModel.MetneCevir(konum.Latitude),
            Longitude = LocationFormViewModel.MetneCevir(konum.Longitude)
        });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, LocationFormViewModel model)
    {
        if (id != model.Id) return BadRequest();

        var konum = await context.Locations.FindAsync(id);
        if (konum is null) return NotFound();

        await TekrariKontrolEt(model);
        if (!ModelState.IsValid) return View(model);

        FormuAktar(model, konum);
        await context.SaveChangesAsync();

        TempData["Basari"] = $"\"{Etiket(konum)}\" konumu güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var konum = await context.Locations
            .AsNoTracking()
            .Where(l => l.Id == id)
            .Select(l => new LocationListItem(l.Id, l.Building, l.FloorOrRoom, l.Latitude, l.Longitude, l.Items.Count))
            .FirstOrDefaultAsync();
        if (konum is null) return NotFound();

        return View(konum);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var konum = await context.Locations.FindAsync(id);
        if (konum is null) return NotFound();

        var ilanSayisi = await context.Items.CountAsync(i => i.LocationId == id);
        if (ilanSayisi > 0)
        {
            TempData["Hata"] = $"\"{Etiket(konum)}\" konumuna bağlı {ilanSayisi} ilan olduğu için silinemez.";
            return RedirectToAction(nameof(Index));
        }

        context.Locations.Remove(konum);
        try
        {
            await context.SaveChangesAsync();
            TempData["Basari"] = $"\"{Etiket(konum)}\" konumu silindi.";
        }
        catch (DbUpdateException)
        {
            TempData["Hata"] = $"\"{Etiket(konum)}\" konumu bu sırada bir ilana bağlandığı için silinemedi.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task TekrariKontrolEt(LocationFormViewModel model)
    {
        var bina = model.Building?.Trim();
        if (string.IsNullOrEmpty(bina)) return;

        var katDerslik = string.IsNullOrWhiteSpace(model.FloorOrRoom) ? null : model.FloorOrRoom.Trim();
        if (await context.Locations.AnyAsync(l => l.Building == bina && l.FloorOrRoom == katDerslik && l.Id != model.Id))
            ModelState.AddModelError(nameof(model.Building), "Bu bina ve kat/derslik zaten kayıtlı.");
    }

    private static void FormuAktar(LocationFormViewModel model, Location konum)
    {
        konum.Building = model.Building.Trim();
        konum.FloorOrRoom = string.IsNullOrWhiteSpace(model.FloorOrRoom) ? null : model.FloorOrRoom.Trim();
        konum.Latitude = model.EnlemSayisi;
        konum.Longitude = model.BoylamSayisi;
    }

    private static string Etiket(Location konum) =>
        konum.FloorOrRoom is null ? konum.Building : $"{konum.Building} – {konum.FloorOrRoom}";
}
