using PathLab.Core.Abstractions;
using PathLab.Core.Algorithms.Heuristics;
using PathLab.Core.Structures;

namespace PathLab.Core.Tests.Algorithms.Heuristics;

[TestClass]
public class ManhattanHeuristicTests : HeuristicContractTests
{
    protected override IHeuristic CreateHeuristic() => new ManhattanHeuristic();

    [TestMethod]
    public void Estimate_RowAndColumnDifference_ReturnsTheirSum()
    {
        double result = CreateHeuristic().Estimate(new Point(0, 0), new Point(1, 3));

        Assert.AreEqual(4, result);
    }

    [TestMethod]
    public void Estimate_FourDirectionsOpenGrid_EqualsDijkstraCost()
    {
        Grid grid = new Grid(5, 5);
        Point from = new Point(1, 2);
        Point goal = new Point(4, 3);

        double estimate = CreateHeuristic().Estimate(from, goal);
        double realCost = GetDijkstraCost(grid, from, goal, MovementMode.FourDirections);

        Assert.AreEqual(realCost, estimate);
    }

    [TestMethod]
    public void Estimate_EightDirectionsDiagonalMove_OverestimatesRealCost()
    {
        Grid grid = new Grid(2, 2);
        Point from = new Point(0, 0);
        Point goal = new Point(1, 1);

        double estimate = CreateHeuristic().Estimate(from, goal);
        double realCost = GetDijkstraCost(grid, from, goal, MovementMode.EightDirections);

        Assert.AreEqual(2, estimate);
        Assert.AreEqual(1.414, realCost, 0.001);
    }
}
