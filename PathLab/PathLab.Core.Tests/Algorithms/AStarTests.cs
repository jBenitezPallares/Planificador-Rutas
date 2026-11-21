using PathLab.Core.Abstractions;
using PathLab.Core.Algorithms;
using PathLab.Core.Algorithms.Heuristics;
using PathLab.Core.Structures;

namespace PathLab.Core.Tests.Algorithms;

[TestClass]
public class AStarTests : PathFinderContractTests
{
    protected override IPathFinder CreateFinder(MovementMode mode) =>
        new AStar(mode, HeuristicFactory.CreateFor(mode));

    [TestMethod]
    public void Constructor_NullHeuristic_ThrowsException()
    {
        ArgumentNullException exception =
            Assert.ThrowsExactly<ArgumentNullException>(() => new AStar(MovementMode.FourDirections, null!));

        Assert.AreEqual("heuristic", exception.ParamName);
    }

    [TestMethod]
    public void Find_FourDirectionsWithManhattan_ReturnsSameCostAsDijkstra()
    {
        AssertSameCostAsDijkstra(new Grid(4, 4), new Point(1, 1), new Point(3, 3), MovementMode.FourDirections);
    }

    [TestMethod]
    public void Find_EightDirectionsWithOctile_ReturnsSameCostAsDijkstra()
    {
        AssertSameCostAsDijkstra(new Grid(4, 4), new Point(1, 1), new Point(3, 3), MovementMode.EightDirections);
    }

    [TestMethod]
    [DataRow(CellType.Mud)]
    [DataRow(CellType.Water)]
    public void Find_WithMudAndWater_ReturnsSameCostAsDijkstra(CellType crossable)
    {
        Grid grid = new Grid(4, 4);
        grid.SetCell(new Point(2, 2), crossable);

        AssertSameCostAsDijkstra(grid, new Point(1, 1), new Point(3, 3), MovementMode.EightDirections);
    }

    [TestMethod]
    public void Find_OpenGridFourDirections_ExploresFewerCellsThanDijkstra()
    {
        AssertExploresFewerCellsThanDijkstra(
            new Grid(11, 11), new Point(5, 4), new Point(10, 10), MovementMode.FourDirections);
    }

    [TestMethod]
    public void Find_OpenGridEightDirections_ExploresFewerCellsThanDijkstra()
    {
        AssertExploresFewerCellsThanDijkstra(
            new Grid(11, 11), new Point(5, 4), new Point(10, 10), MovementMode.EightDirections);
    }

    private void AssertSameCostAsDijkstra(Grid grid, Point from, Point goal, MovementMode mode)
    {
        (SearchResult aStar, SearchResult dijkstra) = FindWithBoth(grid, from, goal, mode);

        Assert.AreEqual(dijkstra.Cost, aStar.Cost, 0.001);
    }

    private void AssertExploresFewerCellsThanDijkstra(Grid grid, Point from, Point goal, MovementMode mode)
    {
        (SearchResult aStar, SearchResult dijkstra) = FindWithBoth(grid, from, goal, mode);

        Assert.IsLessThan(dijkstra.ExplorationOrder.Count, aStar.ExplorationOrder.Count);
    }

    private (SearchResult AStar, SearchResult Dijkstra) FindWithBoth(Grid grid, Point from, Point goal, MovementMode mode)
    {
        SearchResult aStar = CreateFinder(mode).Find(grid, from, goal);
        SearchResult dijkstra = new Dijkstra(mode).Find(grid, from, goal);

        return (aStar, dijkstra);
    }
}
