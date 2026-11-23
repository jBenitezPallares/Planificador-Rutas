using PathLab.Core.Abstractions;
using PathLab.Core.Structures;

namespace PathLab.Core.Algorithms.Heuristics;

/// <summary>
/// The distance when diagonal moves are allowed:
/// <c>max(rows, columns) + (diagonal factor - 1) * min(rows, columns)</c>.
/// </summary>
/// <remarks>
/// It walks diagonally as long as both distances last and then straight for the rest. It uses the
/// same diagonal factor as the grid, so on a free map with eight directions it is exact and it
/// never overestimates. It is the heuristic meant for eight directions.
/// </remarks>
public class OctileHeuristic : IHeuristic
{
    /// <inheritdoc />
    public double Estimate(Point from, Point goal)
    {
        int rowDistance = Math.Abs(from.Row - goal.Row);
        int columnDistance = Math.Abs(from.Column - goal.Column);

        return Math.Max(rowDistance, columnDistance)
               + (MovementModeExtensions.DiagonalFactor - 1) * Math.Min(rowDistance, columnDistance);
    }
}
