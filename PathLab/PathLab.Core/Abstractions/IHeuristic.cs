using PathLab.Core.Structures;

namespace PathLab.Core.Abstractions;

/// <summary>
/// Estimates the cost still left to reach the goal from a given point.
/// </summary>
/// <remarks>
/// A* orders its cells by <c>f = g + h</c>, where <c>g</c> is the cost already paid and
/// <c>h</c> is this estimate. A good estimate makes the search go straight towards the goal
/// instead of spreading in every direction, which is why A* explores fewer cells than Dijkstra.
/// <para>
/// A* only returns the cheapest path if every implementation keeps two properties:
/// </para>
/// <list type="bullet">
/// <item>
/// <description>
/// It is admissible: it never overestimates the real cost. An estimate that is too high
/// makes A* discard the cheapest route and return a more expensive one.
/// </description>
/// </item>
/// <item>
/// <description>
/// It is consistent: it never drops by more than the cost of one step. A* closes each cell
/// the first time it takes it out of the queue and never reopens it, which is only safe
/// when the estimate is consistent.
/// </description>
/// </item>
/// </list>
/// Both depend on the movement mode, so a heuristic is not valid everywhere. Manhattan distance
/// is exact with four directions, but with eight it overestimates, because a diagonal advances
/// two units for a cost of 1.414. Octile distance is the one meant for eight directions.
/// <para>
/// An estimate that always returns zero is valid too, and turns A* into Dijkstra.
/// </para>
/// </remarks>
public interface IHeuristic
{
    /// <summary>
    /// Estimates the cost of going from <paramref name="from"/> to <paramref name="goal"/>.
    /// </summary>
    /// <param name="from">The point the estimate starts at.</param>
    /// <param name="goal">The destination of the search.</param>
    /// <returns>
    /// A non-negative estimate, zero when both points are the same. It ignores walls and the
    /// cost of the terrain, which is what keeps it cheap to compute and never too high.
    /// </returns>
    double Estimate(Point from, Point goal);
}
