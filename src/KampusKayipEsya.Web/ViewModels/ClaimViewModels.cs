using System.ComponentModel.DataAnnotations;
using KampusKayipEsya.Web.Models;
using KampusKayipEsya.Web.Models.Enums;
using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;

namespace KampusKayipEsya.Web.ViewModels;

public class ClaimCreateViewModel
{
    public const int CevapEnAz = 2;
    public const int CevapEnFazla = 300;
    public const int MesajEnFazla = 500;

    [Required(ErrorMessage = "Doğrulama sorusunun cevabını yazın.")]
    [StringLength(CevapEnFazla, MinimumLength = CevapEnAz, ErrorMessage = "Cevap {2}–{1} karakter olmalıdır.")]
    [Display(Name = "Cevabınız")]
    public string VerificationAnswer { get; set; } = string.Empty;

    [StringLength(MesajEnFazla, ErrorMessage = "Not en fazla {1} karakter olabilir.")]
    [Display(Name = "İlan sahibine not (isteğe bağlı)")]
    public string? Message { get; set; }

    [ValidateNever]
    public Item Item { get; set; } = null!;
}

public class IncomingClaimsViewModel(IReadOnlyList<ItemClaim> claims)
{
    public IReadOnlyList<ItemClaim> Bekleyenler { get; } = claims.Where(c => c.Status == ClaimStatus.Pending).ToList();
    public IReadOnlyList<ItemClaim> Sonuclananlar { get; } = claims.Where(c => c.Status != ClaimStatus.Pending).ToList();
}
