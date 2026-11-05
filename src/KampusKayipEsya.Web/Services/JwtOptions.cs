namespace KampusKayipEsya.Web.Services;

public class JwtOptions
{
    public const int MinKeyBytes = 32;

    // Yalnızca user-secrets'ta; depoya girmez.
    public string Key { get; set; } = string.Empty;
    public string Issuer { get; set; } = string.Empty;
    public string Audience { get; set; } = string.Empty;
    public int ExpiresMinutes { get; set; } = 1440;
}
