using System.Linq.Expressions;

namespace Data_Access_Layer.Repositories
{
    public interface IGenericRepository<T> where T : class
    {
        IEnumerable<T> GetAll();
            IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includes);
        T GetById(Guid id, params Expression<Func<T, object>>[] includes);
        T GetById(object id);
        void Add(T entity);
        void Update(T entity);
        void Delete(object id);
        void Save();
        Task<IEnumerable<T>> GetAllAsync();

        // في الـ IRepository
        IQueryable<T> GetQueryable(params Expression<Func<T, object>>[] includes);

    }
}