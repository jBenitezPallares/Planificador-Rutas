using PathLab.Core.Structures;

namespace PathLab.Core.Algorithms;

/// <summary>
/// The outcome of searching a path between two points of a <see cref="Grid"/>.
/// </summary>
/// <remarks>
/// It is immutable: every value is set once, when the search ends. Comparing the
/// <see cref="Cost"/>, the <see cref="Path"/> length and the number of explored cells of
/// two results is how the algorithms are compared with each other.
/// </remarks>
public class SearchResult
{
    /// <summary>
    /// Whether a path from the start to the goal was found.
    /// </summary>
    /// <value>
    /// <c>false</c> when no route exists, or when the start or the goal is a wall.
    /// </value>
    public bool Found { get; }

    /// <summary>
    /// The cells of the path, in order, from the start to the goal (both included).
    /// </summary>
    /// <value>Empty when <see cref="Found"/> is <c>false</c>.</value>
    public IReadOnlyList<Point> Path { get; }

    /// <summary>
    /// The real cost of <see cref="Path"/>: the sum of the cost of every cell entered,
    /// with the diagonal factor applied. The start cell is not counted.
    /// </summary>
    /// <value>
    /// Zero when there is no path, or when the start and the goal are the same cell.
    /// It is the cost of the path that was found, which is only the lowest possible one
    /// for Dijkstra and A*; breadth-first search can return a cheaper-in-cells but costlier path.
    /// </value>
    public double Cost { get; }

    /// <summary>
    /// The cells the algorithm closed, in the order it took them from its queue.
    /// </summary>
    /// <value>
    /// It starts with the start cell and, when a path was found, ends with the goal.
    /// It also holds cells that are not part of <see cref="Path"/>: the gap between both lists
    /// is what shows that A* explores fewer cells than Dijkstra. It is empty when the search
    /// never started because the start or the goal is a wall.
    /// </value>
    public IReadOnlyList<Point> ExplorationOrder { get; }

    /// <summary>
    /// How long the search took, in whole milliseconds.
    /// </summary>
    /// <value>
    /// Small maps usually report zero because the search finishes in less than a millisecond.
    /// </value>
    public long ElapsedMs { get; }

    /// <summary>
    /// Creates a result with every value already known.
    /// </summary>
    /// <param name="found">Whether a path was found.</param>
    /// <param name="path">The cells from the start to the goal, empty when none was found.</param>
    /// <param name="cost">The cost of <paramref name="path"/>.</param>
    /// <param name="explorationOrder">The cells explored, in order.</param>
    /// <param name="ms">The elapsed time in milliseconds.</param>
    public SearchResult(bool found, IReadOnlyList<Point> path, double cost, IReadOnlyList<Point> explorationOrder, long ms)
    {
        Found = found;
        Path = path;
        Cost = cost;
        ExplorationOrder = explorationOrder;
        ElapsedMs = ms;
    }

    /// <summary>
    /// Creates the result of a search that found no path: empty path and zero cost.
    /// </summary>
    /// <param name="explored">The cells explored before giving up.</param>
    /// <param name="elapsedMs">The elapsed time in milliseconds.</param>
    /// <returns>A result with <see cref="Found"/> set to <c>false</c>.</returns>
    public static SearchResult NotFound(IReadOnlyList<Point> explored, long elapsedMs)
    {
        return new SearchResult(false, new List<Point>(), 0, explored, elapsedMs);
    }
}
