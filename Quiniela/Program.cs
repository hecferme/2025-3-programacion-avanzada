using System;
using System.Reflection;

// Import Scores from scores.cs

class Program
{
    static void Main(string[] args)
    {
        // Require 4 mandatory command-line arguments
        if (args.Length != 4)
        {
            Console.WriteLine("Error: You must provide 4 integer arguments: guess1 guess2 real1 real2");
            Console.WriteLine("Example: Quiniela.exe 2 1 3 1");
            Environment.Exit(1);
        }

        int guess1 = 0, guess2 = 0, real1 = 0, real2 = 0;
        if (!int.TryParse(args[0], out guess1) || !int.TryParse(args[1], out guess2) ||
            !int.TryParse(args[2], out real1) || !int.TryParse(args[3], out real2))
        {
            Console.WriteLine("Error: All arguments must be valid integers.");
            Environment.Exit(1);
            return;
        }

        // Show entered values
        Console.WriteLine($"Your guess: {guess1} - {guess2}");
        Console.WriteLine($"Real score: {real1} - {real2}");

        // Dynamically invoke CalculateMatchPoints
        var scores = new Scores(guess1, guess2, real1, real2);
        MethodInfo? method = typeof(Scores).GetMethod("CalculateMatchPoints");
        if (method == null)
        {
            Console.WriteLine("Error: Could not find CalculateMatchPoints method.");
            Environment.Exit(1);
            return;
        }
        object? result = method.Invoke(scores, null);
        if (result == null)
        {
            Console.WriteLine("Error: Method invocation returned null.");
            Environment.Exit(1);
            return;
        }
        int points = (int)result;
        Console.WriteLine($"Points awarded: {points}");
    }
}
