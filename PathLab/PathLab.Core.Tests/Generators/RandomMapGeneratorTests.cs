using PathLab.Core.Abstractions;
using PathLab.Core.Generators;
using PathLab.Core.Structures;

namespace PathLab.Core.Tests.Generators;

[TestClass]
public class RandomMapGeneratorTests
{
    [TestMethod]
    public void Constructor_NullOptions_ThrowsException()
    {
        ArgumentNullException exception =
            Assert.ThrowsExactly<ArgumentNullException>(() => new RandomMapGenerator(null!, new Random(42)));

        Assert.AreEqual("options", exception.ParamName);
    }

    [TestMethod]
    public void Constructor_PercentagesSumGreaterThanOne_ThrowsException()
    {
        MapGenerationOptions options = new MapGenerationOptions(0.4, 0.8, 0.2);
        
        ArgumentOutOfRangeException exception =
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => new RandomMapGenerator(options, new Random()));
        
        Assert.AreEqual("options", exception.ParamName);
    }
    
    [TestMethod]
    public void Generate_ReturnsGridWithRequestedSize()
    {
        IMapGenerator randomGenerator = new RandomMapGenerator(
            new MapGenerationOptions(WallPercentage: 0.2), new Random(42));
        int height = 20;
        int width = 20;
        
        Grid result = randomGenerator.Generate(width, height);
        
        Assert.AreEqual(width, result.Width);
        Assert.AreEqual(height, result.Height);
    }

    [TestMethod]
    public void Generate_ZeroPercent_HasNoWalls()
    {
        MapGenerationOptions options = new MapGenerationOptions(0, 0.8, 0.2);
        IMapGenerator randomGenerator = new RandomMapGenerator(options, new Random(42));
        
        Grid grid = randomGenerator.Generate(20, 20);
        int result = CountWalls(grid);
        
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Generate_FullPercent_AllCellsAreWalls()
    {
        MapGenerationOptions options = new MapGenerationOptions(1);
        IMapGenerator randomGenerator = new RandomMapGenerator(options, new Random(42));
        
        Grid grid = randomGenerator.Generate(20, 20);
        int result = CountWalls(grid);
        
        Assert.AreEqual(20 * 20, result);
    }

    [TestMethod]
    public void Generate_TwentyPercent_HasExactlyThatManyWalls()
    {
        MapGenerationOptions options = new MapGenerationOptions(0.2);
        IMapGenerator randomGenerator = new RandomMapGenerator(options, new Random(42));
        
        Grid grid = randomGenerator.Generate(20, 20);
        int result = CountWalls(grid);
        
        Assert.AreEqual(80, result);
    }

    [TestMethod]
    public void Generate_SameSeed_ReturnsSameMap()
    {
        MapGenerationOptions options = new MapGenerationOptions(0.3);
        IMapGenerator randomGenerator = new RandomMapGenerator(options, new Random(42));
        IMapGenerator otherRandomGenerator = new RandomMapGenerator(options, new Random(42));
        
        Grid grid = randomGenerator.Generate(20, 20);
        Grid otherGrid = otherRandomGenerator.Generate(20, 20);
        int result = CountWalls(grid);
        int otherResult = CountWalls(otherGrid);
        
        Assert.AreEqual(result, otherResult);
    }

    [TestMethod]
    public void Generate_WithMudAndWaterPercentages_PlacesExactlyThoseCounts()
    {
        MapGenerationOptions options = new MapGenerationOptions(0, 0.4, 0.5);
        IMapGenerator randomGenerator = new RandomMapGenerator(options, new Random(42));
        
        Grid grid = randomGenerator.Generate(20, 20);
        int mud = CountCellsWithCost(grid, 5);      
        int water = CountCellsWithCost(grid, 10);   
        
        Assert.AreEqual(160, mud);
        Assert.AreEqual(200, water);
    }
    
    [TestMethod]
    public void Generate_RoundingExceedsTotal_DoesNotThrow()
    {
        MapGenerationOptions options = new MapGenerationOptions(0.5, 0.5);
        IMapGenerator generator = new RandomMapGenerator(options, new Random(42));

        Grid grid = generator.Generate(3, 1);

        Assert.AreEqual(2, CountWalls(grid));
        Assert.AreEqual(1, CountCellsWithCost(grid, 5));
    }
    
    private static int CountWalls(Grid grid)
    {
        int walls = 0;

        for (int row = 0; row < grid.Height; row++)
        {
            for (int column = 0; column < grid.Width; column++)
            {
                if (grid.IsWall(new Point(row, column)))
                {
                    walls++;
                }
            }
        }

        return walls;
    }

    private static int CountCellsWithCost(Grid grid, int cost)
    {
        int amount = 0;

        for (int row = 0; row < grid.Height; row++)
        {
            for (int column = 0; column < grid.Width; column++)
            {
                Point current = new Point(row, column);
                if (!grid.IsWall(current) && grid.GetCost(current) == cost)
                {
                    amount++;
                }
            }
        }
        
        return amount;
    }
}