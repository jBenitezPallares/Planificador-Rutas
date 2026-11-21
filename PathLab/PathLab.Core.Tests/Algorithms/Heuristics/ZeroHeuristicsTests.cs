using PathLab.Core.Abstractions;
using PathLab.Core.Algorithms.Heuristics;
using PathLab.Core.Structures;

namespace PathLab.Core.Tests.Algorithms.Heuristics;

[TestClass]
public class ZeroHeuristicsTests : HeuristicContractTests
{
    protected override IHeuristic CreateHeuristic() => new ZeroHeuristic();

    [TestMethod]
    public void Estimate_AnyPoints_ReturnsZero()
    {
        Point from = new Point(0, 0);
        Point goal = new Point(3, 3);
        IHeuristic heuristic = CreateHeuristic();
        
        double result = heuristic.Estimate(from, goal);
        
        Assert.AreEqual(0, result);
    }

    [TestMethod]
    public void Estimate_FarApartPoints_ReturnsZero()
    {
        Point from = new Point(0, 0);
        Point goal = new Point(100, 100);
        IHeuristic heuristic = CreateHeuristic();
        
        double result = heuristic.Estimate(from, goal);
        
        Assert.AreEqual(0, result);
    }
}