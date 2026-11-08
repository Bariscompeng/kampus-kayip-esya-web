using KampusKayipEsya.Web.Data;
using KampusKayipEsya.Web.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KampusKayipEsya.Web.Controllers.Api;

[Route("api/locations")]
[AllowAnonymous]
[Tags("Locations")]
public class LocationsApiController(AppDbContext context) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<LocationDto>>> GetAll(CancellationToken cancellationToken) =>
        await context.Locations
            .AsNoTracking()
            .OrderBy(l => l.Building).ThenBy(l => l.FloorOrRoom)
            .Select(l => new LocationDto(l.Id, l.Building, l.FloorOrRoom, l.Latitude, l.Longitude))
            .ToListAsync(cancellationToken);
}
