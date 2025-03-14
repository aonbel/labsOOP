namespace Domain.Dtos.BankClientDtos;

public class BankClientDto : BaseEntityDto
{
    public ICollection<int> ServiceIds { get; set; } = [];
    public ICollection<int> RecordIds { get; set; } = [];
    public bool IsApproved { get; set; }
}