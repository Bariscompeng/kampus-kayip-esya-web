using KampusKayipEsya.Web.Models;
using KampusKayipEsya.Web.Services;
using Microsoft.AspNetCore.Mvc;

namespace KampusKayipEsya.Web.ViewComponents;

// Menüdeki onay bekleyen ilan rozeti; yalnızca görevli ve admin görür.
public class PendingItemCountViewComponent(IItemService itemService) : ViewComponent
{
    public async Task<IViewComponentResult> InvokeAsync()
    {
        if (!UserClaimsPrincipal.IsInRole(Roles.Gorevli) && !UserClaimsPrincipal.IsInRole(Roles.Admin))
            return Content(string.Empty);

        var sayi = await itemService.CountPendingAsync(HttpContext.RequestAborted);
        return sayi == 0 ? Content(string.Empty) : View(sayi);
    }
}
