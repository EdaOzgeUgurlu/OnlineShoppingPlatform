using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Data.UnitOfWork
{
    public interface IUnitOfWork : IDisposable
    {
        // Veritabanı değişikliklerini asenkron olarak kaydeder
        // Kaydedilen kayıt sayısını geri döner
        Task<int> SaveChangesAsync();

        // Yeni bir işlem başlatır. Bu, veritabanı değişikliklerinin topluca yönetilmesi için kullanılır.
        // İşlem (transaction) süreci başlatılır.
        Task BeginTransaction();

        // İşlemdeki tüm değişiklikleri veritabanına kaydeder.
        // Commit, yapılan işlemleri kalıcı hale getirir.
        Task CommitTransaction();

        // İşlemdeki değişiklikleri geri alır.
        // Rollback, işlem sırasında yapılan değişikliklerin tamamını geri alır ve veritabanını önceki durumuna getirir.
        Task RollBackTransaction();


    }
}
