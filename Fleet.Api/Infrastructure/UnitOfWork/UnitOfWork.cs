using System.Threading;
using System.Threading.Tasks;
using Fleet.Api.Database;

namespace Fleet.Api.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly ApplicationDbContext _db;

    public UnitOfWork(ApplicationDbContext db)
    {
        _db = db;
    }

    public Task SaveChangesAsync(CancellationToken token)
    {
        return _db.SaveChangesAsync(token);
    }
}