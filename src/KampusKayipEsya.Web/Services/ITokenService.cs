using KampusKayipEsya.Web.Models;

namespace KampusKayipEsya.Web.Services;

public interface ITokenService
{
    (string Token, DateTime ExpiresAt) CreateToken(AppUser user, IEnumerable<string> roles);
}
