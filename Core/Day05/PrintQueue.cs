using System.Diagnostics;
using Core.Shared;
using Core.Shared.Extensions;
using Core.Shared.Modules;

namespace Core.Day05;

public class PrintQueue : BaseDayModule
{
    public PrintQueue(ITestOutputHelper outputHelper) : base(outputHelper) { }
    
    public override int Day => 5;
    public override string Title => "Print Queue";

    [Fact] public void Part1_Sample() => ExecutePart1(GetData(InputType.Sample)).Should().Be(143);
    [Fact] public void Part1() => ExecutePart1(GetData(InputType.Input));

    [Fact] public void Part2_Sample() => ExecutePart2(GetData(InputType.Sample)).Should().Be(123);
    [Fact] public void Part2() => ExecutePart2(GetData(InputType.Input));

    private record PageOrderingRule(int Before, int After);

    private record Update(List<int> PageNumbers);

    private class PrintJob
    {
        public List<PageOrderingRule> PageOrderingRules { get; }
        public List<Update> Updates { get; }

        public PrintJob(List<PageOrderingRule> pageOrderingRules, List<Update> updates)
        {
            PageOrderingRules = pageOrderingRules;
            Updates = updates;
        }
        
    }

    private PrintJob LoadPrintJob(string input)
    {
        var sections = input.ToParagraphs();
        var rules = sections[0].ToLines(true)
            .Select(line => line.Split("|").Select(int.Parse).ToArray())
            .Select(x => new PageOrderingRule(x[0], x[1]))
            .ToList();

        var updates = sections[1].ToLines(true)
            .Select(line => line.Split(",").Select(int.Parse).ToList())
            .Select(pageList => new Update(pageList))
            .ToList();
        
        var printJob = new PrintJob(rules, updates);
        WriteLine($"Loaded PrintJob with {printJob.PageOrderingRules.Count} rules and {printJob.Updates.Count} updates.");
        return printJob;
    }
    
    public long ExecutePart1(string data)
    {
        var printJob = LoadPrintJob(data);

        var correctlyOrderedUpdates = printJob.Updates
            .Where(update => UpdateFollowsRules(update, printJob.PageOrderingRules))
            .ToList();
        WriteLine($"Found {correctlyOrderedUpdates.Count} correctly ordered updates.");

        var sumOfMiddlePageNumbers = correctlyOrderedUpdates
            .Select(GetMiddlePageNumber)
            .Sum();

        var solution = sumOfMiddlePageNumbers;
        WriteLine($"Solution: {solution}");
        return solution;
    }

    private bool UpdateFollowsRules(Update update, List<PageOrderingRule> pageRules)
    {
        // "within each update, the ordering rules that involve missing page numbers are not used."
        var applicableRules = pageRules
            .Where(r => update.PageNumbers.Contains(r.Before) && update.PageNumbers.Contains(r.After)).ToList();
        return applicableRules.All(rule => UpdateFollowsRule(update, rule));
    }
    
    private bool UpdateFollowsRule(Update update, PageOrderingRule pageRule)
    {
        // does the "before" page actually come before the "after" page?
        return update.PageNumbers.IndexOf(pageRule.Before) < update.PageNumbers.IndexOf(pageRule.After);
    }

    private int GetMiddlePageNumber(Update update)
    {
        // (assumes there are an odd number of values)
        var middleIndex = (update.PageNumbers.Count - 1) / 2;
        return update.PageNumbers[middleIndex];
    }
    
    public long ExecutePart2(string data)
    {
        var printJob = LoadPrintJob(data);
        
        var incorrectlyOrderedUpdates = printJob.Updates
            .Where(update => !UpdateFollowsRules(update, printJob.PageOrderingRules))
            .ToList();
        WriteLine($"Found {incorrectlyOrderedUpdates.Count} incorrectly ordered updates.");

        var fixedUpdates = incorrectlyOrderedUpdates
            .Select(x => SortUpdate(x, printJob.PageOrderingRules))
            .ToList();

        var sumOfMiddlePageNumbers = fixedUpdates
            .Select(GetMiddlePageNumber)
            .Sum();

        var solution = sumOfMiddlePageNumbers;
        WriteLine($"Solution: {solution}");
        return solution;
    }

    private Update SortUpdate(Update badUpdate, List<PageOrderingRule> pageOrderingRules)
    {
        var values = new List<int>(badUpdate.PageNumbers);
        var comparer = new RulesComparer(pageOrderingRules);
        values.Sort(comparer);
        return new Update(values);
    }

    private class RulesComparer : IComparer<int>
    {
        private readonly List<PageOrderingRule> _rules;

        public RulesComparer(List<PageOrderingRule> rules)
        {
            _rules = rules;
        }

        public int Compare(int x, int y)
        {
            var rule = _rules.SingleOrDefault(r => r.Before == x && r.After == y);
            if (rule != null)
            {
                return -1;
            }
            else
            {
                return 1;
            }
        }
    }

}

