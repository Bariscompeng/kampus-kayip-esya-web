using Microsoft.AspNetCore.Mvc;

namespace KampusKayipEsya.Web.Areas.Gorevli.Controllers;

public class HomeController : GorevliControllerBase
{
    public IActionResult Index() => View();
}
