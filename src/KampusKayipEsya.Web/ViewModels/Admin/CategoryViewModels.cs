using System.ComponentModel.DataAnnotations;

namespace KampusKayipEsya.Web.ViewModels.Admin;

public record CategoryListItem(int Id, string Name, string? Icon, int ItemCount);

public class CategoryFormViewModel
{
    public int Id { get; set; }

    [Required(ErrorMessage = "Kategori adı zorunludur.")]
    [StringLength(50, MinimumLength = 2, ErrorMessage = "Kategori adı {2}–{1} karakter olmalıdır.")]
    [Display(Name = "Kategori adı")]
    public string Name { get; set; } = string.Empty;

    [StringLength(50, ErrorMessage = "Simge adı en fazla {1} karakter olabilir.")]
    [RegularExpression(@"^bi-[a-z0-9-]+$", ErrorMessage = "Simge adı 'bi-' ile başlamalı ve küçük harf, rakam, tire içermelidir (ör. bi-phone).")]
    [Display(Name = "Simge (Bootstrap Icons)")]
    public string? Icon { get; set; }
}
