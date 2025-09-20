using System;

public class Scores
{
    public int GuessTeam1 { get; }
    public int GuessTeam2 { get; }
    public int RealTeam1 { get; }
    public int RealTeam2 { get; }
    public Scores(int guessTeam1, int guessTeam2, int realTeam1, int realTeam2)
    {
        GuessTeam1 = guessTeam1;
        GuessTeam2 = guessTeam2;
        RealTeam1 = realTeam1;
        RealTeam2 = realTeam2;
    }

    public int CalculateMatchPoints()
    {
        // Exact score match
        if (GuessTeam1 == RealTeam1 && GuessTeam2 == RealTeam2)
            return 5;

        // Guess winner correctly (but not exact score)
        bool guessWin = GuessTeam1 > GuessTeam2;
        bool realWin = RealTeam1 > RealTeam2;
        bool guessDraw = GuessTeam1 == GuessTeam2;
        bool realDraw = RealTeam1 == RealTeam2;

        bool winnerGuessed = (guessWin && realWin) || (guessDraw && realDraw) || (!guessWin && !guessDraw && !realWin && !realDraw);
        if (winnerGuessed)
        {
            int points = 2;
            // Award extra point if one score matches
            if ((GuessTeam1 == RealTeam1 && GuessTeam2 != RealTeam2) || (GuessTeam2 == RealTeam2 && GuessTeam1 != RealTeam1))
                points += 1;
            return points;
        }

        // Otherwise
        return 0;
    }
}
