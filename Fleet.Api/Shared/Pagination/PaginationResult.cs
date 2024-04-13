using System.Collections.Generic;

namespace Fleet.Api.Shared.Pagination;

public class PaginationResult<TSource> : List<TSource> where TSource : class
{
    public PaginationResult(IEnumerable<TSource> items, int count)
    {
        TotalCount = count;
        AddRange(items);
    }

    public int TotalCount { get; set; }
}