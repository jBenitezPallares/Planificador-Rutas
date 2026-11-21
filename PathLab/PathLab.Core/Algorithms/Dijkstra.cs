using PathLab.Core.Algorithms.Heuristics;
using PathLab.Core.Structures;

namespace PathLab.Core.Algorithms;

/// <summary>
/// Finds the cheapest path using only the cost paid so far.
/// </summary>
/// <remarks>
/// It is A* with a heuristic that always returns zero (<see cref="ZeroHeuristic"/>). With no
/// estimate of what is left, the priority <c>f = g + h</c> reduces to <c>g</c>, so the search
/// spreads in every direction like a wave until it reaches the goal. That is why it explores
/// more cells than A* with a useful heuristic, and why it always finds the cheapest path.
/// </remarks>
public class Dijkstra : AStar
{
    /// <summary>
    /// Creates a Dijkstra finder for the given movement mode.
    /// </summary>
    /// <param name="movementMode">Four directions, or eight when diagonals are allowed.</param>
    public Dijkstra(MovementMode movementMode) : base(movementMode, new ZeroHeuristic()) { }
}
