namespace WhiteEagles.Data.Infrastructure
{
    using System;
    using Models;

    public interface IDatabaseFactory : IDisposable
    {
        WhiteEaglesContext Get();
    }
}
