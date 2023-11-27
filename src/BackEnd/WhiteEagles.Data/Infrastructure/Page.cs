namespace WhiteEagles.Data.Infrastructure
{
    using System.Linq;

    public class Page
    {
        public int PageNumber { get; set; }
        public int PageSize { get; set; }

        public Page()
        {
            PageNumber = 1;
            PageSize = 10;
        }

        public Page(int pageNumber, int pageSize)
        {
            PageNumber = pageNumber;
            PageSize = pageSize;
        }

        public int Skip => (PageNumber - 1) * PageSize;
    }

    public static class PagingExtensions
    {
        public static IQueryable<T> GetPage<T>(this IQueryable<T> queryable, Page page)
            => queryable.Skip(page.Skip).Take(page.PageSize);
    }
}
