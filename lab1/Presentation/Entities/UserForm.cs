using Presentation.Interfaces;

namespace Presentation.Entities;

public class UserForm<TEntity> : IUiInstance where TEntity : new()
{
    public delegate Task ResultHandler(TEntity entity);
    
    public required ResultHandler OnResult;
    
    private readonly TEntity _result = new ();
    public string Name { get; set; } = string.Empty;
    public bool IsRunning { get; private set; }

    public async Task RunAsync()
    {
        IsRunning = true;
        
        Console.Clear();
        Console.WriteLine($"\n=== {Name.ToUpper()} ===\n");

        foreach (var field in typeof(TEntity).GetProperties())
        {
            Console.Write($"Enter {field.Name}: ");
            var input = Console.ReadLine();
            
            if (string.IsNullOrWhiteSpace(input))
            {
                Console.WriteLine("Value cannot be empty!");
                field.SetValue(_result, default);
                continue;
            }

            try
            {
                var value = Convert.ChangeType(input, field.PropertyType);
                field.SetValue(_result, value);
            }
            catch
            {
                Console.WriteLine($"Invalid format for {field.PropertyType.Name}");
                field.SetValue(_result, default);
            }
        }

        await OnResult.Invoke(_result);
        IsRunning = false;
    }
}