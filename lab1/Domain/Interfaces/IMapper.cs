namespace Domain.Interfaces;

public interface IMapper<TEntity, TDto>
{
    public TDto Map(TEntity entity);
    public TEntity Map(TDto dto);
}