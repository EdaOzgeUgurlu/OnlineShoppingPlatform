using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Api.Models
{
    public class UpdateProductRequest
    {
        [Required]
        public string ProductName { get; set; } = "";
        public decimal Price { get; set; }
        [Required]
        public int StockQuantity { get; set; }
    }
}
