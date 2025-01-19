using OnlineShopping.Business.Operations.Order.Dtos;
using OnlineShopping.Data.Enums;

namespace OnlineShopping.Business.Operations.User.Dtos
{
    public class UserInfoDto
    {
        public int Id { get; set; }
        public string Email { get; set; } = "";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public UserType UserType { get; set; }
        public List<OrderProductDto> Orders { get; set; }
    }
}