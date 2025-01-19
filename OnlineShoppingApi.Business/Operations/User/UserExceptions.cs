using OnlineShopping.Business.Operations.Order.Dtos;
using OnlineShopping.Data.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Business.Operations.User
{
    // Kullanıcı işlemleri sırasında oluşacak hatalar için özel istisna sınıfı
    public class UserException : Exception
    {
        public UserException(string message) : base(message) { } // Hata mesajını alarak istisna oluşturur
    }

    // Geçersiz giriş bilgileri için özel istisna sınıfı
    public class InvalidCredentialsException : Exception
    {
        public InvalidCredentialsException(string message) : base(message) { } // Hata mesajını alarak istisna oluşturur
    }
}
