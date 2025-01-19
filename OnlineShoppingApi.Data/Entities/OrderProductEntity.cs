using Microsoft.EntityFrameworkCore.Metadata.Builders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Data.Entities
{
    public class OrderProductEntity : BaseEntity
    {
        public int OrderId { get; set; }
        public OrderEntity Order { get; set; }  // ÇOKA ÇOK BAĞLNTI 

        //BİR KULLANICI BİR ÇOK PRODUCT ALABİLİR
        //BİR PRODUCT BİR ÇOK ORDERIN SEPETİNDE OLABİLİR

        public int ProductId { get; set; }
        public ProductEntity Product { get; set; } //ÇOKA ÇOK BAĞLNTI 

        public int Quantity { get; set; }
    }

    public class OrderProductConfiguration : BaseConfiguration<OrderProductEntity>
    {
        public override void Configure(EntityTypeBuilder<OrderProductEntity> builder)
        {
            builder.Ignore(x => x.Id);
            builder.HasKey("OrderId", "ProductId");
        
            base.Configure(builder);
        }
    }
}
