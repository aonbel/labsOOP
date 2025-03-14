using Presentation.Interfaces;

namespace Presentation.Entities;

public class ActionForm : IUiInstance
{
    public string Name { get; set; }
    
    public bool IsRunning { get; private set; }
    
    public delegate Task ActionHandler();
    
    public ActionHandler Handler { get; set; }
        
    public async Task RunAsync()
    {
        Console.Clear();
        Console.WriteLine($"\n=== {Name.ToUpper()} ===");
        
        IsRunning = true;
        
        await Handler.Invoke();
        
        IsRunning = false;
    }
}