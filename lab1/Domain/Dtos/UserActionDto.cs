using Domain.Entities.Core;

namespace Domain.Dtos;

public class UserActionDto : EntityDto 
{
    public int UserId { get; set; }
    public DateTime Date { get; set; }
    public string ActionTargetType { get; set; }
    public int PreviousStateId { get; set; }
    public int Type { get; set; }
}