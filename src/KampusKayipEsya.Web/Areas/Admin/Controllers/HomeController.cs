using Microsoft.AspNetCore.Mvc;

namespace KampusKayipEsya.Web.Areas.Admin.Controllers;

public class HomeController : AdminControllerBase
{
    public IActionResult Index() => View();
}
