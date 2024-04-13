using System.Threading;
using System.Threading.Tasks;

namespace Fleet.Api.Infrastructure;

public interface IUnitOfWork
{
    Task SaveChangesAsync(CancellationToken token);
}