using PathLab.Core.Abstractions;
using PathLab.Core.Algorithms.Heuristics;
using PathLab.Core.Structures;

namespace PathLab.Core.Tests.Algorithms.Heuristics;

[TestClass]
public class EuclideanHeuristicTests : HeuristicContractTests
{
    protected override IHeuristic CreateHeuristic() => new EuclideanHeuristic();

    [TestMethod]
    public void Estimate_ThreeFourFiveTriangle_ReturnsFive()
    {
        Assert.AreEqual(5, CreateHeuristic().Estimate(new Point(0, 0), new Point(3, 4)));
    }
    
    [TestMethod]
    public void Estimate_FourDirectionsOpenGrid_IsLessThanDijkstraCost()
    {
        Grid grid = new Grid(5, 5);
        Point from = new Point(1, 2);
        Point goal = new Point(4, 3);

        double estimate = CreateHeuristic().Estimate(from, goal);
        double realCost = GetDijkstraCost(grid, from, goal, MovementMode.FourDirections);

        Assert.IsLessThan(realCost, estimate);
    }

    [TestMethod]
    public void Estimate_EightDirectionsDiagonalMove_IsAlmostTheRealCost()
    {
        Grid grid = new Grid(2, 2);
        Point from = new Point(0, 0);
        Point goal = new Point(1, 1);

        double estimate = CreateHeuristic().Estimate(from, goal);
        double realCost = GetDijkstraCost(grid, from, goal, MovementMode.EightDirections);

        Assert.AreEqual(realCost, estimate, 0.001);
    }
}