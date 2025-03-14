using Microsoft.Extensions.Options;
using Presentation.Interfaces;

namespace Presentation.Entities;

public class InfoForm<TEntity> : IUiInstance
{
    public required TEntity Entity { get; set; }
    public required string Name { get; set; }
    public bool IsRunning { get; private set; }
    public async Task RunAsync()
    {
        IsRunning = true;
        Console.CursorVisible = false;
        
        Display();

        while (IsRunning)
        {
            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.Home:
                    IsRunning = false;
                    break;
            }
        }

        Console.CursorVisible = true;
    }

    private void Display()
    {
        Console.Clear();
        Console.WriteLine($"\n=== {Name.ToUpper()} ===");
        
        foreach (var property in Entity.GetType().GetProperties())
        {
            Console.WriteLine($"{property.Name}: {property.GetValue(Entity)}");
        }
        
        Console.WriteLine("\n[HOME] Exit");
    }
}