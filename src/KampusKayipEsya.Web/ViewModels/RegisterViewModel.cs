using System.ComponentModel.DataAnnotations;

namespace KampusKayipEsya.Web.ViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Ad soyad zorunludur.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Ad soyad {2}–{1} karakter olmalıdır.")]
    [RegularExpression(@"^[a-zA-ZçğıöşüÇĞİÖŞÜâîûÂÎÛ\s.'-]+$", ErrorMessage = "Ad soyad yalnızca harf içermelidir.")]
    [Display(Name = "Ad Soyad")]
    public string FullName { get; set; } = string.Empty;

    [RegularExpression(@"^\d{8}$", ErrorMessage = "Öğrenci numarası 8 haneli olmalı ve yalnızca rakam içermelidir.")]
    [Display(Name = "Öğrenci No")]
    public string? StudentNo { get; set; }

    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    [StringLength(256, ErrorMessage = "E-posta en fazla {1} karakter olabilir.")]
    [Display(Name = "Okul E-postası")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Şifre en az {2}, en fazla {1} karakter olmalıdır.")]
    [DataType(DataType.Password)]
    [Display(Name = "Şifre")]
    public string Password { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre tekrarı zorunludur.")]
    [DataType(DataType.Password)]
    [Compare(nameof(Password), ErrorMessage = "Şifreler eşleşmiyor.")]
    [Display(Name = "Şifre (tekrar)")]
    public string ConfirmPassword { get; set; } = string.Empty;
}
