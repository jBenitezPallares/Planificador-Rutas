using PathLab.Core.Abstractions;
using PathLab.Core.Structures;

namespace PathLab.Core.Algorithms.Heuristics;

/// <summary>
/// Picks the heuristic that is valid for a movement mode.
/// </summary>
/// <remarks>
/// A heuristic is only valid for the movement it was designed for, so the pair mode and heuristic
/// lives in one place instead of being repeated wherever an A* is created.
/// </remarks>
public static class HeuristicFactory
{
    /// <summary>
    /// Creates the heuristic that matches a movement mode.
    /// </summary>
    /// <param name="mode">The movement mode of the finder that will use the heuristic.</param>
    /// <returns>
    /// <see cref="ManhattanHeuristic"/> for four directions and <see cref="OctileHeuristic"/>
    /// for eight. Both are exact on a free map.
    /// </returns>
    /// <exception cref="ArgumentOutOfRangeException"><paramref name="mode"/> is not a known movement mode.</exception>
    public static IHeuristic CreateFor(MovementMode mode) => mode switch
    {
        MovementMode.FourDirections => new ManhattanHeuristic(),
        MovementMode.EightDirections => new OctileHeuristic(),
        _ => throw new ArgumentOutOfRangeException(nameof(mode))
    };
}
