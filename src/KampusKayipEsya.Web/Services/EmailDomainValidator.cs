namespace KampusKayipEsya.Web.Services;

public class EmailDomainValidator : IEmailDomainValidator
{
    public IReadOnlyList<string> AllowedDomains { get; }

    public EmailDomainValidator(IEnumerable<string> domains)
    {
        AllowedDomains = domains
            .Select(d => d.Trim().TrimStart('@').ToLowerInvariant())
            .Where(d => d.Length > 0)
            .ToList();
    }

    // "ankara.edu.tr" izinliyse "ogrenci.ankara.edu.tr" de kabul edilir; "xankara.edu.tr" edilmez.
    public bool IsAllowed(string? email)
    {
        if (string.IsNullOrWhiteSpace(email)) return false;

        var at = email.LastIndexOf('@');
        if (at <= 0 || at == email.Length - 1) return false;

        var domain = email[(at + 1)..].Trim().ToLowerInvariant();
        return AllowedDomains.Any(d => domain == d || domain.EndsWith("." + d));
    }
}
