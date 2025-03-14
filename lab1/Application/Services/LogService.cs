using Application.Interfaces;
using Application.Options;
using Microsoft.Extensions.Options;

namespace Application.Services;

public class LogService(IOptions<LoggerOptions> loggerOptions) : ILogService
{
    private readonly string _fileName = loggerOptions.Value.FileNameForLogging;
    
    public async Task Log(string message)
    {
        await using var fileStream = new FileStream(_fileName, FileMode.Append, FileAccess.Write);

        await using var streamWriter = new StreamWriter(fileStream);
        
        await streamWriter.WriteLineAsync($"{DateTime.Now} : {message}");
    }

    public async Task<ICollection<string>> GetLogs()
    {
        await using var fileStream = new FileStream(_fileName, FileMode.Open, FileAccess.Read);
        
        using var streamReader = new StreamReader(fileStream);
        
        return (await streamReader.ReadToEndAsync()).Split('\n');
    }
}