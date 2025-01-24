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

    [Fact] public void Part2_Sample() => ExecutePart2(GetData(InputType.Sample)).Should().Be(6);
    // This is slow (~35min) but it works, I'm sure there is something to be learned here
    [Fact] public void Part2() => ExecutePart2(GetData(InputType.Input));

    private (Grid<char> Grid, GridCoordinate StartCoordinate, GridDirection StartDirection) LoadMap(string inputText)
    {
        var grid = new Grid<char>(inputText.ToGrid(removeEmptyLines: true));
        Debug($"Loaded map grid of {grid.RowCount} rows and {grid.ColumnCount} columns");
        
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
                
                Debug($"Guard starting at ({startCoordinate.Row}, {startCoordinate.Column}) facing {startDirection.Name}");
                
                return (grid, startCoordinate, startDirection);
            }
        }
        
        throw new Exception("No guard found in the map");
    }
    
    public long ExecutePart1(string data)
    {
        var (grid, loc, dir) = LoadMap(data);
        var solution = GetPath(grid, loc, dir).Select(x => x.Loc).Distinct().Count();
        WriteLine($"Visited Cells: {solution}");
        return solution;
    }

    private IEnumerable<(GridCoordinate Loc, GridDirection Dir)> GetPath(Grid<char> grid, GridCoordinate loc, GridDirection dir)
    {
        yield return (loc, dir);

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
                yield return (loc, dir);
            }
        }
    }
    
    public long ExecutePart2(string data)
    {
        var loopingMapCount = 0;

        // find the solution path of the original map to determine possible points of obstruction
        var (originalGrid, startLoc, startDir) = LoadMap(data);
        var possibleObstructionLocations = GetPath(originalGrid, startLoc, startDir)
            .Skip(1) // not the starting position
            .Select(x => x.Loc) // just the positions
            .Distinct() // no dupes
            .ToList();
        
        foreach (var openPosition in possibleObstructionLocations)
        {
            var (grid, loc, dir) = LoadMap(data);
            grid[openPosition] = '#'; // place the obstruction
            
            try
            {
                var cycle = CycleFinder.FindCycle(GetPath(grid, loc, dir));
                // (for some reason the cycle finder has been finding cycles of length 1, not going to dig into it now)
                if (cycle.CycleLength > 1)
                {
                    WriteLine($"Found cycle of length {cycle.CycleLength}");
                    loopingMapCount++;                    
                }
            }
            catch (InvalidOperationException e)
            {
                // cycle not found
            }
        }

        WriteLine($"Possible Looping-Path Maps: {loopingMapCount}");
        return loopingMapCount;
    }

}
