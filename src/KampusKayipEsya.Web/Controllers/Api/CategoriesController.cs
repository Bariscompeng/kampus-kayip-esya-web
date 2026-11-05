using KampusKayipEsya.Web.Data;
using KampusKayipEsya.Web.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace KampusKayipEsya.Web.Controllers.Api;

[Route("api/categories")]
[AllowAnonymous]
public class CategoriesController(AppDbContext context) : ApiControllerBase
{
    [HttpGet]
    public async Task<ActionResult<List<CategoryDto>>> GetAll(CancellationToken cancellationToken) =>
        await context.Categories
            .AsNoTracking()
            .OrderBy(c => c.Name)
            .Select(c => new CategoryDto(c.Id, c.Name, c.Icon))
            .ToListAsync(cancellationToken);
}
