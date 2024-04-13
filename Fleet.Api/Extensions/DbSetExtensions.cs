using System.Linq;
using Fleet.Api.Entities;
using Microsoft.EntityFrameworkCore;

namespace Fleet.Api.Extensions;

/// <summary>
///     Extension methods for DbSet to control change tracking behavior.
/// </summary>
public static class DbSetExtensions
{
    /// <summary>
    ///     Configures change tracking for a DbSet based on the specified flag.
    /// </summary>
    /// <typeparam name="TSource">The type of entities in the DbSet, must inherit from BaseEntity.</typeparam>
    /// <param name="dbSet">The DbSet to configure change tracking for.</param>
    /// <param name="trackChanges">A boolean flag indicating whether to track changes or not.</param>
    /// <returns>
    ///     An IQueryable that includes or excludes change tracking based on the provided flag.
    /// </returns>
    public static IQueryable<TSource> TrackChanges<TSource>(this DbSet<TSource> dbSet, bool trackChanges)
        where TSource : BaseEntity
    {
        // If trackChanges is true, enable change tracking; otherwise, disable it.
        return trackChanges ? dbSet.AsTracking() : dbSet.AsNoTracking();
    }
}