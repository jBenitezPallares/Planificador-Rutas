using PathLab.Core.Abstractions;
using PathLab.Core.Algorithms;
using PathLab.Core.Generators;
using PathLab.Core.Structures;

namespace PathLab.Core.Tests.Generators;

[TestClass]
public class MazeMapGeneratorTests : MapGeneratorContractTests
{
    protected override IMapGenerator CreateGenerator(Random random) => new MazeMapGenerator(random);

    [TestMethod]
    public void Generate_FreeCellCount_IsTwoTimesCellsMinusOne()
    {
        IMapGenerator generator = CreateGenerator(new Random(42));
        Grid grid = generator.Generate(DefaultWidth, DefaultHeight);
        int realCells = ((DefaultWidth + 1) / 2) * ((DefaultHeight + 1) / 2);

        int result = CountCellsWithCost(grid, 1);

        Assert.AreEqual(2 * realCells - 1, result);
    }

    [TestMethod]
    public void Generate_Corners_AreConnected()
    {
        IMapGenerator generator = CreateGenerator(new Random(42));
        IPathFinder dijkstra = new Dijkstra(MovementMode.FourDirections);
        Grid grid = generator.Generate(DefaultWidth, DefaultHeight);
        Point goal = new Point(grid.Height - 1, grid.Width - 1);

        SearchResult result = dijkstra.Find(grid, new Point(0, 0), goal);

        Assert.IsTrue(result.Found);
    }

    [TestMethod]
    public void Generate_AllFreeCellsAreReachable()
    {
        IMapGenerator generator = CreateGenerator(new Random(42));
        Grid grid = generator.Generate(DefaultWidth, DefaultHeight);
        HashSet<Point> visited = new HashSet<Point> { new Point(0, 0) };
        Queue<Point> pending = new Queue<Point>();
        pending.Enqueue(new Point(0, 0));

        while (pending.Count > 0)
        {
            Point current = pending.Dequeue();

            foreach (Point neighbor in grid.GetNeighbors(current, MovementMode.FourDirections))
            {
                if (visited.Add(neighbor))
                {
                    pending.Enqueue(neighbor);
                }
            }
        }

        Assert.HasCount(CountCellsWithCost(grid, 1), visited);
    }

    [TestMethod]
    public void Generate_WidthSmallerThanThree_ThrowsException()
    {
        IMapGenerator generator = CreateGenerator(new Random(42));

        ArgumentOutOfRangeException exception =
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => generator.Generate(2, 5));

        Assert.AreEqual("width", exception.ParamName);
    }

    [TestMethod]
    public void Generate_HeightSmallerThanThree_ThrowsException()
    {
        IMapGenerator generator = CreateGenerator(new Random(42));

        ArgumentOutOfRangeException exception =
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => generator.Generate(3, 2));

        Assert.AreEqual("height", exception.ParamName);
    }

    [TestMethod]
    public void Generate_EvenWidth_ThrowsException()
    {
        IMapGenerator generator = CreateGenerator(new Random(42));

        ArgumentOutOfRangeException exception =
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => generator.Generate(32, 25));

        Assert.AreEqual("width", exception.ParamName);
    }

    [TestMethod]
    public void Generate_EvenHeight_ThrowsException()
    {
        IMapGenerator generator = CreateGenerator(new Random(42));

        ArgumentOutOfRangeException exception =
            Assert.ThrowsExactly<ArgumentOutOfRangeException>(() => generator.Generate(25, 32));

        Assert.AreEqual("height", exception.ParamName);
    }
}
