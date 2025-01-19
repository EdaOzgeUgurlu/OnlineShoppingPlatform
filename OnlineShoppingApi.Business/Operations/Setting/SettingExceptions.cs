using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Business.Operations.Setting
{
    public class SettingException : Exception
    {
        public SettingException(string message) : base(message) { } // Hata mesajını alarak istisna oluşturur
    }
}
