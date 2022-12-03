enum GameResult
{
    Win = 6,
    Lose = 0,
    Draw = 3,
}

enum Hand
{
    Rock = 1,
    Paper = 2,
    Scissors = 3
}

static class HandUtils
{
    public static Hand LosesTo(this Hand me) => me switch
    {
        Hand.Rock => Hand.Paper,
        Hand.Paper => Hand.Scissors,
        Hand.Scissors => Hand.Rock,
    };

    public static Hand WinsAgainst(this Hand me) => me switch
    {
        Hand.Rock => Hand.Scissors,
        Hand.Paper => Hand.Rock,
        Hand.Scissors => Hand.Paper,
    };

    public static Hand GetHand(char handSymbol)
    {
        return handSymbol switch
        {
            'A' or 'X' => Hand.Rock,
            'B' or 'Y' => Hand.Paper,
            'C' or 'Z' => Hand.Scissors,
        };
    }

    public static GameResult GetDesiredResult(char resultSymbol)
    {
        return resultSymbol switch
        {
            'X' => GameResult.Lose,
            'Y' => GameResult.Draw,
            'Z' => GameResult.Win,
        };
    }

    public static GameResult PlayScriptedHand(Hand player, Hand opponent)
    {
        if (opponent.LosesTo() == player) return GameResult.Win;
        if (opponent.WinsAgainst() == player) return GameResult.Lose;
        return GameResult.Draw;
    }

    public static Hand HandNeededToAchieveResultVersus(Hand opponent, GameResult desiredResult)
    {
        return desiredResult switch
        {
            GameResult.Win => opponent.LosesTo(),
            GameResult.Lose => opponent.WinsAgainst(),
            GameResult.Draw => opponent,
        };
    }
}

class Program
{
    public static void Main()
    {
        string[] lines = File.ReadAllLines("./input.txt");

        int yourTotal = 0;
        int opponentsTotal = 0;

        foreach (string line in lines)
        {
            (int yourScore, int opponentsScore) = PlayScriptedHandStrategy(line);
            // (int yourScore, int opponentsScore) = PlayDesiredResultStrategy(line);

            yourTotal += yourScore;
            opponentsTotal += opponentsScore;
        }

        Console.WriteLine($"Total Score: \nYou \t\t({yourTotal}) \nOpponent \t({opponentsTotal})");
    }

    private static (int yourTotal, int opponentsTotal) PlayDesiredResultStrategy(string line)
    {
        Hand opponentsHand = HandUtils.GetHand(line[0]);

        GameResult yourDesiredResult = HandUtils.GetDesiredResult(line[2]);

        Hand yourNecessaryHand = HandUtils.HandNeededToAchieveResultVersus(opponentsHand, yourDesiredResult);

        GameResult opponentsResult = HandUtils.PlayScriptedHand(opponentsHand, yourNecessaryHand);

        int yourScore = (int)yourDesiredResult + (int)yourNecessaryHand;
        int opponentsScore = (int)opponentsResult + (int)opponentsHand;

        return (yourScore, opponentsScore);
    }

    private static (int yourTotal, int opponentsTotal) PlayScriptedHandStrategy(string line)
    {
        Hand opponentsHand = HandUtils.GetHand(line[0]);
        Hand yourHand = HandUtils.GetHand(line[2]);

        GameResult yourResult = HandUtils.PlayScriptedHand(yourHand, opponentsHand);
        GameResult opponentsResult = HandUtils.PlayScriptedHand(opponentsHand, yourHand);

        int yourScore = (int)yourResult + (int)yourHand;
        int opponentsScore = (int)opponentsResult + (int)opponentsHand;

        return (yourScore, opponentsScore);
    }
}