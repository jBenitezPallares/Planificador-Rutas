using PathLab.Core.Abstractions;
using PathLab.Core.Structures;

namespace PathLab.Core.Algorithms.Heuristics;

/// <summary>
/// An estimate that is always zero, which means "I know nothing about what is left".
/// </summary>
/// <remarks>
/// It is valid for any movement mode, because zero never overestimates and never changes between
/// neighbors. With it, A* orders its cells by the cost paid alone, which is exactly
/// <see cref="Dijkstra"/>.
/// </remarks>
public class ZeroHeuristic : IHeuristic
{
    /// <inheritdoc />
    public double Estimate(Point from, Point goal) => 0;
}
