namespace WhiteEagles.Data.Infrastructure
{
    using System.Threading.Tasks;

    public interface IUnitOfWork
    {
        Task CommitAsync();
        void Commit();
    }
}
    