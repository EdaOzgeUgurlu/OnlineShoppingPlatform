using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Business.Operations.Order.Dtos
{
    public class OrderDto
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; } = "";
        public DateTime OrderDate { get; set; }
        public int TotalAmount { get; set; }
        public List<OrderProductDto> Products { get; set; } = new List<OrderProductDto>();
    }
}
