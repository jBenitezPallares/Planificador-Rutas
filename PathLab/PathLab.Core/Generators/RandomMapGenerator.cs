using PathLab.Core.Abstractions;
using PathLab.Core.Structures;

namespace PathLab.Core.Generators;

/// <summary>
/// Builds a map with walls, mud and water scattered at random, in exact amounts.
/// </summary>
/// <remarks>
/// The amount of each kind is the percentage of the total number of cells, rounded, so it is the
/// same every time (a 20 % of a 20 x 20 map is always 80 walls) and only the positions change.
/// The positions come from shuffling the indexes of all the cells (Fisher-Yates) and handing out
/// consecutive ranges: first the walls, then the mud and then the water. The rest stays free.
/// <para>
/// Nothing guarantees a path between two cells: the start or the goal can land on a wall.
/// The randomness is injected, so the same seed always builds the same map.
/// </para>
/// </remarks>
public class RandomMapGenerator : IMapGenerator
{
    private readonly MapGenerationOptions _options;
    private readonly Random _random;

    /// <summary>
    /// Creates a random map generator.
    /// </summary>
    /// <param name="options">The percentage of walls, mud and water.</param>
    /// <param name="random">The source of randomness that shuffles the cells.</param>
    /// <exception cref="ArgumentNullException"><paramref name="options"/> or <paramref name="random"/> is <c>null</c>.</exception>
    /// <exception cref="ArgumentOutOfRangeException">
    /// A percentage is negative or greater than 1, or the three of them add up to more than 1.
    /// </exception>
    public RandomMapGenerator(MapGenerationOptions options, Random random)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options), "Las opciones no pueden ser nulas");
        _random = random ?? throw new ArgumentNullException(nameof(random), "El generador de azar no puede ser nulo");
        ValidateOptions(options);
    }

    /// <inheritdoc />
    /// <remarks>
    /// When the rounded amounts add up to more than the number of cells, the last kinds get the
    /// cells that are left instead of failing.
    /// </remarks>
    public Grid Generate(int width, int height)
    {
        int total =  width * height;
        int wallCount = (int)Math.Round(total * _options.WallPercentage);
        int mudCount = (int)Math.Round(total * _options.MudPercentage);
        int waterCount = (int)Math.Round(total * _options.WaterPercentage);

        Grid grid = new Grid(width, height);
        int[] indices = Enumerable.Range(0, total).ToArray();

        for (int i = indices.Length - 1; i > 0; i--)
        {
            int j = _random.Next(i + 1);
            (indices[i], indices[j]) = (indices[j], indices[i]);
        }

        int index = 0;
        index = Paint(grid, indices, index, wallCount, CellType.Wall);
        index = Paint(grid, indices, index, mudCount, CellType.Mud);
        Paint(grid, indices, index, waterCount, CellType.Water);

        return grid;
    }

    private static void ValidateOptions(MapGenerationOptions options)
    {
        if (options.MudPercentage < 0 || options.WaterPercentage < 0 || options.WallPercentage < 0)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Los porcentajes no pueden ser negativos");
        }

        if (options.MudPercentage > 1 || options.WaterPercentage > 1 || options.WallPercentage > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Los porcentajes no pueden ser mayores a uno");
        }

        if (options.MudPercentage + options.WaterPercentage + options.WallPercentage > 1)
        {
            throw new ArgumentOutOfRangeException(nameof(options), "Los porcentajes no pueden sumar mas de uno");
        }
    }

    private static int Paint(Grid grid, int[] indices, int start, int count, CellType type)
    {
        int width = grid.Width;
        int total = grid.Width * grid.Height;
        int end = Math.Min(start + count, total);

        for (int k = start; k < end; k++)
        {
            grid.SetCell(ToPoint(indices[k], width), type);
        }

        return end;
    }

    private static Point ToPoint(int index, int width) => new Point(index / width, index % width);
}
