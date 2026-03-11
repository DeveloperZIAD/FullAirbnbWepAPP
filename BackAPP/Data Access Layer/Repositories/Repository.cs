using Data_Access_Layer.Data;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Data_Access_Layer.Repositories
{
    public class GenericRepository<T> : IGenericRepository<T> where T : class
    {
        private readonly ApplicationDbContext _context;
        private readonly DbSet<T> _dbSet;

        public GenericRepository(ApplicationDbContext context)
        {
            _context = context;
            _dbSet = _context.Set<T>(); // ده بيخلي الـ EF يركز على الجدول الخاص بالنوع T
        }
        public async Task<IEnumerable<T>> GetAllAsync()
        {
            return await _context.Set<T>().ToListAsync();
        }
        // جلب كل البيانات (بديل SELECT *)
        public IEnumerable<T> GetAll()
        {
            return _dbSet.ToList();
        }
        public IQueryable<T> GetQueryable(params Expression<Func<T, object>>[] includes)
        {
            // ابدأ بالـ DbSet الخاص بالنوع T
            IQueryable<T> query = _dbSet.AsQueryable();

            // قم بإضافة الـ Includes (علاقات الجداول)
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            return query;
        }

        public IEnumerable<T> GetAll(params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _context.Set<T>();
            foreach (var include in includes)
            {
                query = query.Include(include);
            }
            return query.ToList();
        }

        // جلب سجل واحد (بديل WHERE Id = @Id)
        public T GetById(object id)
        {
            return _dbSet.Find(id);
        }
        public T GetById(Guid id, params Expression<Func<T, object>>[] includes)
        {
            IQueryable<T> query = _dbSet;

            // نقوم بإضافة الجداول المرتبطة أولاً
            foreach (var include in includes)
            {
                query = query.Include(include);
            }

            // ثم نبحث عن الـ ID في النهاية
            // ملاحظة: افترضنا أن الحقل اسمه Id في الـ BaseEntity
            return query.FirstOrDefault(e => EF.Property<Guid>(e, "Id") == id);
        }
        

        // إضافة سجل (بديل INSERT INTO)
        public void Add(T entity)
        {
            _dbSet.Add(entity);
        }

        // تعديل سجل (بديل UPDATE)
        public void Update(T entity)
        {
            _dbSet.Attach(entity);
            _context.Entry(entity).State = EntityState.Modified;
        }

        // حذف سجل (بديل DELETE)
        public void Delete(object id)
        {
            T existing = _dbSet.Find(id);
            if (existing != null)
            {
                _dbSet.Remove(existing);
            }
        }

        // حفظ التغييرات (هنا الـ EF بيبعت كل الأوامر للسيرفر مرة واحدة)
        public void Save()
        {
            _context.SaveChanges();
        }
    }
}