using System.Diagnostics;
using Core.Shared;
using Core.Shared.Extensions;
using Core.Shared.Modules;

namespace Core.Day08;

public class ResonantCollinearity : BaseDayModule
{
    public ResonantCollinearity(ITestOutputHelper outputHelper) : base(outputHelper) { }
    
    public override int Day => 8;

    public override string Title => "Resonant Collinearity";

    [Fact] public void Part1_Sample() => ExecutePart1(GetData(InputType.Sample)).Should().Be(14);
    [Fact] public void Part1() => ExecutePart1(GetData(InputType.Input));

    [Fact, ShowDebug] public void Part2_Sample() => ExecutePart2(GetData(InputType.Sample)).Should().Be(34);
    [Fact, ShowDebug] public void Part2() => ExecutePart2(GetData(InputType.Input));

    public long ExecutePart1(string data)
    {
        var (grid, antennas) = LoadAntennaMap(data);

        var allAntennaPairs = antennas
            .GroupBy(a => a.Frequency)
            .Select(freqGroup => GetAllPairs(freqGroup.ToList()))
            .SelectMany(x => x.Select(y => y))
            .ToList();

        GridCoordinate GetAntiNode(GridCoordinate c1, GridCoordinate c2)
        {
            var dx = c2.Column - c1.Column;
            var dy = c2.Row - c1.Row;
            return new GridCoordinate(c1.Column - dx, c1.Row - dy);
        }
        
        var antiNodeCoordinates = new List<GridCoordinate>();
        foreach (var pair in allAntennaPairs)
        {
            antiNodeCoordinates.Add(GetAntiNode(pair.Item1.Coordinate, pair.Item2.Coordinate));
            antiNodeCoordinates.Add(GetAntiNode(pair.Item2.Coordinate, pair.Item1.Coordinate));
        }

        var uniqueAntinodesWithinMap = antiNodeCoordinates
            .Where(c => c.IsInBounds(grid))
            .Distinct()
            .ToList();
        
        var solution = uniqueAntinodesWithinMap.Count;
        WriteLine($"Unique Antinode Locations within the Map: {solution}");
        return solution;
    }
    
    private record Antenna(char Frequency, GridCoordinate Coordinate);
    
    private (Grid<char> Grid, List<Antenna> Antennas) LoadAntennaMap(string inputData)
    {
        var grid = new Grid<char>(inputData.ToGrid(removeEmptyLines: true));
        
        var antennas = new List<Antenna>();
        foreach (var coordinate in grid.AllCoordinates())
        {
            var cellValue = grid[coordinate];
            if (cellValue != '.')
            {
                antennas.Add(new Antenna(cellValue, coordinate));
            }
        }
        
        WriteLine($"Loaded antenna map grid {grid.ColumnCount} x {grid.RowCount} with {antennas.Count} antennas");
        return (grid, antennas);
    }

    private IList<Tuple<T, T>> GetAllPairs<T>(IList<T> items)
    {
        ArgumentOutOfRangeException.ThrowIfLessThan(items.Count, 2);
        var results = new List<Tuple<T, T>>();
        for (int a = 0; a < (items.Count-1); a++)
        {
            var item1 = items[a];
            for (int b = a+1; b < items.Count; b++)
            {
                var item2 = items[b];
                results.Add(new(item1, item2));
            }
        }

        return results;
    }
    
    public long ExecutePart2(string data)
    {
        var (grid, antennas) = LoadAntennaMap(data);
        
        Debug(grid.Visualize());
        Debug(new string('-', grid.ColumnCount + 5));

        var allAntennaPairs = antennas
            .GroupBy(a => a.Frequency)
            .Select(freqGroup => GetAllPairs(freqGroup.ToList()))
            .SelectMany(x => x.Select(y => y))
            .ToList();

        List<GridCoordinate> GetPointsOnLineInGrid(GridCoordinate c1, GridCoordinate c2)
        {
            var linePoints = new List<GridCoordinate>();
            var dx = c2.Column - c1.Column;
            var dy = c2.Row - c1.Row;
            var m = (double)dy / (double)dx; // slope
            var b = c1.Row - (m * c1.Column); // y-intercept
            for (int x = 0; x < grid.ColumnCount; x++)
            {
                var y = (m * x) + b;
                var isInteger = Math.Abs(Math.Round(y) - y) < 0.00001d;
                if (isInteger)
                {
                    // falls exactly on a grid coordinate
                    var coord = new GridCoordinate((int)Math.Round(y), x);
                    if (coord.IsInBounds(grid))
                    {
                        linePoints.Add(coord);
                    }
                }
                
            }
            return linePoints;
        }

        var antinodeCoordinates = new List<GridCoordinate>();
        foreach (var pair in allAntennaPairs)
        {
            var linePoints = GetPointsOnLineInGrid(pair.Item1.Coordinate, pair.Item2.Coordinate);
            antinodeCoordinates.AddRange(linePoints);
        }

        var uniqueAntinodes = antinodeCoordinates.Distinct().ToList();
        
        // visualize
        var vizGrid = new Grid<char>(data.ToGrid(removeEmptyLines: true));
        uniqueAntinodes.ForEach(c =>
        {
            if (vizGrid[c] == '.')
            {
                vizGrid[c] = '#';
            }
        });
        Debug(vizGrid.Visualize());
        
        
        var solution = uniqueAntinodes.Count;
        WriteLine($"Unique Antinode Locations within the Map: {solution}");
        return solution;
    }

}

