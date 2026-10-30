using KampusKayipEsya.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KampusKayipEsya.Web.Areas.Gorevli.Controllers;

// Admin de görevli sayfalarını görebilir.
[Area("Gorevli")]
[Authorize(Roles = Roles.Gorevli + "," + Roles.Admin)]
public abstract class GorevliControllerBase : Controller
{
}
