using System.ComponentModel.DataAnnotations;
using System.Globalization;

namespace KampusKayipEsya.Web.ViewModels.Admin;

public record LocationListItem(int Id, string Building, string? FloorOrRoom, double? Latitude, double? Longitude, int ItemCount)
{
    public string? Koordinat => Latitude is null || Longitude is null
        ? null
        : string.Create(CultureInfo.InvariantCulture, $"{Latitude:0.######}, {Longitude:0.######}");
}

public class LocationFormViewModel : IValidatableObject
{
    private const string KoordinatDeseni = @"^-?\d{1,3}([.,]\d{1,8})?$";

    public int Id { get; set; }

    [Required(ErrorMessage = "Bina adı zorunludur.")]
    [StringLength(100, MinimumLength = 2, ErrorMessage = "Bina adı {2}–{1} karakter olmalıdır.")]
    [Display(Name = "Bina")]
    public string Building { get; set; } = string.Empty;

    [StringLength(100, ErrorMessage = "Kat / derslik en fazla {1} karakter olabilir.")]
    [Display(Name = "Kat / derslik")]
    public string? FloorOrRoom { get; set; }

    [RegularExpression(KoordinatDeseni, ErrorMessage = "Enlem sayı olmalıdır (ör. 39.9334).")]
    [Display(Name = "Enlem")]
    public string? Latitude { get; set; }

    [RegularExpression(KoordinatDeseni, ErrorMessage = "Boylam sayı olmalıdır (ör. 32.8597).")]
    [Display(Name = "Boylam")]
    public string? Longitude { get; set; }

    public double? EnlemSayisi => SayiyaCevir(Latitude);
    public double? BoylamSayisi => SayiyaCevir(Longitude);

    public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
    {
        var enlemVar = !string.IsNullOrWhiteSpace(Latitude);
        var boylamVar = !string.IsNullOrWhiteSpace(Longitude);

        if (enlemVar != boylamVar)
            yield return new ValidationResult("Enlem ve boylam birlikte girilmeli ya da ikisi de boş bırakılmalıdır.",
                [enlemVar ? nameof(Longitude) : nameof(Latitude)]);

        if (EnlemSayisi is < -90 or > 90)
            yield return new ValidationResult("Enlem -90 ile 90 arasında olmalıdır.", [nameof(Latitude)]);

        if (BoylamSayisi is < -180 or > 180)
            yield return new ValidationResult("Boylam -180 ile 180 arasında olmalıdır.", [nameof(Longitude)]);
    }

    public static string? MetneCevir(double? sayi) =>
        sayi?.ToString("0.########", CultureInfo.InvariantCulture);

    private static double? SayiyaCevir(string? metin) =>
        double.TryParse(metin?.Trim().Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out var sayi)
            ? sayi
            : null;
}
