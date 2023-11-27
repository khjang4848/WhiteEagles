namespace WhiteEagles.Data.Infrastructure
{
    using System;

    public abstract class Disposable : IDisposable
    {
        private bool _isDisposed;

        ~Disposable() => Dispose(false);

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public void Dispose(bool disposing)
        {
            if (!_isDisposed && disposing)
            {
                DisposeCore();
            }

            _isDisposed = true;
        }

        protected virtual void DisposeCore() { }
    }
}
