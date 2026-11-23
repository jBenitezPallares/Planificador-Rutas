using PathLab.Core.Abstractions;
using PathLab.Core.Generators;
using PathLab.Core.Structures;

namespace PathLab.Core.Tests.Generators;

[TestClass]
public class RandomMapGeneratorTests : MapGeneratorContractTests
{
    protected override IMapGenerator CreateGenerator(Random random) =>
        new RandomMapGenerator(new MapGenerationOptions(0.2, 0.1, 0.05), random);

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
}
