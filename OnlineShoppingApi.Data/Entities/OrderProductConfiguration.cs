using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineShopping.Data.Entities
{
    public class OrderProductConfiguration : BaseConfiguration<OrderProductEntity>
    {
        public override void Configure(EntityTypeBuilder<OrderProductEntity> builder)
        {
            base.Configure(builder);
        }
    }
}
