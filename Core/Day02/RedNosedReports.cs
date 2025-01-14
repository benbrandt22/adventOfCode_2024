using System.Diagnostics;
using Core.Shared;
using Core.Shared.Extensions;
using Core.Shared.Modules;
using Xunit.Sdk;

namespace Core.Day02;

public class RedNosedReports : BaseDayModule
{
    public RedNosedReports(ITestOutputHelper outputHelper) : base(outputHelper) { }
    
    public override int Day => 2;
    public override string Title => "Red-Nosed Reports";

    [Fact] public void Part1_Sample() => ExecutePart1(GetData(InputType.Sample)).Should().Be(2);
    [Fact] public void Part1() => ExecutePart1(GetData(InputType.Input));

    [Fact] public void Part2_Sample() => ExecutePart2(GetData(InputType.Sample)).Should().Be(4);
    [Fact] public void Part2() => ExecutePart2(GetData(InputType.Input));

    class Report
    {
        public List<int> Items { get; }

        public Report(List<int> items)
        {
            Items = items;
        }

        public bool IsSafe()
        {
            var diffs = new List<int>();
            for (int i = 1; i < Items.Count; i++)
            {
                var diff = Items[i] - Items[i - 1];
                diffs.Add(diff);
            }

            bool allSameDirection = diffs.All(x => x > 0) || diffs.All(x => x < 0);
            bool allSmall = diffs.All(x => Math.Abs(x) >= 1 && Math.Abs(x) <= 3);
            
            return allSameDirection && allSmall;
        }

        public bool IsSafeWithProblemDampener()
        {
            // already safe as-is?
            if (IsSafe())
            {
                return true;
            }
            
            // try removing one item and check if the report becomes safe
            for (int i = 0; i < Items.Count; i++)
            {
                var oneRemovedList = new List<int>(Items);
                oneRemovedList.RemoveAt(i);
                var dampenedReport = new Report(oneRemovedList);
                if (dampenedReport.IsSafe())
                {
                    return true;
                }
            }
            
            // did not find a safe variant
            return false;
        }
    }

    private List<Report> GetData(string input)
    {
        var lines = input.ToLines(removeEmptyLines: true);
        var result = new List<Report>();
        foreach (var line in lines)
        {
            var report = new Report(line.Split(' ').Select(int.Parse).ToList());
            result.Add(report);
        }
        WriteLine($"Loaded data with {result.Count} reports.");
        return result;
    }
    
    public long ExecutePart1(string data)
    {
        var reports = GetData(data);

        var solution = reports.Count(r => r.IsSafe());
        WriteLine($"Safe Reports: {solution}");
        return solution;
    }
    
    public long ExecutePart2(string data)
    {
        var reports = GetData(data);

        var solution = reports.Count(r => r.IsSafeWithProblemDampener());
        WriteLine($"Safe Reports with Problem Dampener: {solution}");
        return solution;
    }

}

