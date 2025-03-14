using Domain.Entities.Core;

namespace Application.Reverters;

public interface IReverter<TEntity> where TEntity : BaseEntity
{
    public Task RevertCreateActionAsync(int entityId, CancellationToken cancellationToken);
    public Task RevertUpdateActionAsync(TEntity entityState, CancellationToken cancellationToken);
    public Task RevertDeleteActionAsync(TEntity entityState, CancellationToken cancellationToken);
}