using PathLab.Core.Structures;

namespace PathLab.Core.Abstractions;

/// <summary>
/// Builds a map: a <see cref="Grid"/> with its walls and terrain already placed.
/// </summary>
/// <remarks>
/// Each implementation decides the layout (scattered obstacles, a maze, ...), so the
/// caller only chooses the size. Two things are worth knowing before relying on a generator:
/// <list type="bullet">
/// <item>
/// A generator does not promise that a path exists between two points unless its own
/// documentation says so. A random map can leave the start or the goal inside a wall.
/// </item>
/// <item>
/// Randomness should be injected (a <see cref="Random"/> given to the constructor) instead
/// of created inside the generator. With a fixed seed the same map comes out every time,
/// which keeps tests deterministic and lets a failing map be reproduced.
/// </item>
/// </list>
/// </remarks>
public interface IMapGenerator
{
    /// <summary>
    /// Generates a new map of the requested size.
    /// </summary>
    /// <param name="width">Number of columns of the map.</param>
    /// <param name="height">Number of rows of the map.</param>
    /// <returns>
    /// A new <see cref="Grid"/> on every call, so the caller owns it and can modify it freely.
    /// Cells the generator does not touch stay free.
    /// </returns>
    Grid Generate(int width, int height);
}
