namespace PathLab.Core.Tests;

[TestClass]
public class BreadthFirstSearchTests: PathFinderContractTests
{
    protected override IPathFinder CreateFinder(MovementMode mode) => new BreadthFirstSearch(mode);
    
    [TestMethod]
    public void Find_MudOnShortestRoute_ReturnsShorterButMoreExpensivePath()
    {
        Grid grid = new Grid(3, 3);
        grid.SetCell(new Point(0, 1), CellType.Mud);
        IPathFinder finder = CreateFinder(MovementMode.FourDirections);

        SearchResult result = finder.Find(grid, new Point(0, 0), new Point(0, 2));

        Assert.HasCount(3, result.Path);
        Assert.AreEqual(6, result.Cost);
    }
    
    [TestMethod]
    public void Find_FourDirections_ReturnsPathWithFewestCells()
    {
        Grid grid = new Grid(3, 3);
        grid.SetCell(new Point(0, 1), CellType.Water);
        IPathFinder finder = CreateFinder(MovementMode.FourDirections);

        SearchResult result = finder.Find(grid, new Point(0, 0), new Point(0, 2));

        Assert.HasCount(3, result.Path);
        Assert.AreEqual(new Point(0, 1), result.Path[1]);
        Assert.AreEqual(11, result.Cost);
    }

    [TestMethod]
    public void Find_EightDirections_ReturnsPathWithFewestCells()
    {
        Grid grid = new Grid(10, 10);
        for (int i = 0; i < 10; i++)
        {
            grid.SetCell(new Point(i,i), CellType.Water);
        }
        IPathFinder finder = CreateFinder(MovementMode.EightDirections);
        
        SearchResult result = finder.Find(grid, new Point(0, 0), new Point(9, 9));
        
        Assert.HasCount(10, result.Path);
        Assert.AreEqual(127.259, result.Cost, 0.001);
    }
}