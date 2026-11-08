using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KampusKayipEsya.Web.Controllers.Api;

// Tüm API uçları yalnızca JWT kabul eder; site çerezi burada geçmez, bu yüzden CSRF riski de yoktur.
[ApiController]
[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
[Produces("application/json")]
public abstract class ApiControllerBase : ControllerBase
{
}
