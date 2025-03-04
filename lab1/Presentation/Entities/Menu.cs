using Presentation.Interfaces;

namespace Presentation.Entities;

public class Menu : IUiInstance
{
    public List<(string Label, IUiInstance Instance)> Options { get; set; }
    public int CurrOption { get; private set; }
    public string Name { get; set; }
    public bool IsRunning { get; private set; }

    public async Task RunAsync()
    {
        IsRunning = true;
        Console.CursorVisible = false;

        while (IsRunning)
        {
            Display();
            var key = Console.ReadKey(true).Key;

            switch (key)
            {
                case ConsoleKey.UpArrow:
                    CurrOption = Math.Max(0, CurrOption - 1);
                    break;
                case ConsoleKey.DownArrow:
                    CurrOption = Math.Min(Options.Count - 1, CurrOption + 1);
                    break;
                case ConsoleKey.Enter:
                    await ExecuteSelectedOption();
                    break;
                case ConsoleKey.Escape:
                    IsRunning = false;
                    break;
            }
        }

        Console.CursorVisible = true;
    }

    private async Task ExecuteSelectedOption()
    {
        if (Options.Count == 0) return;

        var (_, instance) = Options[CurrOption];
        if (instance != null)
        {
            await instance.RunAsync();
        }
    }

    private void Display()
    {
        Console.Clear();
        Console.WriteLine($"\n=== {Name.ToUpper()} ===\n");

        for (int i = 0; i < Options.Count; i++)
        {
            var prefix = i == CurrOption ? " ► " : "   ";
            Console.ForegroundColor = i == CurrOption ? ConsoleColor.Cyan : ConsoleColor.Gray;
            Console.WriteLine($"{prefix}{Options[i].Label}");
        }
        Console.ResetColor();
        Console.WriteLine("\n[UP/DOWN] Navigate | [ENTER] Select | [ESC] Exit");
    }
}