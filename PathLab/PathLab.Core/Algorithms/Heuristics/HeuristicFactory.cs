using PathLab.Core.Abstractions;
using PathLab.Core.Structures;

namespace PathLab.Core.Algorithms.Heuristics;

public static class HeuristicFactory
{
    public static IHeuristic CreateFor(MovementMode mode) => mode switch
    {
        MovementMode.FourDirections => new ManhattanHeuristic(),
        MovementMode.EightDirections => new OctileHeuristic(),
        _ => throw new ArgumentOutOfRangeException(nameof(mode))
    };
}