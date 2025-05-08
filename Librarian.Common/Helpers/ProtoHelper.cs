using TuiHub.Protos.Librarian.V1;

namespace Librarian.Common.Helpers
{
    public static class ProtoHelper
    {
        public static IQueryable<T> ApplyPagingRequest<T>(this IQueryable<T> source, PagingRequest? pagingRequest)
        {
            if (pagingRequest == null)
            {
                return source;
            }
            else
            {
                var pageSize = pagingRequest.PageSize;
                var pageNum = pagingRequest.PageNum;
                return source.Skip((int)(pageSize * (pageNum - 1))).Take((int)pageSize);
            }
        }
    }
}
