using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Api.Models
{
    public class UpdateOrderRequest
    {
        [Required]
        public int UserId { get; set; }

        public DateTime OrderDate { get; set; }
        [Required]
        public int TotalAmount { get; set; }
        [Required]
        public List<int> ProductIds { get; set; } = new List<int>();
    }
}
