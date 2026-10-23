using System.Diagnostics;
using KampusKayipEsya.Web.Data;
using KampusKayipEsya.Web.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KampusKayipEsya.Web.Controllers;

public class HomeController(AppDbContext context) : Controller
{
    public async Task<IActionResult> Index()
    {
        var kategoriler = await context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Id)
            .ToListAsync();

        return View(kategoriler);
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
