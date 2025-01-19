using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Api.Models
{
    public class UpdateUserRequest
    {
        [Required]
        [MaxLength(50)]
        public string FirstName { get; set; } = "";
        [Required]
        [MaxLength(50)]
        public string LastName { get; set; } = "";
        [Required]
        [MaxLength(50)]
        public string PhoneNumber { get; set; } = "";

    }
}
