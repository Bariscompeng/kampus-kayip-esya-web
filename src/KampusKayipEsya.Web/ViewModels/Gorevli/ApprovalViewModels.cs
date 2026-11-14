using System.ComponentModel.DataAnnotations;
using KampusKayipEsya.Web.Dtos;
using KampusKayipEsya.Web.Models;

namespace KampusKayipEsya.Web.ViewModels.Gorevli;

public record ApprovalQueueViewModel(PagedResponse<Item> Bekleyenler)
{
    public int SayfaSayisi => Math.Max(1, (int)Math.Ceiling(Bekleyenler.TotalCount / (double)Bekleyenler.PageSize));

    public static string BeklemeSuresi(DateTime olusturma)
    {
        var sure = DateTime.UtcNow - olusturma;
        return sure.TotalHours switch
        {
            < 1 => "1 saatten az",
            < 24 => $"{(int)sure.TotalHours} saat",
            _ => $"{(int)sure.TotalDays} gün"
        };
    }
}

public class RejectItemViewModel
{
    public const int SebepEnAz = 5;
    public const int SebepEnFazla = 500;

    [Required(ErrorMessage = "Ret sebebini yazın.")]
    [StringLength(SebepEnFazla, MinimumLength = SebepEnAz, ErrorMessage = "Ret sebebi {2}–{1} karakter olmalıdır.")]
    [Display(Name = "Ret sebebi")]
    public string Reason { get; set; } = string.Empty;
}
