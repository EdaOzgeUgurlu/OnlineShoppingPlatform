using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Data.Entities
{
    public class ProductEntity : BaseEntity
    {
        public string ProductName { get; set; }
        public decimal Price { get; set; }
        public int StockQuantity { get; set; }

        //Relational property
        //çoka çok ilişkiyi alıyoruz
        public ICollection<OrderProductEntity> OrderProducts { get; set; } //çoklu bağlantı yapıldı
    }

    public class ProductConfiguration : BaseConfiguration<ProductEntity>
    {

        public override void Configure(EntityTypeBuilder<ProductEntity> builder)
        {
            //Price property configuration
            builder.Property(p => p.Price)
                   .HasColumnType("decimal(18,2)")
                   .IsRequired();

            base.Configure(builder);
        }

    }
}
