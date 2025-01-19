using Microsoft.EntityFrameworkCore;
using OnlineShopping.Data.Context;
using OnlineShopping.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Data.Repositories
{
    // Genel CRUD işlemlerini gerçekleştiren Repository sınıfı
    public class Repository<TEntity> : IRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly ShoppingAppDbContext _db;
        private readonly DbSet<TEntity> _dbSet;

        // Constructor, DbContext'e ve DbSet'e erişim sağlamak için kullanılır
        public Repository(ShoppingAppDbContext db)
        {
            _db = db;
            _dbSet = db.Set<TEntity>();
        }

        // Yeni bir entity ekler ve oluşturulma tarihini atar
        public void Add(TEntity entity)
        {
            entity.CreatedDate = DateTime.Now; // Entity'nin oluşturulma tarihi atanır
            _dbSet.Add(entity); // Entity eklenir
                                // Veritabanı işlemleri burada yapılabilir, ancak burada yorumda bırakılmış.
        }

        // Entity'yi siler. Soft delete işlemi yapılabilir.
        public void Delete(TEntity entity, bool softDelete = true)
        {
            if (softDelete)
            {
                // Soft delete, yani mantıksal silme
                entity.ModifiedDate = DateTime.Now; // Güncellenme tarihi atanır
                entity.IsDeleted = true; // Silindi olarak işaretlenir
                _dbSet.Update(entity); // Güncellenmiş entity veritabanına yansıtılır
                                       // Veritabanı işlemleri burada yapılabilir, ancak burada yorumda bırakılmış.
            }
            else
            {
                // Hard delete, yani veritabanından tamamen silme
                _dbSet.Remove(entity); // Entity veritabanından tamamen kaldırılır
            }
        }

        // Belirtilen id'ye göre entity silinir
        public void Delete(int id)
        {
            var entity = _dbSet.Find(id); // Id'ye göre entity bulunur
            Delete(entity); // Entity silinir
        }

        // Veritabanından, belirli bir koşula (predicate) uyan ilk entity'yi getirir
        public TEntity Get(Expression<Func<TEntity, bool>> predicate)
        {
            return _dbSet.FirstOrDefault(predicate); // Predicate ile eşleşen ilk entity döndürülür
        }

        // Veritabanındaki tüm entity'leri getirir. Koşul eklenebilir.
        public IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate = null)
        {
            return predicate is null ? _dbSet : _dbSet.Where(predicate); // Predicate varsa filtreleme yapılır
        }

        // Belirli bir id'ye sahip entity'yi getirir
        public TEntity GetById(int id)
        {
            return _dbSet.Find(id); // Id'ye göre entity bulunur
        }

        // Entity'yi günceller ve güncellenme tarihini atar
        public void Update(TEntity entity)
        {
            entity.ModifiedDate = DateTime.Now; // Güncellenme tarihi atanır
            _dbSet.Update(entity); // Entity güncellenir
                                   // Veritabanı işlemleri burada yapılabilir, ancak burada yorumda bırakılmış.
        }
    }
}