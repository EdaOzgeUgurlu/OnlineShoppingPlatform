using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace OnlineShopping.Data.Entities
{
    public interface IOrderProductConfiguration
    {
        void Configure(EntityTypeBuilder<OrderProductEntity> builder);
    }
}