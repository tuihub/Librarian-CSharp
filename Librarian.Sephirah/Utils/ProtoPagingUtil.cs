using TuiHub.Protos.Librarian.V1;

namespace Librarian.Sephirah.Utils
{
    public static class ProtoPagingUtil
    {
        public static IEnumerable<T> ApplyPagingRequest<T>(this IEnumerable<T> source, PagingRequest pagingRequest)
        {
            var pageSize = pagingRequest.PageSize;
            var pageNum = pagingRequest.PageNum;
            return source.Skip(pageSize * (pageNum - 1)).Take(pageSize);
        }
    }
}
