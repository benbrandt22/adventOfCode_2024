namespace Core.Shared;

/// <summary>
/// A basic grid structure based on a 2D array of rows & columns, with the 0,0 origin in the top left corner
/// </summary>
public class Grid<T>
{
    private readonly T[,] _array;

    public Grid(T[,] array)
    {
        _array = array;
    }
    
    public int RowCount => _array.GetLength(0);
    public int ColumnCount => _array.GetLength(1);
    
    public T ValueAt(GridCoordinate coordinate) => _array[coordinate.Row, coordinate.Column];

    public IEnumerable<GridCoordinate> AllCoordinates()
    {
        var totalRows = RowCount;
        var totalColumns = ColumnCount;
        for (var row = 0; row < totalRows; row++)
        {
            for (var column = 0; column < totalColumns; column++)
            {
                yield return new GridCoordinate(row, column);
            }
        }
    }
    
}

public record GridCoordinate(int Row, int Column)
{
    public GridCoordinate Move(GridDirection direction, int distance = 1) =>
        new(Row + (direction.DRow * distance), Column + (direction.DCol * distance));
        
    public bool IsInBounds<T>(Grid<T> grid)
    {
        return Row >= 0 && Row < grid.RowCount && Column >= 0 && Column < grid.ColumnCount;
    }
}

public record GridDirection(int DRow, int DCol)
{
    public static readonly GridDirection Up = new(-1, 0);
    public static readonly GridDirection Down = new(1, 0);
    public static readonly GridDirection Left = new(0, -1);
    public static readonly GridDirection Right = new(0, 1);
    public static readonly GridDirection UpLeft = new(-1, -1);
    public static readonly GridDirection UpRight = new(-1, 1);
    public static readonly GridDirection DownLeft = new(1, -1);
    public static readonly GridDirection DownRight = new(1, 1);

    public static readonly IReadOnlyList<GridDirection> AllDirections = new[]
        { Up, Down, Left, Right, UpLeft, UpRight, DownLeft, DownRight };
}