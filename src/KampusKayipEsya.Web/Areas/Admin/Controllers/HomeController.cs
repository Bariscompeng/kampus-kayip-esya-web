using KampusKayipEsya.Web.Data;
using KampusKayipEsya.Web.Models.Enums;
using KampusKayipEsya.Web.ViewModels.Admin;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KampusKayipEsya.Web.Areas.Admin.Controllers;

public class HomeController(AppDbContext context) : AdminControllerBase
{
    public async Task<IActionResult> Index()
    {
        var model = new AdminDashboardViewModel(
            CategoryCount: await context.Categories.CountAsync(),
            LocationCount: await context.Locations.CountAsync(),
            ItemCount: await context.Items.CountAsync(),
            PendingItemCount: await context.Items.CountAsync(i => i.Status == ItemStatus.Pending),
            UserCount: await context.Users.CountAsync());

        return View(model);
    }
}
