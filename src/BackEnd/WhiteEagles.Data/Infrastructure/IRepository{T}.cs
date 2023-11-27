namespace WhiteEagles.Data.Infrastructure
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Linq.Expressions;
    using System.Threading.Tasks;
    using X.PagedList;

    public interface IRepository<T> where T : class
    {
        void Add(T entity);
        bool AddIfNotExist(T entity, Expression<Func<T, bool>> where);

        void Update(T entity);

        void Update(Expression<Func<T, bool>> where, Func<T, T> select);

        void Delete(T entity);

        void Delete(Expression<Func<T, bool>> where);

        T Get(Expression<Func<T, bool>> where);

        Task<T> GetAsync(Expression<Func<T, bool>> where);

        IQueryable<T> GetAll();

        Task<IEnumerable<T>> GetAllAsync();

        IEnumerable<T> GetMany(Expression<Func<T, bool>> where);

        Task<IEnumerable<T>> GetManyAsync(Expression<Func<T, bool>> where);

        Task<IPagedList<T>> GetPageAsync<TOrder>(Page page,
            Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> order);

        Task<IPagedList<T>> GetPageAsync<TOrder>(Page page,
            Expression<Func<T, TOrder>> order);

        Task<IPagedList<T>> GetPageDescendingAsync<TOrder>(Page page,
            Expression<Func<T, bool>> where, Expression<Func<T, TOrder>> order);

        Task<IPagedList<T>> GetPageDescendingAsync<TOrder>(Page page,
            Expression<Func<T, TOrder>> order);

        Task<IEnumerable<T>> GetOrderAsync<TOrder>(Expression<Func<T, bool>> where,
            Expression<Func<T, TOrder>> order);

        Task<IEnumerable<T>> GetOrderAsync<TOrder>(Expression<Func<T, bool>> where,
            Expression<Func<T, TOrder>> order, int takeCount);

        Task<IEnumerable<T>> GetOrderAsync<TOrder>(Expression<Func<T, TOrder>> order,
            int takeCount);

        Task<IEnumerable<T>> GetDescendingAsync<TOrder>(Expression<Func<T, bool>> where,
            Expression<Func<T, TOrder>> order);

        Task<IEnumerable<T>> GetDescendingAsync<TOrder>(Expression<Func<T, bool>> where,
            Expression<Func<T, TOrder>> order, int takeCount);

        Task<IEnumerable<T>> GetDescendingAsync<TOrder>
            (Expression<Func<T, TOrder>> order, int takeCount);

        Task<int> GetCountAsync(Expression<Func<T, bool>> where);
    }
}
