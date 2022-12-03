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
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public static GameResult GetDesiredResult(char resultSymbol)
    {
        return resultSymbol switch
        {
            'X' => GameResult.Lose,
            'Y' => GameResult.Draw,
            'Z' => GameResult.Win,
            _ => throw new ArgumentOutOfRangeException()
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
            // (yourTotal, opponentsTotal) = PlayScriptedHandStrategy(line, yourTotal, opponentsTotal);
            (yourTotal, opponentsTotal) = PlayDesiredResultStrategy(line, yourTotal, opponentsTotal);
        }


        Console.WriteLine($"Total Score: \nYou \t\t({yourTotal}) \nOpponent \t({opponentsTotal})");
        Console.WriteLine(yourTotal > opponentsTotal ? "You Win!" : "You Lose!");
    }

    private static (int yourTotal, int opponentsTotal) PlayDesiredResultStrategy(string line, int yourTotal,
        int opponentsTotal)
    {
        Hand opponentsHand = HandUtils.GetHand(line[0]);

        GameResult yourDesiredResult = HandUtils.GetDesiredResult(line[2]);

        Hand yourNecessaryHand = HandUtils.HandNeededToAchieveResultVersus(opponentsHand, yourDesiredResult);

        GameResult opponentsResult = HandUtils.PlayScriptedHand(opponentsHand, yourNecessaryHand);

        int yourScore = (int)yourDesiredResult + (int)yourNecessaryHand;
        int opponentsScore = (int)opponentsResult + (int)opponentsHand;

        Console.WriteLine(
            $"You have scored: \t{yourDesiredResult} ({(int)yourDesiredResult}) \t+ {yourNecessaryHand} ({(int)yourNecessaryHand}) \t= {yourScore}" +
            $"\nOpponent Score: \t{opponentsResult} ({(int)opponentsResult}) \t+ {opponentsHand} ({(int)opponentsHand}) \t= {opponentsScore}");

        Console.WriteLine(
            $"You scored: {yourScore}. Opponent Score: {yourScore}");

        yourTotal += yourScore;
        opponentsTotal += opponentsScore;

        Console.WriteLine($"Current Score: U: ({yourTotal}) O: ({opponentsTotal})");

        return (yourTotal, opponentsTotal);
    }

    private static (int yourTotal, int opponentsTotal) PlayScriptedHandStrategy(string line, int yourTotal,
        int opponentsTotal)
    {
        Hand opponentsHand = HandUtils.GetHand(line[0]);
        Hand yourHand = HandUtils.GetHand(line[2]);

        GameResult yourResult = HandUtils.PlayScriptedHand(yourHand, opponentsHand);
        GameResult opponentsResult = HandUtils.PlayScriptedHand(opponentsHand, yourHand);


        int yourScore = (int)yourResult + (int)yourHand;
        int opponentsScore = (int)opponentsResult + (int)opponentsHand;

        Console.WriteLine(
            $"You have scored: \t{yourResult} ({(int)yourResult}) \t+ {yourHand} ({(int)yourHand}) \t= {yourScore}" +
            $"\nOpponent Score: \t{opponentsResult} ({(int)opponentsResult}) \t+ {opponentsHand} ({(int)opponentsHand}) \t= {opponentsScore}");

        Console.WriteLine(
            $"You scored: {yourScore}. Opponent Score: {yourScore}");

        yourTotal += yourScore;
        opponentsTotal += opponentsScore;

        Console.WriteLine($"Current Score: U: ({yourTotal}) O: ({opponentsTotal})");
        return (yourTotal, opponentsTotal);
    }
}