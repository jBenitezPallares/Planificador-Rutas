using PathLab.Core.Algorithms;
using PathLab.Core.Structures;

namespace PathLab.Core.Abstractions;

/// <summary>
/// Finds a path between two points of a <see cref="Grid"/>.
/// </summary>
/// <remarks>
/// The movement mode (four or eight directions) is chosen when the finder is created,
/// not when it searches, so one instance always behaves the same way.
/// Implementations differ in what they optimize:
/// <list type="bullet">
/// <item>Breadth-first search returns the path with the fewest cells, which can cost more than another route.</item>
/// <item>Dijkstra and A* return the path with the lowest cost.</item>
/// </list>
/// A search only reads the grid; it never modifies it.
/// </remarks>
public interface IPathFinder
{
    /// <summary>
    /// Searches for a path from <paramref name="start"/> to <paramref name="goal"/>.
    /// </summary>
    /// <param name="grid">The map to search on.</param>
    /// <param name="start">The first cell of the path.</param>
    /// <param name="goal">The last cell of the path.</param>
    /// <returns>
    /// A result with <c>Found = false</c> and an empty path when no route exists, or when the
    /// start or the goal is a wall. An unreachable goal is an expected answer, so it never throws.
    /// When a path is found it includes both the start and the goal. Its cost adds the cost of
    /// every cell entered, so the start cell is not counted: if the start and the goal are the
    /// same cell, the cost is zero and the path has a single cell.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException">
    /// <paramref name="start"/> or <paramref name="goal"/> is outside the grid. Unlike a
    /// missing path, this means the caller passed invalid data.
    /// </exception>
    SearchResult Find(Grid grid, Point start, Point goal);
}
