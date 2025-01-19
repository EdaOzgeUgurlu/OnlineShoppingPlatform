using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Business.Operations.Product
{
    public class ProductException : Exception
    {
        public ProductException(string message) : base(message) { } // Hata mesajını alarak istisna oluşturur
    }

}
