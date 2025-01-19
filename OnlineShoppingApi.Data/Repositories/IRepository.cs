using Microsoft.EntityFrameworkCore.Metadata.Conventions;
using OnlineShopping.Data.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OnlineShopping.Data.Repositories
{

    public interface IRepository<TEntity> where TEntity : class
    {
        // Yeni bir entity ekler
        void Add(TEntity entity);

        // Mevcut bir entity'yi siler. Varsayılan olarak soft delete (mantıksal silme) kullanır.
        void Delete(TEntity entity, bool softDelete = true);

        // Belirli bir id'ye sahip olan entity'yi siler
        void Delete(int id);

        // Mevcut bir entity'yi günceller
        void Update(TEntity entity);

        // Veritabanından id'ye göre bir entity getirir
        TEntity GetById(int id);

        // Belirtilen bir koşula (predicate) göre entity getirir
        TEntity Get(Expression<Func<TEntity, bool>> predicate);

        // Veritabanındaki tüm entity'leri getirir. İsteğe bağlı olarak predicate (koşul) eklenebilir.
        IQueryable<TEntity> GetAll(Expression<Func<TEntity, bool>> predicate = null);
    }
}
