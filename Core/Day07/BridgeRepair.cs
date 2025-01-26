using System.Diagnostics;
using Core.Shared;
using Core.Shared.Extensions;
using Core.Shared.Modules;

namespace Core.Day07;

public class BridgeRepair : BaseDayModule
{
    public BridgeRepair(ITestOutputHelper outputHelper) : base(outputHelper) { }
    
    public override int Day => 7;
    public override string Title => "Bridge Repair";

    [Fact] public void Part1_Sample() => ExecutePart1(GetData(InputType.Sample)).Should().Be(3749);
    [Fact] public void Part1() => ExecutePart1(GetData(InputType.Input));

    [Fact] public void Part2_Sample() => ExecutePart2(GetData(InputType.Sample)).Should().Be(11387);
    [Fact] public void Part2() => ExecutePart2(GetData(InputType.Input));

    public long ExecutePart1(string data)
    {
        var equations = LoadData(data);
        
        List<Func<long, long, long>> AddOrMultiplyOperators = new()
        {
            (x, y) => x + y, // Add
            (x, y) => x * y, // Multiply
        };
        
        var solution = equations
            .Where(e => CanBeTrue(e, AddOrMultiplyOperators))
            .Sum(e => e.Solution);
        
        WriteLine($"Total of valid equations: {solution}");
        return solution;
    }

    private record CalibrationEquation(long Solution, List<long> Inputs);

    private List<CalibrationEquation> LoadData(string inputData)
    {
        var lines = inputData.ToLines(removeEmptyLines: true);
        var results = lines.Select(line =>
        {
            var parts = line.Split(": ");
            var solution = long.Parse(parts[0]);
            var inputs = parts[1]
                .Split(" ", StringSplitOptions.RemoveEmptyEntries)
                .Select(long.Parse).ToList();
            return new CalibrationEquation(solution, inputs);
        }).ToList();
        WriteLine($"Loaded {results.Count} calibration equations");
        return results;
    }

    private bool CanBeTrue(CalibrationEquation calibrationEquation, List<Func<long, long, long>> possibleOperators)
    {
        var workingSet = new HashSet<long> { calibrationEquation.Inputs[0] };
        foreach (var nextInput in calibrationEquation.Inputs.Skip(1))
        {
            var newSet = new HashSet<long>();
            foreach (var previousOutput in workingSet)
            {
                possibleOperators.ForEach(op => newSet.Add(op(previousOutput, nextInput)));
            }

            workingSet = new HashSet<long>(newSet);
        }

        if (workingSet.Contains(calibrationEquation.Solution))
        {
            return true;
        }

        return false;
    }
    
    public long ExecutePart2(string data)
    {
        var equations = LoadData(data);
        
        List<Func<long, long, long>> AddOrMultiplyOrConcatenate = new()
        {
            (x, y) => x + y, // Add
            (x, y) => x * y, // Multiply
            (x, y) => long.Parse($"{x}{y}") // Concatenate
        };
        
        var solution = equations
            .Where(e => CanBeTrue(e, AddOrMultiplyOrConcatenate))
            .Sum(e => e.Solution);
        
        WriteLine($"Total of valid equations: {solution}");
        return solution;
    }

}

