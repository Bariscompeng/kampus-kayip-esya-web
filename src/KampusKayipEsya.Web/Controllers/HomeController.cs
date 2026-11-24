using System.Diagnostics;
using System.Xml.Linq;
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

    /// <summary>Arama motorları için tarama kuralları; panel, hesap ve API yolları dışarıda kalır.</summary>
    [HttpGet("/robots.txt")]
    [ResponseCache(Duration = 3600)]
    public ContentResult Robots()
    {
        var kok = $"{Request.Scheme}://{Request.Host}";
        var metin = $"""
            User-agent: *
            Disallow: /admin
            Disallow: /gorevli
            Disallow: /account
            Disallow: /api
            Disallow: /profilim
            Disallow: /items/create
            Disallow: /items/edit

            Sitemap: {kok}/sitemap.xml

            """;
        return Content(metin, "text/plain; charset=utf-8");
    }

    /// <summary>Yayındaki ilanlarla site haritası.</summary>
    [HttpGet("/sitemap.xml")]
    [ResponseCache(Duration = 3600)]
    public async Task<ContentResult> Sitemap()
    {
        var kok = $"{Request.Scheme}://{Request.Host}";
        var ilanlar = await context.Items
            .AsNoTracking()
            .Where(i => i.Status == ItemStatus.Published)
            .OrderByDescending(i => i.CreatedAt)
            .Select(i => new { i.Id, i.CreatedAt })
            .Take(5000)
            .ToListAsync();

        XNamespace ns = "http://www.sitemaps.org/schemas/sitemap/0.9";
        XElement Adres(string yol, DateTime? tarih = null) => new(ns + "url",
            new XElement(ns + "loc", kok + yol),
            tarih is null ? null : new XElement(ns + "lastmod", tarih.Value.ToString("yyyy-MM-dd")));

        var harita = new XElement(ns + "urlset",
            Adres("/"),
            Adres("/items"),
            Adres("/home/privacy"),
            ilanlar.Select(i => Adres($"/items/{i.Id}", i.CreatedAt)));

        return Content(new XDocument(new XDeclaration("1.0", "utf-8", null), harita).Declaration + "\n" + harita, "application/xml; charset=utf-8");
    }

    /// <summary>404 gibi gövdesiz hata yanıtları için sayfa (Program.cs: UseStatusCodePagesWithReExecute).</summary>
    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Durum(int id)
    {
        return View("StatusCode", id is >= 400 and <= 599 ? id : 404);
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}
