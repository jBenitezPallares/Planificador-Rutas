using PathLab.Core.Abstractions;
using PathLab.Core.Structures;

namespace PathLab.Core.Algorithms.Heuristics;

public class EuclideanHeuristic : IHeuristic
{
    public double Estimate(Point from, Point goal)
    {
        return Math.Sqrt(Math.Pow(from.Row - goal.Row, 2) + Math.Pow(from.Column - goal.Column, 2));
    }
}