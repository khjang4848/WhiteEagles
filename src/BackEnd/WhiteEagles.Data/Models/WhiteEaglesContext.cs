namespace WhiteEagles.Data.Models
{
    using System.Threading.Tasks;
    using Microsoft.EntityFrameworkCore;

    public class WhiteEaglesContext : DbContext
    {
        public virtual async Task CommitAsync() => await base.SaveChangesAsync();
        public virtual void Commit() => base.SaveChanges();
    }
}