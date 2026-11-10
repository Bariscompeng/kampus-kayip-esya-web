using System.Diagnostics;
using KampusKayipEsya.Web.Data;
using KampusKayipEsya.Web.Dtos;
using KampusKayipEsya.Web.Models;
using KampusKayipEsya.Web.Models.Enums;
using KampusKayipEsya.Web.Services;
using KampusKayipEsya.Web.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KampusKayipEsya.Web.Controllers;

public class HomeController(AppDbContext context, IItemService itemService) : Controller
{
    private const int SeritBoyutu = 4;

    public async Task<IActionResult> Index()
    {
        var kategoriler = await context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .ToListAsync();

        var bulunanlar = await itemService.SearchAsync(new ItemSearchRequest { Type = ItemType.Found, PageSize = SeritBoyutu });
        var kayiplar = await itemService.SearchAsync(new ItemSearchRequest { Type = ItemType.Lost, PageSize = SeritBoyutu });

        return View(new HomeIndexViewModel(kategoriler, bulunanlar.Items, kayiplar.Items));
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
