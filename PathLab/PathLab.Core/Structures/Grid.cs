namespace PathLab.Core.Structures;

/// <summary>
/// A rectangular map made of cells, each one with a <see cref="CellType"/>.
/// </summary>
/// <remarks>
/// Rows run along <see cref="Height"/> and columns along <see cref="Width"/>, and a
/// <see cref="Point"/> is always <c>(Row, Column)</c>. A new grid is entirely free.
/// </remarks>
public class Grid
{
    private readonly CellType[,] _grid;

    /// <summary>The number of columns.</summary>
    public int Width { get; }

    /// <summary>The number of rows.</summary>
    public int Height { get; }

    /// <summary>
    /// Creates a grid with every cell free.
    /// </summary>
    /// <param name="width">The number of columns.</param>
    /// <param name="height">The number of rows.</param>
    public Grid(int width, int height)
    {
        Width = width;
        Height = height;
        _grid = new CellType[height, width];
    }

    /// <summary>
    /// Gets the cells a search can move to from <paramref name="point"/>.
    /// </summary>
    /// <param name="point">The cell to move from.</param>
    /// <param name="mode">Four directions, or eight when diagonals are allowed.</param>
    /// <returns>A new list on every call. The order of the neighbors is not specified.</returns>
    /// <remarks>
    /// A neighbor is left out when it is:
    /// <list type="bullet">
    /// <item>outside the grid,</item>
    /// <item>a wall,</item>
    /// <item>
    /// a diagonal that cuts a corner: it is refused only when both cells beside the diagonal
    /// are walls. A single wall next to the diagonal still lets the move through.
    /// </item>
    /// </list>
    /// </remarks>
    public IEnumerable<Point> GetNeighbors(Point point, MovementMode mode)
    {
        List<Point> neighbors = new List<Point>();
        foreach ((int rowStep, int columnStep) in MovementModeExtensions.GetOffsets(mode))
        {
            Point candidate = new Point(point.Row + rowStep, point.Column + columnStep);
            if (Contains(candidate)
                && GetCellType(candidate) != CellType.Wall
                && !CutsCorner(point, rowStep, columnStep))
            {
                neighbors.Add(candidate);
            }
        }
        return neighbors;
    }

    /// <summary>
    /// Tells whether <paramref name="point"/> is inside the grid.
    /// </summary>
    /// <param name="point">The point to check.</param>
    /// <returns><c>true</c> when its row and its column are both in range.</returns>
    public bool Contains(Point point)
    {
        if (point.Row < 0 || point.Row >= Height || point.Column < 0 || point.Column >= Width)
            return false;
        return true;
    }

    private bool CutsCorner(Point point, int rowStep, int columnStep)
    {
        bool isDiagonal = rowStep != 0 && columnStep != 0;
        if (!isDiagonal)
            return false;

        Point verticalCorner = new Point(point.Row + rowStep, point.Column);
        Point horizontalCorner = new Point(point.Row, point.Column + columnStep);

        return GetCellType(verticalCorner) == CellType.Wall
               && GetCellType(horizontalCorner) == CellType.Wall;
    }

    private CellType GetCellType(Point point)
    {
        return _grid[point.Row, point.Column];
    }

    /// <summary>
    /// Sets the terrain of a cell.
    /// </summary>
    /// <param name="point">The cell to change.</param>
    /// <param name="cell">The new terrain.</param>
    /// <remarks>A point outside the grid is ignored: nothing changes and nothing is thrown.</remarks>
    public void SetCell(Point point, CellType cell)
    {
        if (Contains(point))
        {
            _grid[point.Row, point.Column] = cell;
        }
    }

    /// <summary>
    /// Gets the cost of moving from <paramref name="from"/> to the adjacent cell <paramref name="to"/>.
    /// </summary>
    /// <param name="from">The cell the move starts at. Its own cost is not charged.</param>
    /// <param name="to">The adjacent cell that is entered.</param>
    /// <returns>
    /// The cost of entering <paramref name="to"/>, multiplied by 1.414 when the move is a diagonal.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="to"/> is outside the grid or is a wall.
    /// </exception>
    public double StepCost(Point from, Point to)
    {
        bool isDiagonal = from.Row != to.Row && from.Column != to.Column;
        double multiplyFactor = MovementModeExtensions.OrthogonalFactor;

        if (isDiagonal)
        {
            multiplyFactor = MovementModeExtensions.DiagonalFactor;
        }

        return GetCost(to) * multiplyFactor;
    }

    /// <summary>
    /// Gets the cost of entering a cell: 1 for free ground, 5 for mud and 10 for water.
    /// </summary>
    /// <param name="point">The cell to enter.</param>
    /// <returns>The cost of the terrain of <paramref name="point"/>.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="point"/> is outside the grid, or is a wall (a wall cannot be entered, so it has no cost).
    /// </exception>
    public int GetCost(Point point)
    {
        EnsureContains(point);
        return CellTypeExtensions.GetCost(_grid[point.Row, point.Column]);
    }

    /// <summary>
    /// Tells whether a cell is a wall.
    /// </summary>
    /// <param name="point">The cell to check.</param>
    /// <returns><c>true</c> when the cell is a wall.</returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="point"/> is outside the grid.</exception>
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
