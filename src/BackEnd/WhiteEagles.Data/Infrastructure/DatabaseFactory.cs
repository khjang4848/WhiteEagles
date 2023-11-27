#nullable enable
namespace WhiteEagles.Data.Infrastructure
{
    using Models;

    public class DatabaseFactory : Disposable, IDatabaseFactory
    {
        private WhiteEaglesContext? _dataContext;

        public WhiteEaglesContext Get()
            => _dataContext ??= new WhiteEaglesContext();

        protected override void DisposeCore() => _dataContext?.Dispose();
    }
}
