using PathLab.Core.Abstractions;
using PathLab.Core.Structures;

namespace PathLab.Core.Tests.Generators;

public abstract class MapGeneratorContractTests
{
    protected const int DefaultWidth = 43;
    protected const int DefaultHeight = 39;

    protected abstract IMapGenerator CreateGenerator(Random random);

    [TestMethod]
    public void Constructor_NullRandom_ThrowsException()
    {
        ArgumentNullException exception =
            Assert.ThrowsExactly<ArgumentNullException>(() => CreateGenerator(null!));

        Assert.AreEqual("random", exception.ParamName);
    }

    [TestMethod]
    public void Generate_ReturnsGridWithRequestedSize()
    {
        IMapGenerator generator = CreateGenerator(new Random(42));

        Grid grid = generator.Generate(DefaultWidth, DefaultHeight);

        Assert.AreEqual(DefaultWidth, grid.Width);
        Assert.AreEqual(DefaultHeight, grid.Height);
    }

    [TestMethod]
    public void Generate_ReturnsNewGridOnEveryCall()
    {
        IMapGenerator generator = CreateGenerator(new Random(42));

        Grid grid = generator.Generate(DefaultWidth, DefaultHeight);
        Grid otherGrid = generator.Generate(DefaultWidth, DefaultHeight);

        Assert.AreNotSame(grid, otherGrid);
    }

    [TestMethod]
    public void Generate_SameSeed_ReturnsSameMap()
    {
        IMapGenerator generator = CreateGenerator(new Random(42));
        IMapGenerator otherGenerator = CreateGenerator(new Random(42));

        Grid grid = generator.Generate(DefaultWidth, DefaultHeight);
        Grid otherGrid = otherGenerator.Generate(DefaultWidth, DefaultHeight);

        Assert.IsTrue(AreSameMap(grid, otherGrid));
    }

    [TestMethod]
    public void Generate_DifferentSeeds_ReturnDifferentMaps()
    {
        IMapGenerator generator = CreateGenerator(new Random(1));
        IMapGenerator otherGenerator = CreateGenerator(new Random(2));

        Grid grid = generator.Generate(DefaultWidth, DefaultHeight);
        Grid otherGrid = otherGenerator.Generate(DefaultWidth, DefaultHeight);

        Assert.IsFalse(AreSameMap(grid, otherGrid));
    }

    protected static int CountCellsWithCost(Grid grid, int cost)
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

    protected static int CountWalls(Grid grid)
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

    private static bool AreSameMap(Grid grid, Grid otherGrid)
    {
        if (grid.Width != otherGrid.Width || grid.Height != otherGrid.Height)
        {
            return false;
        }

        for (int row = 0; row < grid.Height; row++)
        {
            for (int column = 0; column < grid.Width; column++)
            {
                Point current = new Point(row, column);

                if (grid.IsWall(current) != otherGrid.IsWall(current))
                {
                    return false;
                }

                if (!grid.IsWall(current) && grid.GetCost(current) != otherGrid.GetCost(current))
                {
                    return false;
                }
            }
        }

        return true;
    }
}
