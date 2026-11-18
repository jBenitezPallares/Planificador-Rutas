namespace PathLab.Core;

public class Grid
{
    private readonly CellType[,] _grid;

    public int Width { get; }
    public int Height { get; }

    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
        _grid = new CellType[height, width];
    }

    public IEnumerable<Point> GetNeighbors(Point point, MovementMode mode)
    {
        List<Point> neighbors = new List<Point>();
        foreach ((int rowStep, int columnStep) in MovementModeExtensions.GetOffsets(mode))
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
        if (point.Row < 0 || point.Row >= Height || point.Column < 0 || point.Column >= Width)
            return false;
        return true;
    }

    private CellType GetCellType(Point point)
    {
        return _grid[point.Row, point.Column];
    }

    public void SetCell(Point point, CellType cell)
    {
        if (Contains(point))
        {
            _grid[point.Row, point.Column] = cell;
        }
    }

    public int GetCost(Point point)
    {
        EnsureContains(point);
        return CellTypeExtensions.GetCost(_grid[point.Row, point.Column]);
    }

    public bool IsWall(Point point)
    {
        EnsureContains(point);
        return _grid[point.Row, point.Column] == CellType.Wall;
    }

    private void EnsureContains(Point point)
    {
        if (!Contains(point))
        {
            throw new ArgumentOutOfRangeException(nameof(point), point, "El punto está fuera de la grilla");
        }
    }
}