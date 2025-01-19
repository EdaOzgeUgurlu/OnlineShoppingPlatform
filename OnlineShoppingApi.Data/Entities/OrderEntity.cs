using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Data.Entities
{
    public class OrderEntity : BaseEntity
    {
        
        public DateTime OrderDate { get; set; }
        public int TotalAmount { get; set; }
        public int UserId { get; set; } //siparişi veren ile ilişkilendirlmesi


        //Relational property
        //çoka çok ilişkiyi alıyoruz
        public UserEntity User { get; set; }
        public ICollection<OrderProductEntity> OrderProducts { get; set; } //çoklu bağlantı yapıldı
    }
    public class OrderConfiguration : BaseConfiguration<OrderEntity>
    {
        public override void Configure(EntityTypeBuilder<OrderEntity> builder)
        {
            builder.Property(x => x.UserId).IsRequired();

            base.Configure(builder);
        }

    }
}