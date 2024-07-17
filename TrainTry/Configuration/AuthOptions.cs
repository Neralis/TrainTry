using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace TrainTry.Configuration
{
    public class AuthOptions
    {
        public const string ISSUER = "AuthServer"; // издатель токена
        public const string AUDIENCE = "AuthService"; // потребитель токена
        const string KEY = "HeadCrackerMain_TryNum_90110131212";   // ключ для шифрации
        public static SymmetricSecurityKey GetSymmetricSecurityKey() =>
            new SymmetricSecurityKey(Encoding.UTF8.GetBytes(KEY));
    }
}
