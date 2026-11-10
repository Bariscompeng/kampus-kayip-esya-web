using System.ComponentModel.DataAnnotations;
using KampusKayipEsya.Web.Models.Enums;

namespace KampusKayipEsya.Web.Dtos;

public class OwnedItemsRequest
{
    public ItemStatus? Status { get; set; }

    [Range(1, int.MaxValue, ErrorMessage = "Sayfa numarası en az 1 olmalıdır.")]
    public int Page { get; set; } = 1;

    [Range(1, 50, ErrorMessage = "Sayfa boyutu {1}–{2} arasında olmalıdır.")]
    public int PageSize { get; set; } = 20;
}
