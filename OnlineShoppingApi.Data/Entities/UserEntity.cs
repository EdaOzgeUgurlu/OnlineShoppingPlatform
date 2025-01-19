using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineShopping.Data.Enums;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Data.Entities
{
    public class UserEntity : BaseEntity //yetki işlemlerinde kullanırız
    {
        public string Email { get; set; } = " ";
        public string FirstName { get; set; } = "";
        public string LastName { get; set; } = "";
        public string Password { get; set; } = "";
        public string PhoneNumber { get; set; } = "";
        //user'a eklememiz gereken bir yetki durumu var 
        //user admin de olabilir kullanıcı da olabilir
        //bu iki seçeneği sunabileceğimiz bir enum oluşturuyoruz

        //Enum oluşturduktan sonra enumda yapılan user'ın admin ve customer olma durumunu property olarak ekliyoruz.
        public UserType UserType { get; set; }
    }

    public class UserConfiguration : BaseConfiguration<UserEntity>
    {
        public override void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.Property(e => e.FirstName)
                .IsRequired();

            builder.Property(e => e.LastName)
               .IsRequired();

            builder.Property(e => e.PhoneNumber)
               .IsRequired();

            base.Configure(builder);
        }
    }
}
