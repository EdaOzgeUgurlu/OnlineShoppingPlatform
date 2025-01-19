using Microsoft.AspNetCore.DataProtection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Business.DataProtection
{
    public class DataProtection : IDataProtection
    {
        // IDataProtector nesnesi, verileri şifrelemek ve şifreyi çözmek için kullanılır.
        private readonly IDataProtector _protector;

        // Constructor, IDataProtectionProvider parametresi alır ve IDataProtector nesnesi oluşturur.
        // Bu nesne, veriyi korumak (şifrelemek) ve korumayı kaldırmak (şifreyi çözmek) için kullanılacak.
        public DataProtection(IDataProtectionProvider provider)
        {
            // 'Security' adında bir koruyucu oluşturuluyor. Bu, verinin şifrelenmesinde kullanılacak.
            _protector = provider.CreateProtector("Security");
        }

        // Veriyi korur (şifreler).
        // Bu metod, dışarıdan gelen düz metni alır ve şifreler, böylece güvenli bir şekilde saklanmasını sağlar.
        public string Protect(string text)
        {
            return _protector.Protect(text);  // Veriyi şifreler ve şifreli metni döndürür.
        }

        // Korumayı kaldırır (şifreyi çözer).
        // Bu metod, şifrelenmiş metni alır ve orijinal metni geri döndürür.
        public string UnProtect(string protectedText)
        {
            return _protector.Unprotect(protectedText);  // Şifreyi çözer ve orijinal metni döndürür.
        }
    }
}
