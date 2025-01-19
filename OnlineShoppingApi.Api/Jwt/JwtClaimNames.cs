namespace OnlineShopping.Api.Jwt
{
    public class JwtClaimNames
    {
        // Kullanıcının eşsiz ID'sini temsil eden claim ismi.
        public const string Id = "Id";

        // Kullanıcının adı için kullanılan claim ismi.
        public const string FirstName = "FisrtName"; // Not: "FisrtName" yazım hatası olabilir, "FirstName" olmalı.

        // Kullanıcının soyadı için kullanılan claim ismi.
        public const string LastName = "LastName";

        // Kullanıcının e-posta adresi için kullanılan claim ismi.
        public const string Email = "Email";

        // Kullanıcının türünü (örneğin Admin veya User) temsil eden claim ismi.
        public const string UserType = "UserType";
    }
}
