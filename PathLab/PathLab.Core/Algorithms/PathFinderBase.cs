using System.Diagnostics;
using PathLab.Core.Algorithms;
using PathLab.Core.Structures;

namespace PathLab.Core.Abstractions;

/// <summary>
/// The common skeleton of every path finder: it checks the points, starts the clock and
/// leaves the actual search to each algorithm.
/// </summary>
/// <remarks>
/// This is the Template Method pattern. <see cref="Find"/> is fixed and does what all the
/// algorithms share, then calls <see cref="Search"/>, which each subclass implements with its own
/// loop (queue for breadth-first search, priority queue for A*). A subclass cannot skip the
/// validation or forget the timing, because it never controls the order of the steps.
/// </remarks>
public abstract class PathFinderBase : IPathFinder
{
    /// <summary>The movement mode this finder uses, fixed when it is created.</summary>
    protected readonly MovementMode MovementMode;

    /// <summary>
    /// Creates a path finder that moves in the given way.
    /// </summary>
    /// <param name="movementMode">Four directions, or eight when diagonals are allowed.</param>
    protected PathFinderBase(MovementMode movementMode)
    {
        MovementMode = movementMode;
    }

    /// <inheritdoc />
    /// <remarks>
    /// The clock starts after the points are checked, so it measures the search and not the
    /// validation. When the start or the goal is a wall there is nothing to search, and the
    /// result comes back with an empty exploration order.
    /// </remarks>
    public SearchResult Find(Grid grid, Point start, Point goal)
    {
        if (!grid.Contains(start))
            throw new ArgumentOutOfRangeException(nameof(start), start, "El punto de inicio está fuera de la grilla");

        if (!grid.Contains(goal))
            throw new ArgumentOutOfRangeException(nameof(goal), goal, "El punto de destino está fuera de la grilla");

        Stopwatch stopwatch = Stopwatch.StartNew();

        if (grid.IsWall(start) || grid.IsWall(goal))
            return SearchResult.NotFound(new List<Point>(), stopwatch.ElapsedMilliseconds);

        return Search(grid, start, goal, stopwatch);
    }

    /// <summary>
    /// The part of the search that is specific to each algorithm.
    /// </summary>
    /// <param name="grid">The map to search on.</param>
    /// <param name="start">The first cell of the path. It is inside the grid and is not a wall.</param>
    /// <param name="goal">The last cell of the path. It is inside the grid and is not a wall.</param>
    /// <param name="stopwatch">The clock already running, to read the elapsed time of the result.</param>
    /// <returns>The result of the search, with a path or with <c>Found</c> set to <c>false</c>.</returns>
    protected abstract SearchResult Search(Grid grid, Point start, Point goal, Stopwatch stopwatch);

    /// <summary>
    /// Sets every cell of the matrix to infinity, meaning "not reached yet".
    /// </summary>
    /// <param name="distance">The matrix of costs, indexed by row and column.</param>
    protected static void FillWithInfinity(double[,] distance)
    {
        for (int row = 0; row < distance.GetLength(0); row++)
        for (int column = 0; column < distance.GetLength(1); column++)
            distance[row, column] = double.PositiveInfinity;
    }

    /// <summary>
    /// Rebuilds the path by walking backwards from the goal through the cell each one came from.
    /// </summary>
    /// <param name="previous">For every cell, the cell it was reached from. The start has none.</param>
    /// <param name="goal">The cell to walk back from.</param>
    /// <returns>The cells in order, from the start to the goal.</returns>
    protected static List<Point> BuildPath(Point?[,] previous, Point goal)
    {
        List<Point> path = new List<Point>();
        Point? current = goal;

        while (current != null)
        {
            path.Add(current.Value);
            current = previous[current.Value.Row, current.Value.Column];
        }

        path.Reverse();
        return path;
    }
}
