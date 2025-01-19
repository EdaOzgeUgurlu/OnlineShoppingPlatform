using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Business.Types
{
    // Basit bir servis mesajı sınıfı, işlem sonucunu ve mesajı tutar
    public class ServiceMessage
    {
        public bool IsSucceed { get; set; } // İşlemin başarılı olup olmadığını belirten bool değer
        public string Message { get; set; } = ""; // İşlemle ilgili mesaj, varsayılan olarak boş bir string
    }

    // Parametreli bir servis mesajı sınıfı, işlem sonucunu, mesajı ve veri tutar
    public class ServiceMessage<T>
    {
        public bool IsSucceed { get; set; } // İşlemin başarılı olup olmadığını belirten bool değer
        public string Message { get; set; } = ""; // İşlemle ilgili mesaj, varsayılan olarak boş bir string
        public T? Data { get; set; } // İşlemle ilgili veri, tür parametresi T ile belirlenir (nullable)
    }
}

