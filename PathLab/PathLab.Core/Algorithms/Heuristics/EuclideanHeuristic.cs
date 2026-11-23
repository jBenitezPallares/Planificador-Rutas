using PathLab.Core.Abstractions;
using PathLab.Core.Structures;

namespace PathLab.Core.Algorithms.Heuristics;

/// <summary>
/// The straight-line distance: <c>sqrt(rows² + columns²)</c>.
/// </summary>
/// <remarks>
/// It works with both movement modes, but it is the least informed of the three: on a free map
/// it stays below the real cost, so A* explores more cells than with Manhattan or octile distance.
/// <para>
/// With eight directions it is not strictly admissible. A diagonal step costs 1.414 while the
/// straight-line distance of one diagonal is the square root of two (1.41421...), so the estimate
/// can exceed the real cost by about 0.0002 per diagonal. It is harmless in practice, and the
/// tests compare it with a tolerance.
/// </para>
/// </remarks>
public class EuclideanHeuristic : IHeuristic
{
    /// <inheritdoc />
    public double Estimate(Point from, Point goal)
    {
        return Math.Sqrt(Math.Pow(from.Row - goal.Row, 2) + Math.Pow(from.Column - goal.Column, 2));
    }
}
