using KampusKayipEsya.Web.Dtos;
using KampusKayipEsya.Web.Models;
using KampusKayipEsya.Web.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace KampusKayipEsya.Web.Controllers.Api;

[Route("api/items")]
[Tags("Items")]
public class ItemsApiController(IItemService itemService) : ApiControllerBase
{
    [HttpGet]
    [AllowAnonymous]
    public async Task<ActionResult<PagedResponse<ItemListDto>>> Search([FromQuery] ItemSearchRequest request, CancellationToken cancellationToken)
    {
        var result = await itemService.SearchAsync(request, cancellationToken);
        var items = result.Items.Select(i => i.ToListDto(PhotoUrl)).ToList();
        return new PagedResponse<ItemListDto>(items, result.Page, result.PageSize, result.TotalCount);
    }

    [HttpGet("{id:int}")]
    [AllowAnonymous]
    public async Task<ActionResult<ItemDetailDto>> Get(int id, CancellationToken cancellationToken)
    {
        var viewer = ItemViewer.From(User);
        var item = await itemService.GetVisibleAsync(id, viewer, cancellationToken);
        if (item is null) return NotFound();

        return item.ToDetailDto(viewer, PhotoUrl);
    }

    [HttpPost]
    public async Task<ActionResult<ItemDetailDto>> Create(ItemWriteRequest request, CancellationToken cancellationToken)
    {
        var input = request.ToInput();
        if (await ReferencesInvalidAsync(input, cancellationToken)) return ValidationProblem(ModelState);

        var viewer = ItemViewer.From(User);
        var created = await itemService.CreateAsync(input, viewer.UserId!, cancellationToken: cancellationToken);
        var item = await itemService.GetVisibleAsync(created.Id, viewer, cancellationToken);

        return CreatedAtAction(nameof(Get), new { id = created.Id }, item!.ToDetailDto(viewer, PhotoUrl));
    }

    [HttpPut("{id:int}")]
    public async Task<ActionResult<ItemDetailDto>> Update(int id, ItemWriteRequest request, CancellationToken cancellationToken)
    {
        var input = request.ToInput();
        if (await ReferencesInvalidAsync(input, cancellationToken)) return ValidationProblem(ModelState);

        var viewer = ItemViewer.From(User);
        var result = await itemService.UpdateAsync(id, input, viewer, cancellationToken);
        if (result != ItemChangeResult.Success) return ToErrorResult(result);

        var item = await itemService.GetVisibleAsync(id, viewer, cancellationToken);
        return item!.ToDetailDto(viewer, PhotoUrl);
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken cancellationToken)
    {
        var result = await itemService.DeleteAsync(id, ItemViewer.From(User), cancellationToken);
        return result == ItemChangeResult.Success ? NoContent() : ToErrorResult(result);
    }

    private async Task<bool> ReferencesInvalidAsync(ItemInput input, CancellationToken cancellationToken)
    {
        var errors = await itemService.ValidateReferencesAsync(input.CategoryId, input.LocationId, cancellationToken);
        foreach (var (field, message) in errors)
            ModelState.AddModelError(field, message);
        return errors.Count > 0;
    }

    private ActionResult ToErrorResult(ItemChangeResult result) => result switch
    {
        ItemChangeResult.NotFound => NotFound(),
        ItemChangeResult.Forbidden => Problem(statusCode: StatusCodes.Status403Forbidden, title: "Bu ilanı yalnızca sahibi ya da görevli değiştirebilir."),
        _ => Problem(statusCode: StatusCodes.Status409Conflict, title: "Teslim edilmiş ya da arşivlenmiş ilan değiştirilemez.")
    };

    private string PhotoUrl(string path) => $"{Request.Scheme}://{Request.Host}{Request.PathBase}/{path}";
}
