
using EcommerceAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace EcommerceAPI.Repository
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        EcommerceContext db;
        public GenericRepository(EcommerceContext db)
        {
            this.db = db;
        }

        public async Task<List<T>> GetAll()
        {
            return await db.Set<T>().ToListAsync();
        }

        public async Task<T> GetById(int id)
        {
            return await db.Set<T>().FindAsync(id);
        }

        public async Task<T> Add(T obj)
        {
            await db.Set<T>().AddAsync(obj);
            return obj;
        }

        //public T Add(T obj)
        //{
        //    db.Entry<T>(obj).State = EntityState.Added;
        //    return obj;
        //}

        public async Task<T> Update(int id, T obj)
        {
            db.Entry<T>(obj).State = EntityState.Modified;
            return obj;
        }

        public async Task<T> Delete(T obj)
        {
            db.Entry<T>(obj).State = EntityState.Deleted;
            return obj;
        }

        public async Task<int> SaveChanges()
        {
            return await db.SaveChangesAsync();
        }

      
        public async Task<CartItem> GetByProductId(int productId)
        {
            return await db.CartItems.FirstOrDefaultAsync(ci => ci.ProductId == productId);
        }
    }
}
