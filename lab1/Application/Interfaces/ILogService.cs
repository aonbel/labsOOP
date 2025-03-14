namespace Application.Interfaces;

public interface ILogService
{
    Task Log(string message);
    
    Task<ICollection<string>> GetLogs();
}