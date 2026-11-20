using PathLab.Core.Abstractions;
using PathLab.Core.Structures;

namespace PathLab.Core.Algorithms.Heuristics;

public class OctileHeuristic : IHeuristic
{
    public double Estimate(Point from, Point goal)
    {
        int rowDistance = Math.Abs(from.Row - goal.Row);
        int columnDistance = Math.Abs(from.Column - goal.Column);

        return Math.Max(rowDistance, columnDistance)
               + (MovementModeExtensions.DiagonalFactor - 1) * Math.Min(rowDistance, columnDistance);
    }
}