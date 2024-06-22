using System.Linq.Expressions;

namespace EcommerceAPI.Repository
{
    public interface IGenericRepository<T>
    {
        //public T Add(T obj);
        public Task<T> Add(T obj);
        public Task<T> Delete(T obj);
        public Task<List<T>> GetAll();
        public Task<int> SaveChanges();
        public Task <T> GetById(int id);
        public Task<T> Update(int id, T obj);
    }
}
