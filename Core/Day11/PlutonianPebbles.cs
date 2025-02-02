using System.Diagnostics;
using Core.Shared;
using Core.Shared.Extensions;
using Core.Shared.Modules;

namespace Core.Day11;

public class PlutonianPebbles : BaseDayModule
{
    public PlutonianPebbles(ITestOutputHelper outputHelper) : base(outputHelper) { }
    
    public override int Day => 11;

    public override string Title => "Plutonian Pebbles";

    [Fact] public void Part1_Sample() => ExecutePart1(GetData(InputType.Sample)).Should().Be(55312);
    [Fact] public void Part1() => ExecutePart1(GetData(InputType.Input));

    [Fact(Skip = "Brute force can't help you here")] public void Part2() => ExecutePart2(GetData(InputType.Input));

    public long ExecutePart1(string data)
    {
        var stones = LoadStones(data);

        var solution = GetCountAfterBlinks(stones, 25);
        return solution;
    }
    
    public long ExecutePart2(string data)
    {
        var stones = LoadStones(data);

        var solution = GetCountAfterBlinks(stones, 75);
        return solution;
    }
    
    private List<Stone> LoadStones(string data) =>
        data.Split(' ').Select(s => new Stone(long.Parse(s))).ToList();

    private List<Stone> Blink(List<Stone> stones, List<IStoneTransformRule> rules)
    {
        var newStones = stones
            .SelectMany(s => rules.First(r => r.AppliesTo(s)).ApplyTo(s))
            .ToList();
        return newStones;
    }

    private long GetCountAfterBlinks(List<Stone> stones, int blinkTimes)
    {
        var rules = new List<IStoneTransformRule>() {
            new ZeroToOneRule(), new EvenDigitSplitRule(), new TwentyTwentyFourRule()
        };

        for (int i = 0; i < blinkTimes; i++)
        {
            stones = Blink(stones, rules);
        }
        
        var stoneCount = stones.Count;
        WriteLine($"Stones after {blinkTimes} blinks: {stoneCount}");

        return stoneCount;
    }
}

public record Stone(long Value);

public interface IStoneTransformRule
{
    bool AppliesTo(Stone stone);
    IEnumerable<Stone> ApplyTo(Stone stone);
}

public class ZeroToOneRule : IStoneTransformRule
{
    public bool AppliesTo(Stone stone) => stone.Value == 0;

    public IEnumerable<Stone> ApplyTo(Stone stone) => new[] { new Stone(1) };
}

public class EvenDigitSplitRule : IStoneTransformRule
{
    public bool AppliesTo(Stone stone)
    {
        // returns true if number has an even number of digits
        var digitCount = $"{stone.Value}".Length;
        return digitCount % 2 == 0;
    }

    public IEnumerable<Stone> ApplyTo(Stone stone)
    {
        // splits digits evenly across two new stones
        var digitCount = $"{stone.Value}".Length;
        yield return new Stone(long.Parse($"{stone.Value}".Substring(0, digitCount / 2)));
        yield return new Stone(long.Parse($"{stone.Value}".Substring(digitCount / 2, digitCount / 2)));
    }
}

public class TwentyTwentyFourRule : IStoneTransformRule
{
    public bool AppliesTo(Stone stone) => true;

    public IEnumerable<Stone> ApplyTo(Stone stone) => new[] { new Stone(stone.Value * 2024) };
}

