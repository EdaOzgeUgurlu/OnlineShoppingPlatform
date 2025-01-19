using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Business.Operations.Order
{
    // Sipariş ve ürünle ilgili özel istisna sınıfları.
    public class OrderException : Exception
    {
        public OrderException(string message) : base(message) { }
    }
    public class OrderProductException : Exception
    {
        public OrderProductException(string message) : base(message) { }
    }
    public class OrderDeleteException : Exception
    {
        public OrderDeleteException(string message) : base(message) { }
    }
}
