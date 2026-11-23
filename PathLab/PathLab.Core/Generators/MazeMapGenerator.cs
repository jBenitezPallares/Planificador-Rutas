using PathLab.Core.Abstractions;
using PathLab.Core.Structures;

namespace PathLab.Core.Generators;

/// <summary>
/// Builds a maze with a recursive backtracker: a depth-first walk that carves corridors.
/// </summary>
/// <remarks>
/// Walls are cells of the grid, so the maze is drawn with the real cells on even rows and
/// even columns and the walls between them on odd ones. That is why both sizes must be odd,
/// and why the two opposite corners of the grid are real, free cells that are always connected.
/// <para>
/// The result is a perfect maze: every free cell can be reached from the corner and there is
/// exactly one path between any two of them, with no loops. With <c>cells = ((width + 1) / 2) *
/// ((height + 1) / 2)</c> real cells, the maze has <c>2 * cells - 1</c> free cells, which is
/// what the tests rely on to detect loops and isolated cells.
/// </para>
/// The walk uses an explicit stack instead of recursion, so big maps cannot overflow the call
/// stack. The randomness is injected, so the same seed always builds the same maze.
/// </remarks>
public class MazeMapGenerator : IMapGenerator
{
    private readonly Random _random;

    /// <summary>
    /// Creates a maze generator.
    /// </summary>
    /// <param name="random">The source of randomness that decides which corridor is carved next.</param>
    /// <exception cref="ArgumentNullException"><paramref name="random"/> is <c>null</c>.</exception>
    public MazeMapGenerator(Random random)
    {
        _random = random ?? throw new ArgumentNullException(nameof(random), "El generador de azar no puede ser nulo");
    }

    /// <summary>
    /// Generates a maze that starts at the top-left corner and reaches every real cell.
    /// </summary>
    /// <param name="width">Number of columns. It must be odd and at least 3.</param>
    /// <param name="height">Number of rows. It must be odd and at least 3.</param>
    /// <returns>A new grid where the walls and corridors of the maze are already placed.</returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="width"/> or <paramref name="height"/> is smaller than 3 or is even.
    /// </exception>
    public Grid Generate(int width, int height)
    {
        if (width < 3)
        {
            throw new ArgumentOutOfRangeException(nameof(width), width, "El ancho debe ser al menos 3");
        }
        if (height < 3)
        {
            throw new ArgumentOutOfRangeException(nameof(height), height, "El alto debe ser al menos 3");
        }

        if (width % 2 == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(width), width, "El ancho debe ser impar");
        }
        if (height % 2 == 0)
        {
            throw new ArgumentOutOfRangeException(nameof(height), height, "El alto debe ser impar");
        }
        
        Grid grid = new Grid(width, height);
        for (int row = 0; row < height; row++)
        for (int column = 0; column < width; column++)
            grid.SetCell(new Point(row, column), CellType.Wall);

        Point start = new Point(0, 0);
        grid.SetCell(start, CellType.Free);
        Stack<Point> stack = new Stack<Point>();
        stack.Push(start);

        while (stack.Count > 0)
        {
            Point current = stack.Peek();

            List<Point> candidates = GetCandidates(grid, current);

            if (candidates.Count > 0)
            {
                Point selected = candidates[_random.Next(candidates.Count)];
                Point middle = new Point((current.Row + selected.Row) / 2, (current.Column + selected.Column) / 2);
                grid.SetCell(middle, CellType.Free);     
                grid.SetCell(selected, CellType.Free);  
                stack.Push(selected);                   
            }
            else
            {
                stack.Pop();
            }
        }

        return grid;
    }

    private static List<Point> GetCandidates(Grid grid, Point current)
    {
        List<Point> candidates = new List<Point>();
        foreach ((int row, int column) in CandidateOffsets)
        {
            Point possibleCandidate = new Point(current.Row + row, current.Column + column);
            if (grid.Contains(possibleCandidate) && grid.IsWall(possibleCandidate))
            {
                candidates.Add(possibleCandidate);
            }
        }

        return candidates;
    }
    
    private static readonly (int Row, int Column)[] CandidateOffsets =
    {
        (-2, 0), (0, -2), (2, 0), (0, 2),
    };
}