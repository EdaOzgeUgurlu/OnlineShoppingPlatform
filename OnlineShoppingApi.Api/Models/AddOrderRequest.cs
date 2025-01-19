using OnlineShopping.Data.Entities;
using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Api.Models
{
    public class AddOrderRequest
    {
        [Required]
        public DateTime OrderDate { get; set; }
        [Required]
        public int TotalAmount { get; set; }
        [Required]
        public int UserId { get; set; }
   
        public List<int> ProductIds { get; set; } = new List<int>();
    }

}
