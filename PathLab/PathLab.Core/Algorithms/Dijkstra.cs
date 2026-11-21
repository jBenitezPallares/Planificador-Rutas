using PathLab.Core.Algorithms.Heuristics;
using PathLab.Core.Structures;

namespace PathLab.Core.Algorithms;

public class Dijkstra : AStar
{
    public Dijkstra(MovementMode movementMode) : base(movementMode, new ZeroHeuristic()) { }
}