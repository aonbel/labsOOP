using Domain.Entities.Core;

namespace Domain.Entities;

public enum ActionType
{
    Create,
    Update,
    Delete
}

public class UserAction : Entity
{
    public User User { get; set; }
    
    public DateTime Date { get; set; }
    
    public int PreviousStateId { get; set; }
    
    public BaseEntity PreviousState { get; set; }
    
    public ActionType Type { get; set; }
}