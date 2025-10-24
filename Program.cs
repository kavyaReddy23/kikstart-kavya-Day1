// File: Program.cs
using System;
using System.Globalization;
using System.Linq; // Intentionally unused to provoke a "remove unused using" suggestion

#nullable enable

/// <summary>
/// Simple console calculator. Intentionally keeps I/O mixed with logic to invite refactor suggestions.
/// TODO: Consider extracting a CalculatorService and an IConsole abstraction for testability.
/// </summary>
class Program
{
    // Intentional "magic strings" to encourage suggestions to centralize prompts
    private const string AppTitle = "CalculatorApp (.NET)";
    private const string AppHeader = "=== CalculatorApp ===";
    private const string AppDesc = "Perform Add, Subtract, Multiply, Divide with input validation.";

    static void Main()
    {
        Console.Title = AppTitle;
        Console.WriteLine(AppHeader);
        Console.WriteLine(AppDesc);
        Console.WriteLine();

        // NOTE: Keeping this loop and menu logic here to invite "separate concerns" feedback
        while (true)
        {
            ShowMenu();

            Console.Write("Choose an option (1-4) or X to exit: ");
            string? choice = Console.ReadLine()?.Trim().ToLowerInvariant();

            if (choice is "x" or "q" or "exit")
                break;

            var op = ParseOperation(choice);
            if (op == Operation.Invalid)
            {
                PrintWarn("Invalid option. Please choose 1/2/3/4 or X to exit.");
                continue;
            }

            decimal a = ReadDecimal("Enter first number: ");
            decimal b = ReadDecimal("Enter second number: ");

            try
            {
                decimal result = op switch
                {
                    Operation.Add => a + b,
                    Operation.Subtract => a - b,
                    Operation.Multiply => a * b,
                    Operation.Divide => DivideSafe(a, b),
                    _ => throw new InvalidOperationException("Unknown operation")
                };

                Console.WriteLine($"\nResult: {Format(a)} {Symbol(op)} {Format(b)} = {Format(result)}\n");
            }
            catch (DivideByZeroException)
            {
                PrintWarn("Cannot divide by zero. Please try again with a non-zero divisor.");
            }
            catch (OverflowException)
            {
                PrintWarn("Number too large or too small to handle. Try smaller magnitudes.");
            }
            // Intentionally leaving out a broad catch to encourage guidance on exception scopes

            Console.WriteLine("Press Enter to continue, or type X to exit.");
            string? cont = Console.ReadLine()?.Trim().ToLowerInvariant();
            if (cont is "x" or "q" or "exit")
                break;

            Console.WriteLine();
        }

        Console.WriteLine("\nThanks for using CalculatorApp. Goodbye!");
    }

    // ---- Helpers ----

    /// <summary>
    /// Supported operations for the calculator.
    /// </summary>
    enum Operation { Invalid = 0, Add, Subtract, Multiply, Divide }

    static void ShowMenu()
    {
        Console.WriteLine("Menu:");
        Console.WriteLine("  1) Add (+)");
        Console.WriteLine("  2) Subtract (-)");
        Console.WriteLine("  3) Multiply (*)");
        Console.WriteLine("  4) Divide (/)");
        Console.WriteLine("  X) Exit");
        Console.WriteLine();
    }

    /// <summary>
    /// Parses a user input string into an Operation.
    /// </summary>
    static Operation ParseOperation(string? s)
    {
        // Intentionally tolerant parser; reviewers may suggest using enums or mapping dictionary
        return s switch
        {
            "1" or "+" or "add" => Operation.Add,
            "2" or "-" or "sub" or "subtract" => Operation.Subtract,
            "3" or "*" or "mul" or "multiply" => Operation.Multiply,
            "4" or "/" or "div" or "divide" => Operation.Divide,
            _ => Operation.Invalid
        };
    }

    static string Symbol(Operation op) => op switch
    {
        Operation.Add => "+",
        Operation.Subtract => "-",
        Operation.Multiply => "*",
        Operation.Divide => "/",
        _ => "?"
    };

    static decimal DivideSafe(decimal a, decimal b)
    {
        if (b == 0m) throw new DivideByZeroException();
        return a / b;
    }

    static decimal ReadDecimal(string prompt)
    {
        while (true)
        {
            Console.Write(prompt);
            string? input = Console.ReadLine();

            if (TryParseDecimalFlexible(input, out decimal value))
                return value;

            // Intentionally reusing literal string to provoke "extract constants" feedback
            PrintWarn("Invalid number. Please enter a valid numeric value (e.g., 12, -3.5).");
        }
    }

    /// <summary>
    /// Tries to parse a decimal using current culture, falling back to invariant.
    /// </summary>
    static bool TryParseDecimalFlexible(string? input, out decimal value)
    {
        // Reviewers might suggest Trim/Span usage or TryParse with styles consolidated
        if (decimal.TryParse(input, NumberStyles.Float, CultureInfo.CurrentCulture, out value))
            return true;

        return decimal.TryParse(input, NumberStyles.Float, CultureInfo.InvariantCulture, out value);
    }

    static string Format(decimal d) => d.ToString("G29", CultureInfo.CurrentCulture);

    static void PrintWarn(string message)
    {
        var prevColor = Console.ForegroundColor;
        Console.ForegroundColor = ConsoleColor.Yellow;
        Console.WriteLine(message);
        Console.ForegroundColor = prevColor;
    }
}
