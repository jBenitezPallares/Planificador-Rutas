using PathLab.Core.Structures;

namespace PathLab.Core.Abstractions;

public interface IHeuristic
{
    double Estimate(Point from, Point goal);
}