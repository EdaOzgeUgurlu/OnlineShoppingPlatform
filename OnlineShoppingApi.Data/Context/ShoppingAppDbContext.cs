using Microsoft.EntityFrameworkCore;
using OnlineShopping.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Data.Context
{
    // Uygulamanın DbContext sınıfı, Entity Framework Core tarafından kullanılan veri erişim sınıfıdır.
    public class ShoppingAppDbContext : DbContext
    {
        // DbContextOptions ile konfigürasyon sağlanır
        public ShoppingAppDbContext(DbContextOptions<ShoppingAppDbContext> options) : base(options)
        {
        }

        // Model oluşturulurken yapılacak konfigürasyonlar burada tanımlanır
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            // Fluent API kullanılarak model konfigürasyonları yapılır
            modelBuilder.ApplyConfiguration(new OrderConfiguration()); // OrderEntity için yapılandırma
            modelBuilder.ApplyConfiguration(new OrderProductConfiguration()); // OrderProductEntity için yapılandırma
            modelBuilder.ApplyConfiguration(new ProductConfiguration()); // ProductEntity için yapılandırma
            modelBuilder.ApplyConfiguration(new UserConfiguration()); // UserEntity için yapılandırma

            // Diğer tüm yapılandırmalar için temel metodu çağırır
            base.OnModelCreating(modelBuilder);
        }

        // Veritabanı tablosu için DbSet tanımları
        public DbSet<UserEntity> Users => Set<UserEntity>(); // Kullanıcılar tablosu
        public DbSet<ProductEntity> Products => Set<ProductEntity>(); // Ürünler tablosu
        public DbSet<OrderEntity> Orders => Set<OrderEntity>(); // Siparişler tablosu
        public DbSet<OrderProductEntity> OrderProducts => Set<OrderProductEntity>(); // Sipariş ürünleri tablosu
        public DbSet<SettingEntity> Settings => Set<SettingEntity>(); // Ayarlar tablosu
    }
}
