using Domain.Entities;
using Domain.Entities.Core;

namespace Application.Interfaces;

public interface IUserActionService
{
    Task<int> AddUserActionAsync(int userId, BaseEntity actionTargetStateBeforeAction, string actionName, ActionType actionType, CancellationToken cancellationToken);
    
    Task<BaseEntity> GetCurrentStateOfActionTargetAsync(int userActionId, CancellationToken cancellationToken);
    
    Task<UserAction> GetUserActionByIdAsync(int userActionId, CancellationToken cancellationToken);

    Task<ICollection<UserAction>> GetUserActionsByUserIdAsync(int userId, CancellationToken cancellationToken);

    Task RevertUserActionByIdAsync(int userActionId, CancellationToken cancellationToken);
}