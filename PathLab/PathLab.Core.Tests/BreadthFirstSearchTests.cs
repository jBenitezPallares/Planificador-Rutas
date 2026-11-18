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
}