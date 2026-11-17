namespace PathLab.Core.Tests;

[TestClass]
public class GridTests
{
    [TestMethod]
    public void GetNeighbors_FourDirectionsInCenter_ReturnsFourNeighbors()
    {
        AssertNeighbors(
            new Grid(3, 3),
            new Point(1, 1),
            MovementMode.FourDirections,
            new Point(0, 1), new Point(2, 1), new Point(1, 0), new Point(1, 2));
    }

    [TestMethod]
    public void GetNeighbors_FourDirectionsInCorner_ReturnsTwoNeighbors()
    {
        AssertNeighbors(
            new Grid(3, 3),
            new Point(0, 0),
            MovementMode.FourDirections,
            new Point(0, 1), new Point(1, 0));
    }

    [TestMethod]
    public void GetNeighbors_FourDirectionsOnEdge_ReturnsThreeNeighbors()
    {
        AssertNeighbors(
            new Grid(3, 3),
            new Point(0, 1),
            MovementMode.FourDirections,
            new Point(0, 0), new Point(0, 2), new Point(1, 1));
    }

    [TestMethod]
    public void GetNeighbors_EightDirectionsInCenter_ReturnsEightNeighbors()
    {
        AssertNeighbors(
            new Grid(3, 3),
            new Point(1, 1),
            MovementMode.EightDirections,
            new Point(0, 0), new Point(0, 1), new Point(0, 2),
            new Point(1, 0), new Point(1, 2),
            new Point(2, 0), new Point(2, 1), new Point(2, 2));
    }

    [TestMethod]
    public void GetNeighbors_EightDirectionsInCorner_ReturnsThreeNeighbors()
    {
        AssertNeighbors(
            new Grid(3, 3),
            new Point(0, 0),
            MovementMode.EightDirections,
            new Point(0, 1), new Point(1, 0), new Point(1, 1));
    }

    [TestMethod]
    public void GetNeighbors_EightDirectionsOnEdge_ReturnsFiveNeighbors()
    {
        AssertNeighbors(
            new Grid(3, 3),
            new Point(0, 1),
            MovementMode.EightDirections,
            new Point(0, 0), new Point(0, 2),
            new Point(1, 0), new Point(1, 1), new Point(1, 2));
    }

    [TestMethod]
    public void GetNeighbors_NonSquareGridInBottomRightCorner_ReturnsTwoNeighbors()
    {
        AssertNeighbors(
            new Grid(5, 3),
            new Point(2, 4),
            MovementMode.FourDirections,
            new Point(1, 4), new Point(2, 3));
    }

    private static void AssertNeighbors(Grid grid, Point point, MovementMode mode, params Point[] expected)
    {
        List<Point> neighbors = grid.GetNeighbors(point, mode).ToList();

        CollectionAssert.AreEquivalent(expected, neighbors);
    }
}
