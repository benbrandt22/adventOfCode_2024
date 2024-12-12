using System.Text.RegularExpressions;
using Core.Shared;
using Core.Shared.Extensions;
using Core.Shared.Modules;

namespace Core.Day01;

public class HistorianHysteria : BaseDayModule
{
    public HistorianHysteria(ITestOutputHelper outputHelper) : base(outputHelper) { }
    
    public override int Day => 1;
    public override string Title => "Historian Hysteria";

    [Fact] public void Part1Sample() => ProcessPart1(GetData(InputType.Sample)).Should().Be(11);
    [Fact] public void Part1Input() => ProcessPart1(GetData(InputType.Input));
    
    [Fact] public void Part2Sample() => ProcessPart2(GetData(InputType.Sample)).Should().Be(31);
    [Fact] public void Part2Input() => ProcessPart2(GetData(InputType.Input));

    private (List<int> List1, List<int> List2) LoadLists(string inputData)
    {
        var lineRegex = new Regex(@"(?<num1>\d+)\s+(?<num2>\d+)");
        var data = inputData
            .ToLines(removeEmptyLines: true)
            .Select(line => lineRegex.Match(line).MapTo<NumberPair>())
            .ToList();
        WriteLine($"Loaded data with {data.Count} number pairs.");

        var list1 = data.Select(pair => pair.Num1).ToList();
        var list2 = data.Select(pair => pair.Num2).ToList();
        
        return (list1, list2);
    }
    
    public int ProcessPart1(string inputData)
    {
        var (list1, list2) = LoadLists(inputData);
        
        // match up lists smallest-to-smallest pair, up to the largest-to-largest pair, then find distance between each
        var list1Ordered = list1.Order().ToList();
        var list2Ordered = list2.Order().ToList();
        
        var totalPairs = list1.Count;
        
        var totalDistance = 0;
        for (var i = 0; i < totalPairs; i++)
        {
            var distance = Math.Abs(list1Ordered[i] - list2Ordered[i]);
            totalDistance += distance;
        }

        WriteLine($"Total distance: {totalDistance}");
        
        return totalDistance;
    }
    
    public long ProcessPart2(string inputData)
    {
        var (list1, list2) = LoadLists(inputData);
        
        // take each value in list 1, and multiply it by the number of time it occurs in list 2
        // to get a similarity score for each item, then return a sum of similarity scores
        
        var similarityScore = list1
            .Select(num => num * list2.Count(n => n == num))
            .Sum();
        
        WriteLine($"Similarity score: {similarityScore}");
        
        return similarityScore;
    }
    
    public record NumberPair(int Num1, int Num2);
    
}