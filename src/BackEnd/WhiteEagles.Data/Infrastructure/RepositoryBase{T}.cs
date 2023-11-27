#nullable enable
namespace WhiteEagles.Data.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;
    using WhiteEagles.Data.Models;
    using X.PagedList;

    public abstract class RepositoryBase<T> where T : class
    {
        private WhiteEaglesContext? _dataContext;
        private readonly DbSet<T> _dbSet;

        protected RepositoryBase(IDatabaseFactory databaseFactory)
        {
            DatabaseFactory = databaseFactory;
            _dbSet = DataContext.Set<T>();
        }

        protected IDatabaseFactory DatabaseFactory { get; }

        protected WhiteEaglesContext DataContext
            => _dataContext ??= DatabaseFactory.Get();

        public virtual void Add(T entity) => _dbSet.Add(entity);

        public virtual bool AddIfNotExist(T entity, Expression<Func<T, bool>> where)
        {
            var exists = _dbSet.Any(where);
            if (exists) return false;

            _dbSet.Add(entity);
            return true;

        }

        public virtual void Update(T entity)
        {
            _dbSet.Attach(entity);
            if (_dataContext != null)
                _dataContext.Entry(entity).State = EntityState.Modified;
        }

        public virtual void Update(Expression<Func<T, bool>> where, Func<T, T> select)
        {
            var entities = _dbSet.Where(@where).AsEnumerable().Select(select);
            foreach (var entity in entities)
            {
                _dbSet.Attach(entity);
                if (_dataContext != null)
                    _dataContext.Entry(entity).State = EntityState.Modified;
            }
        }

        public virtual void Delete(T entity) => _dbSet.Remove(entity);

        public virtual void Delete(Expression<Func<T, bool>> where)
        {
            var objects = _dbSet.Where(where).AsEnumerable();
            foreach (var obj in objects) _dbSet.Remove(obj);
        }

        public virtual T? Get(Expression<Func<T, bool>> @where)
            => _dbSet.Where(@where).FirstOrDefault();

        public virtual async Task<T> GetAsync(Expression<Func<T, bool>> @where)
            => await _dbSet.Where(where).FirstOrDefaultAsync();

        public virtual IQueryable<T> GetAll() => _dbSet;

        public virtual async Task<IEnumerable<T>> GetAllAsync()
            => await _dbSet.ToListAsync();

        public virtual IEnumerable<T> GetMany(Expression<Func<T, bool>> where)
            => _dbSet.Where(where).ToList();

        public virtual async Task<IEnumerable<T>> GetManyAsync(
            Expression<Func<T, bool>> where)
            => await _dbSet.Where(where).ToListAsync();

        public virtual async Task<int> GetCountAsync(Expression<Func<T, bool>> where)
        {
            var list = await _dbSet.Where(where).ToListAsync();
            return list.Count;
        }

        public virtual async Task<IPagedList<T>> GetPageAsync<TOrder>(Page page,
            Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> order)
        {
            var result = await _dbSet.OrderBy(order).Where(where).GetPage(page).ToListAsync();

            var total = await _dbSet.CountAsync(where);
            return new StaticPagedList<T>(result, page.PageNumber, page.PageSize, total);
        }

        public virtual async Task<IPagedList<T>> GetPageAsync<TOrder>(Page page,
            Expression<Func<T, TOrder>> order)
        {
            var result = await _dbSet.OrderBy(order).GetPage(page).ToListAsync();

            var total = await _dbSet.CountAsync();
            return new StaticPagedList<T>(result, page.PageNumber, page.PageSize, total);
        }

        public virtual async Task<IPagedList<T>> GetPageDescendingAsync<TOrder>(Page page,
            Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> order)
        {
            var result = await _dbSet.OrderByDescending(order).Where(where)
                .GetPage(page).ToListAsync();

            var total = await _dbSet.CountAsync(where);
            return new StaticPagedList<T>(result, page.PageNumber, page.PageSize, total);
        }

        public virtual async Task<IPagedList<T>> GetPageDescendingAsync<TOrder>(Page page,
            Expression<Func<T, TOrder>> order)
        {
            var result = await _dbSet.OrderByDescending(order).GetPage(page).ToListAsync();

            var total = await _dbSet.CountAsync();
            return new StaticPagedList<T>(result, page.PageNumber, page.PageSize, total);
        }

        public virtual async Task<IEnumerable<T>> GetOrderAsync<TOrder>(
            Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> order)
            => await _dbSet.Where(where).OrderBy(order).ToListAsync();

        public virtual async Task<IEnumerable<T>> GetOrderAsync<TOrder>(
            Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> order,
            int takeCount)
            => await _dbSet.Where(where).OrderBy(order).Take(takeCount).ToListAsync();

        public virtual async Task<IEnumerable<T>> GetOrderAsync<TOrder>
            (Expression<Func<T, TOrder>> order, int takeCount)
            => await _dbSet.OrderBy(order).Take(takeCount).ToListAsync();

        public virtual async Task<IEnumerable<T>> GetDescendingAsync<TOrder>(
            Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> order)
            => await _dbSet.Where(where).OrderByDescending(order).ToListAsync();

        public virtual async Task<IEnumerable<T>> GetDescendingAsync<TOrder>(
            Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> order,
            int takeCount)
            => await _dbSet.Where(where).OrderByDescending(order).Take(takeCount)
                .ToListAsync();

        public virtual async Task<IEnumerable<T>> GetDescendingAsync<TOrder>
            (Expression<Func<T, TOrder>> order, int takeCount)
            => await _dbSet.OrderByDescending(order).Take(takeCount).ToListAsync();
    }
}
