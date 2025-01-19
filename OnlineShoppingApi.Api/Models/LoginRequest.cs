using System.ComponentModel.DataAnnotations;

namespace OnlineShopping.Api.Models
{
    public class LoginRequest
    {
        [Required]
        [EmailAddress]//Emailaddress formatında
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}
