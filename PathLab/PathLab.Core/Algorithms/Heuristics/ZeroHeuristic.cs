using PathLab.Core.Abstractions;
using PathLab.Core.Structures;

namespace PathLab.Core.Algorithms.Heuristics;

public class ZeroHeuristic : IHeuristic
{
    public double Estimate(Point from, Point goal) => 0;
}