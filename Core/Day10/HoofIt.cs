using System.Diagnostics;
using Core.Shared;
using Core.Shared.Extensions;
using Core.Shared.Modules;

namespace Core.Day10;

public class HoofIt : BaseDayModule
{
    public HoofIt(ITestOutputHelper outputHelper) : base(outputHelper)
    {
    }

    public override int Day => 10;

    public override string Title => "Hoof It";

    [Fact] public void Part1_Sample() => ExecutePart1(GetData(InputType.Sample)).Should().Be(36);

    [Fact] public void Part1() => ExecutePart1(GetData(InputType.Input));

    [Fact] public void Part2_Sample() => ExecutePart2(GetData(InputType.Sample)).Should().Be(81);

    [Fact] public void Part2() => ExecutePart2(GetData(InputType.Input));

    public long ExecutePart1(string data)
    {
        var map = LoadTopographicMap(data);
        var possibleTrailheads = GetPossibleTrailheads(map);

        var trails = FindAllTrails(map, possibleTrailheads);

        var solution = 0;
        var groupedTrailsByStart = trails
            .GroupBy(t => t.First());
        foreach (var grouping in groupedTrailsByStart)
        {
            var start = grouping.Key;
            // score is the total end-points that the trail head can reach
            var trailheadScore = grouping.Select(t => t.Last()).Distinct().Count();
            WriteLine($"Trailhead [r{start.Row}, c{start.Column}] can reach {trailheadScore} end-points");
            solution += trailheadScore;
        }

        WriteLine($"Total Trail Score: {solution}");
        return solution;
    }

    private List<GridCoordinate> GetPossibleTrailheads(Grid<int> map)
    {
        var trailheads = map.AllCoordinates()
            .Select(coord => (coord, value: map[coord]))
            .Where(x => x.value == 0)
            .Select(x => x.coord)
            .ToList();

        return trailheads;
    }

    private Grid<int> LoadTopographicMap(string inputData)
    {
        var grid = new Grid<int>(inputData.ToIntegerGrid(removeEmptyLines: true));
        WriteLine($"Loaded map grid of {grid.RowCount} rows and {grid.ColumnCount} columns");
        return grid;
    }

    private List<List<GridCoordinate>> FindAllTrails(Grid<int> map, List<GridCoordinate> possibleTrailheads)
    {
        var trails = new List<List<GridCoordinate>>();
        // load up trailheads to start
        foreach (var trailhead in possibleTrailheads)
        {
            var trail = new List<GridCoordinate> { trailhead };
            trails.Add(trail);
        }
        
        var possibleDirections = new List<GridDirection> { GridDirection.Up, GridDirection.Down, GridDirection.Left, GridDirection.Right };
        
        // walk each trail and branch as needed (create new trails for each possible branch)

        for (int elevation = 0; elevation < 9; elevation++)
        {
            var nextElevation = (elevation + 1);
            var nextTrails = new List<List<GridCoordinate>>();
            foreach (var trail in trails)
            {
                var currentPos = trail.Last();
                
                var possibleNextPositions = possibleDirections
                    .Select(dir => currentPos.Move(dir, 1))
                    .Where(pos => pos.IsInBounds(map) && map[pos] == nextElevation)
                    .ToList();
                
                foreach (var nextPosition in possibleNextPositions)
                { 
                    var newTrail = new List<GridCoordinate>(trail) { nextPosition };
                    nextTrails.Add(newTrail);
                }
            }

            trails = nextTrails;
        }
        
        return trails;
    }
    
    public long ExecutePart2(string data)
    {
        var map = LoadTopographicMap(data);
        var possibleTrailheads = GetPossibleTrailheads(map);

        var trails = FindAllTrails(map, possibleTrailheads);

        var solution = trails.Count();
        WriteLine($"Total Trail Rating (unique trails): {solution}");
        return solution;
    }

}

