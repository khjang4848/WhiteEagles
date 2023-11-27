#nullable enable

namespace WhiteEagles.Data.Infrastructure
{
    using System.Threading.Tasks;

    using Models;

    public class UnitOfWork : IUnitOfWork
    {
        private readonly IDatabaseFactory _databaseFactory;
        private WhiteEaglesContext? _dataContext;

        public UnitOfWork(IDatabaseFactory databaseFactory)
            => _databaseFactory = databaseFactory;

        protected WhiteEaglesContext DataContext
            => _dataContext ??= _databaseFactory.Get();

        public async Task CommitAsync() => await DataContext.CommitAsync();

        public void Commit() => DataContext.Commit();
    }
}
