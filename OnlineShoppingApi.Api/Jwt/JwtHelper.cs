using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace OnlineShopping.Api.Jwt
{
    public static class JwtHelper
    {
        public static string GenerateJwtToken(JwtDto jwtInfo)
        {
            // Gizli anahtarı oluşturuyoruz. JWT'nin güvenliği için kullanılacak.
            var secretKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtInfo.SecretKey));

            // Kimlik doğrulama için gerekli olan SigningCredentials'i oluşturuyoruz.
            // HmacSha256 algoritması ile gizli anahtarımızı kullanarak imza oluşturuyoruz.
            var credentials = new SigningCredentials(secretKey, SecurityAlgorithms.HmacSha256);

            // JWT'ye ekleyeceğimiz id, isim, soyisim, email ve kullanıcı türü gibi claim'leri tanımlıyoruz.
            var claims = new[]
            {
        // Kullanıcı bilgilerini claim olarak ekliyoruz
        new Claim(JwtClaimNames.Id, jwtInfo.Id.ToString()),          // Kullanıcı id'si
        new Claim(JwtClaimNames.FirstName, jwtInfo.FirstName),      // Kullanıcının adı
        new Claim(JwtClaimNames.LastName, jwtInfo.LastName),        // Kullanıcının soyadı
        new Claim(JwtClaimNames.Email, jwtInfo.Email),              // Kullanıcı email adresi
        new Claim(JwtClaimNames.UserType, jwtInfo.UserType.ToString()),  // Kullanıcı türü (Admin, User vb.)

        // Kullanıcı yetkisi için role claim'i ekliyoruz
        new Claim(ClaimTypes.Role, jwtInfo.UserType.ToString())     // Kullanıcının rolü
    };

            // Token'ın geçerlilik süresini ayarlıyoruz. Burada 'ExpireMinutes' parametresiyle belirliyoruz.
            var expireTime = DateTime.Now.AddMinutes(jwtInfo.ExpireMinutes);

            // JWT'nin oluşturulması için gerekli tüm parametreleri (issuer, audience, claims vb.) içeren bir JwtSecurityToken nesnesi oluşturuyoruz.
            var tokenDescriptor = new JwtSecurityToken(jwtInfo.Issuer, jwtInfo.Audience, claims, null, expireTime, credentials);

            // JWT token'ını oluşturup string formatında geri döndürüyoruz.
            var token = new JwtSecurityTokenHandler().WriteToken(tokenDescriptor);

            return token;  // Oluşturulan JWT token'ını döndürüyoruz.
        }

    }
}
