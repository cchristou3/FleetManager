using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Fleet.Api.Entities;
using Fleet.Api.Shared;
using Fleet.Api.Shared.Pagination;
using Microsoft.EntityFrameworkCore;

namespace Fleet.Api.Extensions;

/// <summary>
///     Extension methods for DbSet to control change tracking behavior.
/// </summary>
public static class QueryableExtensions
{
    public static async Task<PaginationResult<TSource>> PaginateAsync<TSource>(
        this IQueryable<TSource> source,
        PaginationParams paginationParams,
        CancellationToken ct = default)
        where TSource : BaseEntity
    {
        var count = await source.CountAsync(ct);
        var items = await source
            .Skip((paginationParams.PageNumber - 1) * paginationParams.PageSize)
            .Take(paginationParams.PageSize)
            .ToListAsync(ct);

        return new PaginationResult<TSource>(items, count);
    }
}