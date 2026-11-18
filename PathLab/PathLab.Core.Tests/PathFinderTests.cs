namespace PathLab.Core.Tests;

[TestClass]
public class PathFinderTests
{
    [TestMethod]
    public void Find_HandMadeMapFourDirections_ReturnsCostSeven()
    {
        Grid grid = new Grid(5, 4);
        List<Point> wallPoints = new List<Point>(new[]
        {
            new Point(0, 3), new Point(1, 1), new Point(1, 3),
            new Point(2, 1), new Point(3, 3)
        });
        foreach (Point wall in wallPoints)
        {
            grid.SetCell(wall, CellType.Wall);
        }

        IPathFinder finder = new Dijkstra(MovementMode.FourDirections);
        SearchResult result = finder.Find(grid, new Point(0, 0), new Point(3, 4));

        Assert.IsTrue(result.Found);
        Assert.AreEqual(7, result.Cost);
    }

    [TestMethod]
    public void Find_HandMadeMapEightDirections_ReturnsCostFivePointTwoFourTwo()
    {
        Grid grid = new Grid(5,4);
        List<Point> wallPoints = new List<Point>(new[]
        {
            new Point(0, 3), new Point(1, 1), new Point(1, 3),
            new Point(2, 1), new Point(3, 3)
        });
        foreach (Point wall in wallPoints)
        {
            grid.SetCell(wall, CellType.Wall);
        }
        
        IPathFinder finder = new Dijkstra(MovementMode.EightDirections);
        SearchResult result = finder.Find(grid, new Point(0, 0), new Point(3, 4));
        
        Assert.IsTrue(result.Found);
        Assert.AreEqual(5.242, result.Cost, 0.001);
    }

    [TestMethod]
    public void Find_LargeOpenMap_ReportsPositiveElapsedTime()
    {
        // Big enough that the search takes more than one millisecond on any machine.
        Grid grid = new Grid(300, 300);

        IPathFinder finder = new Dijkstra(MovementMode.FourDirections);
        SearchResult result = finder.Find(grid, new Point(0, 0), new Point(299, 299));

        Assert.IsTrue(result.Found);
        Assert.IsTrue(result.ElapsedMs > 0);
    }

    [TestMethod]
    public void Find_SmallMap_ReportsNonNegativeElapsedTime()
    {
        Grid grid = new Grid(3, 3);

        IPathFinder finder = new Dijkstra(MovementMode.FourDirections);
        SearchResult result = finder.Find(grid, new Point(0, 0), new Point(2, 2));

        Assert.IsTrue(result.ElapsedMs >= 0);
    }
}