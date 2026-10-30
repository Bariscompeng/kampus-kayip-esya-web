namespace KampusKayipEsya.Web.Services;

// Kayıt yalnızca okul e-postasıyla yapılır. MVC kaydı ve /api/auth/register aynı kontrolü kullanır.
public interface IEmailDomainValidator
{
    bool IsAllowed(string? email);
    IReadOnlyList<string> AllowedDomains { get; }
}
