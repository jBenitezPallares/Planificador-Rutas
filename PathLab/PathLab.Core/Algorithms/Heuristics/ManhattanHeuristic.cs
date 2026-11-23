using PathLab.Core.Abstractions;
using PathLab.Core.Structures;

namespace PathLab.Core.Algorithms.Heuristics;

/// <summary>
/// The distance walking only along rows and columns: <c>|rows| + |columns|</c>.
/// </summary>
/// <remarks>
/// With four directions it is exact on a free map, so A* goes straight to the goal. It must not
/// be used with eight directions: a diagonal covers two units for a cost of 1.414, so the estimate
/// is too high and A* could return a path that is not the cheapest.
/// </remarks>
public class ManhattanHeuristic: IHeuristic
{
    /// <inheritdoc />
    public double Estimate(Point from, Point goal)
    {
        return Math.Abs(from.Row - goal.Row) + Math.Abs(from.Column - goal.Column);
    }
}
