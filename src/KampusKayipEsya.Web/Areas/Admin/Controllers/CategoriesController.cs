using KampusKayipEsya.Web.Data;
using KampusKayipEsya.Web.Models;
using KampusKayipEsya.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KampusKayipEsya.Web.Areas.Admin.Controllers;

public class CategoriesController(AppDbContext context) : AdminControllerBase
{
    public async Task<IActionResult> Index()
    {
        var kategoriler = await context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryListItem(c.Id, c.Name, c.Icon, c.Items.Count))
            .ToListAsync();

        return View(kategoriler);
    }

    public IActionResult Create()
    {
        return View(new CategoryFormViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(CategoryFormViewModel model)
    {
        await AdTekrariniKontrolEt(model);
        if (!ModelState.IsValid) return View(model);

        context.Categories.Add(new Category { Name = model.Name.Trim(), Icon = BosIseNull(model.Icon) });
        await context.SaveChangesAsync();

        TempData["Basari"] = $"\"{model.Name.Trim()}\" kategorisi eklendi.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var kategori = await context.Categories.FindAsync(id);
        if (kategori is null) return NotFound();

        return View(new CategoryFormViewModel { Id = kategori.Id, Name = kategori.Name, Icon = kategori.Icon });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, CategoryFormViewModel model)
    {
        if (id != model.Id) return BadRequest();

        var kategori = await context.Categories.FindAsync(id);
        if (kategori is null) return NotFound();

        await AdTekrariniKontrolEt(model);
        if (!ModelState.IsValid) return View(model);

        kategori.Name = model.Name.Trim();
        kategori.Icon = BosIseNull(model.Icon);
        await context.SaveChangesAsync();

        TempData["Basari"] = $"\"{kategori.Name}\" kategorisi güncellendi.";
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Delete(int id)
    {
        var kategori = await context.Categories
            .AsNoTracking()
            .Where(c => c.Id == id)
            .Select(c => new CategoryListItem(c.Id, c.Name, c.Icon, c.Items.Count))
            .FirstOrDefaultAsync();
        if (kategori is null) return NotFound();

        return View(kategori);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var kategori = await context.Categories.FindAsync(id);
        if (kategori is null) return NotFound();

        var ilanSayisi = await context.Items.CountAsync(i => i.CategoryId == id);
        if (ilanSayisi > 0)
        {
            TempData["Hata"] = $"\"{kategori.Name}\" kategorisine bağlı {ilanSayisi} ilan olduğu için silinemez.";
            return RedirectToAction(nameof(Index));
        }

        context.Categories.Remove(kategori);
        try
        {
            await context.SaveChangesAsync();
            TempData["Basari"] = $"\"{kategori.Name}\" kategorisi silindi.";
        }
        catch (DbUpdateException)
        {
            TempData["Hata"] = $"\"{kategori.Name}\" kategorisi bu sırada bir ilana bağlandığı için silinemedi.";
        }

        return RedirectToAction(nameof(Index));
    }

    private async Task AdTekrariniKontrolEt(CategoryFormViewModel model)
    {
        var ad = model.Name?.Trim();
        if (string.IsNullOrEmpty(ad)) return;

        if (await context.Categories.AnyAsync(c => c.Name == ad && c.Id != model.Id))
            ModelState.AddModelError(nameof(model.Name), "Bu adla bir kategori zaten var.");
    }

    private static string? BosIseNull(string? deger) =>
        string.IsNullOrWhiteSpace(deger) ? null : deger.Trim();
}
