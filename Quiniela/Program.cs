using System;

class Program
{
    static int CalculateMatchPoints(int guessTeam1, int guessTeam2, int realTeam1, int realTeam2)
    {
        // Exact score match
        if (guessTeam1 == realTeam1 && guessTeam2 == realTeam2)
            return 5;

        // Guess winner correctly (but not exact score)
        bool guessWin = guessTeam1 > guessTeam2;
        bool realWin = realTeam1 > realTeam2;
        bool guessDraw = guessTeam1 == guessTeam2;
        bool realDraw = realTeam1 == realTeam2;

        if ((guessWin && realWin) || (guessDraw && realDraw) || (!guessWin && !guessDraw && !realWin && !realDraw))
            return 2;

        // Otherwise
        return 0;
    }

    static void Main(string[] args)
    {
        // Example usage
        int guess1 = 2, guess2 = 1;
        int real1 = 3, real2 = 1;
        int points = CalculateMatchPoints(guess1, guess2, real1, real2);
        Console.WriteLine($"Points awarded: {points}");
    }
}
