using KampusKayipEsya.Web.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KampusKayipEsya.Web.Areas.Admin.Controllers;

// Admin alanındaki tüm controller'lar bundan türer; yetki kuralı tek yerde kalır.
[Area("Admin")]
[Authorize(Roles = Roles.Admin)]
public abstract class AdminControllerBase : Controller
{
}
