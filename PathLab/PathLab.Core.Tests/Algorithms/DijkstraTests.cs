using PathLab.Core.Abstractions;
using PathLab.Core.Algorithms;
using PathLab.Core.Structures;

namespace PathLab.Core.Tests.Algorithms;

[TestClass]
public class DijkstraTests: PathFinderContractTests
{
    protected override IPathFinder CreateFinder(MovementMode mode) => new Dijkstra(mode);
    
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
        
        IPathFinder finder = CreateFinder(MovementMode.EightDirections);
        SearchResult result = finder.Find(grid, new Point(0, 0), new Point(3, 4));
        
        Assert.IsTrue(result.Found);
        Assert.AreEqual(5.242, result.Cost, 0.001);
    }
    
    [TestMethod]
    [DataRow(CellType.Mud, 6)]     
    [DataRow(CellType.Water, 11)] 
    public void Find_OnlyRouteThroughCrossableCell_ReturnsItsCostPlusOne(CellType middle, int expectedCost)
    {
        Grid grid = new Grid(3, 1);
        grid.SetCell(new Point(0, 1), middle);
        IPathFinder finder = CreateFinder(MovementMode.FourDirections);

        SearchResult result = finder.Find(grid, new Point(0, 0), new Point(0, 2));

        Assert.IsTrue(result.Found);
        Assert.AreEqual(expectedCost, result.Cost);
    }
    
    [TestMethod]
    public void Find_CheaperDetourAroundMud_ReturnsDetourCost()
    {
        Grid grid = new Grid(3, 3);
        grid.SetCell(new Point(0, 1), CellType.Mud);
        IPathFinder finder = CreateFinder(MovementMode.FourDirections);

        SearchResult result = finder.Find(grid, new Point(0, 0), new Point(0, 2));

        Assert.AreEqual(4, result.Cost);
        Assert.HasCount(5, result.Path);
    }
}