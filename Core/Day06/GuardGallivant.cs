using Core.Shared;
using Core.Shared.Extensions;
using Core.Shared.Modules;

namespace Core.Day06;

public class GuardGallivant : BaseDayModule
{
    public GuardGallivant(ITestOutputHelper outputHelper) : base(outputHelper) { }
    
    public override int Day => 6;
    public override string Title => "Guard Gallivant";

    [Fact] public void Part1_Sample() => ExecutePart1(GetData(InputType.Sample)).Should().Be(41);
    [Fact] public void Part1() => ExecutePart1(GetData(InputType.Input));

    [Fact(Skip = "Not yet implemented")][ShowDebug] public void Part2_Sample() => ExecutePart2(GetData(InputType.Sample)).Should().Be(-1);
    [Fact(Skip = "Not yet implemented")] public void Part2() => ExecutePart2(GetData(InputType.Input));

    private (Grid<char> Grid, GridCoordinate StartCoordinate, GridDirection StartDirection) LoadMap(string inputText)
    {
        var grid = new Grid<char>(inputText.ToGrid(removeEmptyLines: true));
        WriteLine($"Loaded map grid of {grid.RowCount} rows and {grid.ColumnCount} columns");
        
        // find the guard starting position & direction
        var guards = new Dictionary<char, GridDirection>()
        {
            { '^', GridDirection.Up },
            { '>', GridDirection.Right },
            { '<', GridDirection.Left },
            { 'v', GridDirection.Down }
        };
        
        foreach (var coordinate in grid.AllCoordinates())
        {
            var valueHere = grid.ValueAt(coordinate);
            if (guards.Keys.Contains(valueHere))
            {
                var startCoordinate = coordinate;
                var startDirection = guards[valueHere];
                
                WriteLine($"Guard starting at ({startCoordinate.Row}, {startCoordinate.Column}) facing {startDirection.Name}");
                
                return (grid, startCoordinate, startDirection);
            }
        }
        
        throw new Exception("No guard found in the map");
    }
    
    public long ExecutePart1(string data)
    {
        var solution = GetVisitedPositions(data).Distinct().Count();
        WriteLine($"Visited Cells: {solution}");
        return solution;
    }

    private IEnumerable<GridCoordinate> GetVisitedPositions(string data)
    {
        var (grid, loc, dir) = LoadMap(data);

        yield return loc;

        bool IsLeaving()
        {
            var nextLoc = loc.Move(dir);
            return !nextLoc.IsInBounds(grid);
        }
        
        bool IsBlocked()
        {
            var nextLoc = loc.Move(dir);
            var nextValue = grid.ValueAt(nextLoc);
            return nextValue == '#';
        }

        while (!IsLeaving())
        {
            if (IsBlocked())
            {
                // turn right
                dir = dir.Name switch
                {
                    GridDirection.DirectionName.Up => GridDirection.Right,
                    GridDirection.DirectionName.Right => GridDirection.Down,
                    GridDirection.DirectionName.Down => GridDirection.Left,
                    GridDirection.DirectionName.Left => GridDirection.Up,
                    _ => throw new Exception("Invalid direction")
                };
            }
            else
            {
                // move forward
                loc = loc.Move(dir);
                yield return loc;
            }
        }
    }
    
    public long ExecutePart2(string data)
    {
        WriteLine($"Part 2 - Loaded Data");

        var solution = 0;
        WriteLine($"Solution: {solution}");
        return solution;
    }

}

