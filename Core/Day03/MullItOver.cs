using System.Diagnostics;
using System.Text.RegularExpressions;
using Core.Shared;
using Core.Shared.Extensions;
using Core.Shared.Modules;

namespace Core.Day03;

public class MullItOver : BaseDayModule
{
    public MullItOver(ITestOutputHelper outputHelper) : base(outputHelper) { }
    
    public override int Day => 3;
    public override string Title => "Mull It Over";

    [Fact] public void Part1_Sample() => ExecutePart1(GetData(InputType.Sample)).Should().Be(161);
    [Fact] public void Part1() => ExecutePart1(GetData(InputType.Input));

    [Fact(Skip = "Not yet implemented")][ShowDebug] public void Part2_Sample() => ExecutePart2(GetData(InputType.Sample)).Should().Be(-1);
    [Fact(Skip = "Not yet implemented")] public void Part2() => ExecutePart2(GetData(InputType.Input));

    private class Mul
    {
        public long Factor1 { get; }
        public long Factor2 { get; }

        public Mul(long factor1, long factor2)
        {
            Factor1 = factor1;
            Factor2 = factor2;
        }
        
        public long Product => Factor1 * Factor2;
    }
    
    private List<Mul> LoadInstructions(string inputData)
    {
        var multiplicationInstructionRegex = new Regex(@"mul\((?<factor1>\d+),(?<factor2>\d+)\)");
        var data = multiplicationInstructionRegex
            .Matches(inputData)
            .MapEachTo<Mul>()
            .ToList();
        WriteLine($"Loaded data with {data.Count} multiplication instructions.");
        return data;
    }
    
    public long ExecutePart1(string data)
    {
        var instructions = LoadInstructions(data);
        
        var solution = instructions.Sum(x => x.Product);
        WriteLine($"Solution: {solution}");
        return solution;
    }
    
    public long ExecutePart2(string data)
    {
        WriteLine($"Part 2 - Loaded Data");

        var solution = 0;
        WriteLine($"Solution: {solution}");
        return solution;
    }

}

