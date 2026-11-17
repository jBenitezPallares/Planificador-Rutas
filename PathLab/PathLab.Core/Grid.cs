namespace PathLab.Core;

public class Grid
{
    private readonly int _width;
    private readonly int _height;
    private readonly CellType[,] _grid;

    public Grid(int width, int height)
    {
        _width = width;
        _height = height;
        _grid = new CellType[height, width];
    }

    public IEnumerable<Point> GetNeighbors(Point point, MovementMode mode)
    {
        List<Point> neighbors = new List<Point>();
        foreach ((int rowStep, int columnStep) in GetOffsets(mode))
        {
            Point candidate = new Point(point.Row + rowStep, point.Column + columnStep);
            if (Contains(candidate) && GetCellType(candidate) != CellType.Wall)
            {
                neighbors.Add(candidate);
            }
        }
        return neighbors;
    }

    private bool Contains(Point point)
    {
        if (point.Row < 0 || point.Row >= _height || point.Column < 0 || point.Column >= _width) 
            return false;
        return true;
    }

    private CellType GetCellType(Point point)
    {
        return _grid[point.Row, point.Column];
    }
    
    private static (int Row, int Column)[] GetOffsets(MovementMode mode) => mode switch
    {
        MovementMode.FourDirections => FourDirectionOffsets,
        MovementMode.EightDirections => EightDirectionOffsets,
        _ => throw new ArgumentOutOfRangeException(nameof(mode))
    };
    private static readonly (int row, int column)[] FourDirectionOffsets =
    {
        (0, 1), (1, 0), (0, -1), (-1, 0)
    };

    private static readonly (int row, int column)[] EightDirectionOffsets =
    {
        (-1, 0), (1, 0), (0, -1), (0, 1), (-1, -1), (-1, 1), (1, -1), (1, 1)
    };
}