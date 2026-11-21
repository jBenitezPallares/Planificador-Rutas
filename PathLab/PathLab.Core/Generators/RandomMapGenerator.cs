using PathLab.Core.Abstractions;
using PathLab.Core.Structures;

namespace PathLab.Core.Generators;

public class RandomMapGenerator : IMapGenerator
{
    private readonly MapGenerationOptions _options;
    private readonly Random _random;

    public RandomMapGenerator(MapGenerationOptions options, Random random)
    {
        _options = options ?? throw new ArgumentNullException(nameof(options), "Las opciones no pueden ser nulas");
        _random = random ?? throw new ArgumentNullException(nameof(random), "El generador de azar no puede ser nulo");
        ValidateOptions(options);
    }
    
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