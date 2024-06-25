using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace FilmRaterMain.Controllers
{
    public class AuthOptions
    {
        public const string ISSUER = "FilmRaterIssuer";
        public const string AUDIENCE = "FilmRaterAudience";
        const string KEY = "secret_auth_key_longer_than_32_@!192837";
        public static SymmetricSecurityKey GetSymmetricSecurityKey() => new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
