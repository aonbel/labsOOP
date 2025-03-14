using System.ComponentModel.DataAnnotations;

namespace Application.Options;

public class LoggerOptions
{
    [Required]
    public string FileNameForLogging;
}