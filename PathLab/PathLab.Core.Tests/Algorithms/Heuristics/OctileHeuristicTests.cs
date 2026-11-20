using PathLab.Core.Abstractions;
using PathLab.Core.Algorithms.Heuristics;
using PathLab.Core.Structures;

namespace PathLab.Core.Tests.Algorithms.Heuristics;

[TestClass]
public class OctileHeuristicTests : HeuristicContractTests
{
    protected override IHeuristic CreateHeuristic() => new OctileHeuristic();

    [TestMethod]
    public void Estimate_PureDiagonal_ReturnsDiagonalFactorTimesSteps()
    {
        Point from = new Point(0, 0);
        Point goal = new Point(3, 3);
        IHeuristic heuristic = CreateHeuristic();
        
        double result = heuristic.Estimate(from, goal);
        
        Assert.AreEqual(4.242, result, 0.001);
    }

    [TestMethod]
    public void Estimate_StraightPlusDiagonal_ReturnsStraightStepsPlusDiagonalSteps()
    {
        Point from = new Point(0, 0);
        Point goal = new Point(2, 5);
        IHeuristic heuristic = CreateHeuristic();
        
        double result = heuristic.Estimate(from, goal);
        
        Assert.AreEqual(5.828, result, 0.001);
    }

    [TestMethod]
    public void Estimate_FourDirectionsOpenGrid_IsLessThanDijkstraCost()
    {
        Grid grid = new Grid(3, 3);
        Point from = new Point(0, 0);
        Point goal = new Point(2, 2);
        
        double estimate = CreateHeuristic().Estimate(from, goal);
        double realCost = GetDijkstraCost(grid, from, goal, MovementMode.FourDirections);
        
        Assert.IsLessThan(realCost, estimate);
    }

    [TestMethod]
    public void Estimate_EightDirectionsOpenGrid_EqualsDijkstraCost()
    {
        Grid grid = new Grid(3, 3);
        Point from = new Point(0, 0);
        Point goal = new Point(2, 2);
        
        double estimate = CreateHeuristic().Estimate(from, goal);
        double realCost = GetDijkstraCost(grid, from, goal, MovementMode.EightDirections);
        
        Assert.AreEqual(realCost, estimate, 0.001);
    }

    [TestMethod]
    public void Estimate_EightDirectionsWithWallsAroundDiagonal_NeverExceedsDijkstraCost()
    {
        Grid grid = new Grid(5, 5);
        grid.SetCell(new Point(1, 2), CellType.Wall);
        grid.SetCell(new Point(2, 1), CellType.Wall);
        Point from = new Point(0, 0);
        Point goal = new Point(4, 4);

        double estimate = CreateHeuristic().Estimate(from, goal);
        double realCost = GetDijkstraCost(grid, from, goal, MovementMode.EightDirections);

        Assert.IsLessThanOrEqualTo(realCost, estimate);
    }
}