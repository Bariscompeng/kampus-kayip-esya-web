using System.ComponentModel.DataAnnotations;

namespace KampusKayipEsya.Web.Dtos;

public class RegisterRequest
{
    [Required(ErrorMessage = "Ad soyad zorunludur.")]
    [StringLength(100, MinimumLength = 3, ErrorMessage = "Ad soyad {2}–{1} karakter olmalıdır.")]
    [RegularExpression(@"^[a-zA-ZçğıöşüÇĞİÖŞÜâîûÂÎÛ\s.'-]+$", ErrorMessage = "Ad soyad yalnızca harf içermelidir.")]
    public string FullName { get; set; } = string.Empty;

    [RegularExpression(@"^\d{8}$", ErrorMessage = "Öğrenci numarası 8 haneli olmalı ve yalnızca rakam içermelidir.")]
    public string? StudentNo { get; set; }

    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    [StringLength(256, ErrorMessage = "E-posta en fazla {1} karakter olabilir.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [StringLength(100, MinimumLength = 8, ErrorMessage = "Şifre en az {2}, en fazla {1} karakter olmalıdır.")]
    public string Password { get; set; } = string.Empty;
}

public class LoginRequest
{
    [Required(ErrorMessage = "E-posta zorunludur.")]
    [EmailAddress(ErrorMessage = "Geçerli bir e-posta adresi girin.")]
    [StringLength(256, ErrorMessage = "E-posta en fazla {1} karakter olabilir.")]
    public string Email { get; set; } = string.Empty;

    [Required(ErrorMessage = "Şifre zorunludur.")]
    [StringLength(100, ErrorMessage = "Şifre en fazla {1} karakter olabilir.")]
    public string Password { get; set; } = string.Empty;
}

public record UserDto(string Id, string Email, string FullName, string? StudentNo, IReadOnlyList<string> Roles);

public record AuthResponse(string Token, DateTime ExpiresAt, UserDto User);
