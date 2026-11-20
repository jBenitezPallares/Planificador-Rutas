using PathLab.Core.Abstractions;
using PathLab.Core.Algorithms;
using PathLab.Core.Structures;

namespace PathLab.Core.Tests.Algorithms.Heuristics;

public abstract class HeuristicContractTests
{
    protected abstract IHeuristic CreateHeuristic();
    
    [TestMethod]
    public void Estimate_SamePoint_ReturnsZero()
    {
        Point point = new Point(1, 1);
        
        double result = CreateHeuristic().Estimate(point, point);

        Assert.AreEqual(0, result);
    }
    
    [TestMethod]
    public void Estimate_IsSymmetric()
    {
        IHeuristic heuristic = CreateHeuristic();
        Point point = new Point(0, 0);
        Point otherPoint = new Point(3, 4);

        Assert.AreEqual(heuristic.Estimate(otherPoint, point), heuristic.Estimate(point, otherPoint));
    }
    
    [TestMethod]
    [DataRow(CellType.Mud)]
    [DataRow(CellType.Water)]
    public void Estimate_FourDirectionsWithMudAndWater_NeverExceedsDijkstraCost(CellType crossable)
    {
        Grid grid = new Grid(5, 5);
        for (int column = 0; column < grid.Width; column++)
        {
            grid.SetCell(new Point(2, column), crossable);
        }
        Point from = new Point(0, 0);
        Point goal = new Point(4, 3);

        double estimate = CreateHeuristic().Estimate(from, goal);
        double realCost = GetDijkstraCost(grid, from, goal, MovementMode.FourDirections);

        Assert.IsLessThanOrEqualTo(realCost, estimate);
    }
    
    protected static double GetDijkstraCost(Grid grid, Point from, Point goal, MovementMode mode)
    {
        return new Dijkstra(mode).Find(grid, from, goal).Cost;
    }
}