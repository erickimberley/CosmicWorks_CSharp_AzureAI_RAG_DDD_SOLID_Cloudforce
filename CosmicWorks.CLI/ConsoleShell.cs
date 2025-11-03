using System.Text;

namespace CosmicWorks.CLI;

/// <summary>
/// Interactive console host responsible for the REPL loop, basic help output,
/// and forwarding user input to the ICommandRouter.
/// </summary>
internal sealed class ConsoleShell : IConsoleShell
{
    private readonly ICommandRouter _router;

    public ConsoleShell(ICommandRouter router) => _router = router;

    public async Task RunAsync(CancellationToken ct = default)
    {
        PrintWelcome();

        while (!ct.IsCancellationRequested)
        {
            Console.Write("cosmic> ");
            var line = Console.ReadLine();

            if (line is null) break;
            var t = line.Trim();

            if (t.Equals("exit", StringComparison.OrdinalIgnoreCase) ||
                t.Equals("quit", StringComparison.OrdinalIgnoreCase))
            {
                break;
            }

            if (t.Equals("help", StringComparison.OrdinalIgnoreCase) ||
                t.Equals("?", StringComparison.OrdinalIgnoreCase))
            {
                PrintHelp();
                continue;
            }

            try
            {
                var output = await _router.RouteAsync(line, ct);
                if (!string.IsNullOrWhiteSpace(output))
                {
                    Console.WriteLine(output);
                }
            }
            catch (Exception ex)
            {
                Console.ForegroundColor = ConsoleColor.Red;
                Console.WriteLine($"Error: {ex.Message}");
                Console.ResetColor();
            }
        }
    }

    private static void PrintWelcome()
    {
        var sb = new StringBuilder();
        sb.AppendLine("CosmicWorks CLI");
        sb.AppendLine("Type 'help' for commands, 'quit' to exit.");
        Console.WriteLine(sb.ToString());
    }

    private static void PrintHelp()
    {
        Console.WriteLine(
@"Commands:
  categories | list categories       List known categories
  prompt                             Show the current system prompt template
  discount <categoryLike> <rate>     Apply discount (e.g., 'discount helmets 30%' or 'discount ""Touring"" 0.25')
  remove <categoryLike>              Remove discounts (e.g., 'remove helmets')
  <anything else>                    Chat with the catalog

Notes:
  - <rate> may be a fraction (0.30) or a percent (30 or 30%).");
    }
}