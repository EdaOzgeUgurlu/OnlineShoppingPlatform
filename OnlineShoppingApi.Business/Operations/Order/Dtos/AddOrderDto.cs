using OnlineShopping.Data.Entities;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Business.Operations.Order.Dtos
{
    public class AddOrderDto
    {
        public int UserId { get; set; }

        public DateTime OrderDate { get; set; }
        public int TotalAmount { get; set; }

        public List<int> ProductIds { get; set; } 
    }
}
