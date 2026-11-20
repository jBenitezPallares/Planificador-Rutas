using PathLab.Core.Abstractions;
using PathLab.Core.Structures;

namespace PathLab.Core.Algorithms.Heuristics;

public class ManhattanHeuristic: IHeuristic
{
    public double Estimate(Point from, Point goal)
    {
        return Math.Abs(from.Row - goal.Row) + Math.Abs(from.Column - goal.Column);
    }
}