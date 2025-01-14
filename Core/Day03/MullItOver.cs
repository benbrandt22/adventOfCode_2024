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

    [Fact] public void Part2_Sample() => ExecutePart2(GetData("sample2")).Should().Be(48);
    [Fact] public void Part2() => ExecutePart2(GetData(InputType.Input));

    public long ExecutePart1(string data)
    {
        var instructions = LoadInstructions(data);
        
        var solution = instructions
            .Where(x => x.Operation == Operation.Mul)
            .Sum(x => x.Result());
        
        WriteLine($"Solution: {solution}");
        return solution;
    }
    
    private class Instruction
    {
        public Instruction(string operation, long? param1 = null, long? param2 = null)
        {
            Operation = operation switch
            {
                "mul" => Operation.Mul,
                "do" => Operation.Do,
                "don't" => Operation.Dont,
                _ => throw new ArgumentOutOfRangeException(nameof(operation), operation, null)
            };
            Param1 = param1;
            Param2 = param2;
        }
        
        public Operation Operation { get; }
        public long? Param1 { get; }
        public long? Param2 { get; }

        public long Result()
        {
            if(Operation == Operation.Mul && Param1.HasValue && Param2.HasValue)
            {
                return Param1.Value * Param2.Value;
            }

            throw new InvalidOperationException();
        }
    }
    
    private enum Operation { Mul, Do, Dont }

    private List<Instruction> LoadInstructions(string inputData)
    {
        var instructionRegex = new Regex(@"(?<operation>mul|don't|do)\(((?<param1>\d+),(?<param2>\d+))?\)");
        var data = instructionRegex
            .Matches(inputData)
            .MapEachTo<Instruction>()
            .ToList();
        WriteLine($"Loaded data with {data.Count} instructions.");
        return data;
    }
    
    public long ExecutePart2(string data)
    {
        var instructions = LoadInstructions(data);

        long solution = 0;

        bool enabled = true;
        foreach (var i in instructions)
        {
            switch (i.Operation)
            {
                case Operation.Mul:
                    if (enabled)
                    {
                        solution += i.Result();
                    }
                    break;
                case Operation.Do:
                    enabled = true;
                    break;
                case Operation.Dont:
                    enabled = false;
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }
        
        WriteLine($"Solution: {solution}");
        return solution;
    }

}

