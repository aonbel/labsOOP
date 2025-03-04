namespace Presentation.Interfaces;

public interface IUiInstance
{
    public string Name { get; set; }
    public bool IsRunning { get;  }
    Task RunAsync();
}