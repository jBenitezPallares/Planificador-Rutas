using PathLab.Core.Abstractions;
using PathLab.Core.Algorithms;
using PathLab.Core.Algorithms.Heuristics;
using PathLab.Core.Structures;

namespace PathLab.Core.Tests.Algorithms.Heuristics;

[TestClass]
public class ManhattanHeuristicTests
{
    private readonly IHeuristic _heuristic = new ManhattanHeuristic();

    [TestMethod]
    public void Estimate_SamePoint_ReturnsZero()
    {
        Point point = new Point(1, 1);

        double result = _heuristic.Estimate(point, point);

        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Estimate_RowAndColumnDifference_ReturnsTheirSum()
    {
        double result = _heuristic.Estimate(new Point(0, 0), new Point(1, 3));

        Assert.AreEqual(4, result);
    }

    [TestMethod]
    public void Estimate_IsSymmetric()
    {
        Point point = new Point(0, 0);
        Point otherPoint = new Point(1, 3);

        double result = _heuristic.Estimate(point, otherPoint);
        double symmetric = _heuristic.Estimate(otherPoint, point);

        Assert.AreEqual(symmetric, result);
    }

    [TestMethod]
    public void Estimate_FourDirectionsOpenGrid_EqualsDijkstraCost()
    {
        Grid grid = new Grid(5, 5);
        Point from = new Point(1, 2);
        Point goal = new Point(4, 3);

        double estimate = _heuristic.Estimate(from, goal);
        double realCost = GetDijkstraCost(grid, from, goal, MovementMode.FourDirections);

        Assert.AreEqual(realCost, estimate);
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

        double estimate = _heuristic.Estimate(from, goal);
        double realCost = GetDijkstraCost(grid, from, goal, MovementMode.FourDirections);

        Assert.IsLessThanOrEqualTo(realCost, estimate);
    }

    [TestMethod]
    public void Estimate_EightDirectionsDiagonalMove_OverestimatesRealCost()
    {
        Grid grid = new Grid(2, 2);
        Point from = new Point(0, 0);
        Point goal = new Point(1, 1);

        double estimate = _heuristic.Estimate(from, goal);
        double realCost = GetDijkstraCost(grid, from, goal, MovementMode.EightDirections);

        Assert.AreEqual(2, estimate);
        Assert.AreEqual(1.414, realCost, 0.001);
    }

    private static double GetDijkstraCost(Grid grid, Point from, Point goal, MovementMode mode)
    {
        return new Dijkstra(mode).Find(grid, from, goal).Cost;
    }
}
