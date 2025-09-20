using Xunit;

public class ScoresTests
{
    [Theory]
    [InlineData(1, 1, 1, 1, 5)] // perfect tie
    [InlineData(2, 1, 2, 1, 5)] // perfect team 1 win
    [InlineData(1, 2, 1, 2, 5)] // perfect team 2 win
    [InlineData(2, 1, 1, 1, 0)] // incorrect tie
    [InlineData(1, 2, 2, 1, 0)] // incorrect team 1 win
    [InlineData(1, 1, 1, 2, 0)] // incorrect team 2 win
    [InlineData(2, 1, 3, 1, 3)] // team 1 win, not exact, one score matches (1)
    [InlineData(2, 1, 2, 0, 3)] // team 1 win, not exact, one score matches (2)
    [InlineData(1, 1, 2, 2, 2)] // draw, not exact, one score matches (2)
    [InlineData(1, 1, 3, 4, 0)] // draw, not exact, no score matches
    [InlineData(1, 2, 0, 3, 2)] // team 2 win, not exact, one score matches (3)
    [InlineData(0, 2, 3, 2, 0)] // team 2 win, not exact, no score matches
    [InlineData(4, 0, 5, 0, 3)] // team 1 win, not exact, one score matches (0)
    [InlineData(0, 3, 0, 5, 3)] // team 2 win, not exact, one score matches (0)
    public void CalculateMatchPoints_Scenarios_ReturnsExpected(int guess1, int guess2, int real1, int real2, int expected)
    {
        var scores = new Scores(guess1, guess2, real1, real2);
        Assert.Equal(expected, scores.CalculateMatchPoints());
    }
}
