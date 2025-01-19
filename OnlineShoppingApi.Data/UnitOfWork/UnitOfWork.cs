using Microsoft.EntityFrameworkCore.Storage;
using OnlineShopping.Data.Context;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Data.UnitOfWork
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ShoppingAppDbContext _db;
        private IDbContextTransaction _transaction;

        // UnitOfWork sınıfı, veritabanı işlemleri için kullanılan bağlamı alır ve transaction yönetimini sağlar.
        public UnitOfWork(ShoppingAppDbContext db)
        {
            _db = db;
        }

        // Veritabanında yeni bir işlem başlatır. 
        // Bu işlem, birden fazla değişikliği bir arada ve güvenli bir şekilde kaydetmeyi sağlar.
        public async Task BeginTransaction()
        {
            _transaction = await _db.Database.BeginTransactionAsync();
        }

        // Başlatılan işlemi kalıcı hale getirir ve yapılan tüm değişiklikleri veritabanına kaydeder.
        // Commit işleminden sonra yapılan değişiklikler geri alınamaz.
        public async Task CommitTransaction()
        {
            await _transaction.CommitAsync();
        }

        // UnitOfWork nesnesi kullanıldığında, DbContext'in kaynakları serbest bırakılır.
        public void Dispose()
        {
            _db.Dispose();
        }

        // Başlatılan işlemdeki değişiklikleri geri alır. 
        // Rollback, işlemi iptal eder ve veritabanını önceki haline döndürür.
        public async Task RollBackTransaction()
        {
            await _transaction.RollbackAsync();
        }

        // Veritabanı üzerindeki değişiklikleri asenkron olarak kaydeder ve etkilenen kayıt sayısını geri döner.
        public async Task<int> SaveChangesAsync()
        {
            return await _db.SaveChangesAsync();
        }
    }
}
