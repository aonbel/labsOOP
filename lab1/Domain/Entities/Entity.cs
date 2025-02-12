namespace Domain.Entities;

public abstract class Entity
{
    public required int Id { get; init; }
    public required string Name { get; set; }
}