using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using OnlineShopping.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Data.Entities
{
    public class BaseEntity //miras verecek class
    {
        public int Id { get; set; } 
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
        public DateTime? ModifiedDate { get; set; } 
        public bool IsDeleted { get; set; } = false; 
    }
}
//Yeni ortak özellikler eklenmesi gerektiğinde, sadece BaseEntity'ye eklemek yeterlidir.
//Bu sınıf, her tablo için ortak olan özellikleri tanımlar ve tüm Entity'ler bu sınıftan türetilir.

public abstract class BaseConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
{
    public virtual void Configure(EntityTypeBuilder<TEntity> builder)
    {
        builder.HasQueryFilter(x => x.IsDeleted == false);
        //bu veritabanı üzerinde yapılacak bütün sorgulamalarda ve diğer linq işlemlerinde geçerli olacak bir filtreleme yazdık böylelikle hiçbir zaman bir daha soft atılmış verilerle uğraşmayacağız.

        builder.Property(x => x.ModifiedDate).IsRequired(false);

    }
}
