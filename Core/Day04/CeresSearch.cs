using Core.Shared;
using Core.Shared.Extensions;
using Core.Shared.Modules;

namespace Core.Day04;

public class CeresSearch : BaseDayModule
{
    public CeresSearch(ITestOutputHelper outputHelper) : base(outputHelper) { }
    
    public override int Day => 4;
    public override string Title => "Ceres Search";

    [Fact] public void Part1_Sample() => ExecutePart1(GetData(InputType.Sample)).Should().Be(18);
    [Fact] public void Part1() => ExecutePart1(GetData(InputType.Input));

    [Fact] public void Part2_Sample() => ExecutePart2(GetData(InputType.Sample)).Should().Be(9);
    [Fact] public void Part2() => ExecutePart2(GetData(InputType.Input));

    private Grid<char> LoadGrid(string inputText)
    {
        var grid = new Grid<char>(inputText.ToGrid(removeEmptyLines: true));
        WriteLine($"Loaded word search grid of {grid.RowCount} rows and {grid.ColumnCount} columns");
        return grid;
    }
    
    public long ExecutePart1(string data)
    {
        var grid = LoadGrid(data);

        var solution = FindTotalOccurrencesOfWord("XMAS", grid);
        WriteLine($"Total Occurrences of XMAS: {solution}");
        return solution;
    }

    private int FindTotalOccurrencesOfWord(string word, Grid<char> grid)
    {
        var allowedDirections = GridDirection.AllDirections;

        var foundCount = 0;
        
        foreach (var wordStartCoordinate in grid.AllCoordinates())
        {
            // get lines in each direction, see if they're in bounds and match the word
            foreach (var direction in allowedDirections)
            {
                var lettersInDirection = Enumerable.Range(0, word.Length) // i.e. (0,1,2,3) for a 4-letter word
                    .Select(dist => wordStartCoordinate.Move(direction, dist)) // now we have potential coordinates
                    .Where(c => c.IsInBounds(grid)) // only use what's in bounds of the grid
                    .Select(c => grid.ValueAt(c)) // and get the characters at each coordinate
                    .ToArray();
                var wordInDirection = new string(lettersInDirection);
                if (wordInDirection == word)
                {
                    // we found a match
                    Debug($"Found {word} starting at ({wordStartCoordinate.Row}, {wordStartCoordinate.Column})");
                    foundCount++;
                }
            }
        }
        
        return foundCount;
    }
    
    public long ExecutePart2(string data)
    {
        var grid = LoadGrid(data);

        var solution = FindTotalXShapedMasArrangements(grid);
        WriteLine($"Total X-MAS arrangements: {solution}");
        return solution;
    }
    
    private int FindTotalXShapedMasArrangements(Grid<char> grid)
    {
        var foundCount = 0;
        
        foreach (var centerCoordinate in grid.AllCoordinates())
        {
            // check each coordinate as if it's the center of the X
            
            // if the center is not an A, move on to the next coordinate
            if (grid.ValueAt(centerCoordinate) != 'A')
            {
                continue;
            }
            
            // knowing the center is an 'A', check the four adjacent corners
            var cornerDirections = new[]
            {
                GridDirection.UpLeft, GridDirection.UpRight,
                GridDirection.DownLeft, GridDirection.DownRight
            };
            var cornerValuesAsChars = cornerDirections
                .Select(dir => centerCoordinate.Move(dir)) // get four corner coordinates
                .Where(coord => coord.IsInBounds(grid)) // only those in bounds
                .Select(coord => grid.ValueAt(coord))
                .ToArray();
            var cornerValues = new string(cornerValuesAsChars);
            // valid arrangements have 2 M's on one side and 2 S's on the other side (M's can't be on opposite corners)
            var validArrangements = new[]
            {
                "MMSS", // both M's on top
                "SMSM", // both M's on right
                "SSMM", // both M's on bottom
                "MSMS" // both M's on left
            };
            if (validArrangements.Contains(cornerValues))
            {
                Debug($"Found an X-MAS centered at ({centerCoordinate.Row}, {centerCoordinate.Column})");
                foundCount++;
            }
        }
        
        return foundCount;
    }

}

