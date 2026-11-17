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

    [TestMethod]
    public void GetNeighbors_FourDirectionsWithWallBesideCell_DoesNotReturnWall()
    {
        Grid grid = new(3, 3);
        grid.SetCell(new Point(1, 2), CellType.Wall);
        AssertNeighbors(
            grid,
            new Point(1, 1),
            MovementMode.FourDirections,
            new Point(0, 1), new Point(1, 0), new Point(2, 1)
            );
    }

    [TestMethod]
    public void GetNeighbors_EightDirectionsWithWallBesideCell_DoesNotReturnWall()
    {
        Grid grid = new(3, 3);
        grid.SetCell(new Point(1, 2), CellType.Wall);

        AssertNeighbors(
            grid,
            new Point(1, 1),
            MovementMode.EightDirections,
            new Point(0, 0), new Point(0, 1), new Point(0, 2),
            new Point(1, 0),
            new Point(2, 0), new Point(2, 1), new Point(2, 2));
    }

    [TestMethod]
    [DataRow(CellType.Free)]
    [DataRow(CellType.Mud)]
    [DataRow(CellType.Water)]
    public void GetNeighbors_FourDirectionsWithCrossableCellBeside_StillReturnsIt(CellType crossable)
    {
        Grid grid = new(3, 3);
        grid.SetCell(new Point(1, 2), crossable);

        AssertNeighbors(
            grid,
            new Point(1, 1),
            MovementMode.FourDirections,
            new Point(0, 1), new Point(1, 0), new Point(1, 2), new Point(2, 1));
    }

    [TestMethod]
    public void GetNeighbors_WallOverwrittenWithFree_ReturnsCellAgain()
    {
        Grid grid = new(3, 3);
        grid.SetCell(new Point(1, 2), CellType.Wall);
        grid.SetCell(new Point(1, 2), CellType.Free);

        AssertNeighbors(
            grid,
            new Point(1, 1),
            MovementMode.FourDirections,
            new Point(0, 1), new Point(1, 0), new Point(1, 2), new Point(2, 1));
    }

    [TestMethod]
    public void GetNeighbors_FourDirectionsWithAllNeighborsWalls_ReturnsNoNeighbors()
    {
        Grid grid = new(3, 3);
        grid.SetCell(new Point(0, 1), CellType.Wall);
        grid.SetCell(new Point(2, 1), CellType.Wall);
        grid.SetCell(new Point(1, 0), CellType.Wall);
        grid.SetCell(new Point(1, 2), CellType.Wall);

        AssertNeighbors(grid, new Point(1, 1), MovementMode.FourDirections);
    }

    [TestMethod]
    public void GetNeighbors_WallOnlyDiagonal_FourDirectionsStillReturnsAllNeighbors()
    {
        Grid grid = new(3, 3);
        grid.SetCell(new Point(0, 0), CellType.Wall);

        AssertNeighbors(
            grid,
            new Point(1, 1),
            MovementMode.FourDirections,
            new Point(0, 1), new Point(1, 0), new Point(1, 2), new Point(2, 1));
    }

    [TestMethod]
    public void SetCell_PointOutsideGrid_IsIgnored()
    {
        Grid grid = new(3, 3);

        grid.SetCell(new Point(5, 5), CellType.Wall);
        grid.SetCell(new Point(-1, 0), CellType.Wall);

        AssertNeighbors(
            grid,
            new Point(1, 1),
            MovementMode.FourDirections,
            new Point(0, 1), new Point(1, 0), new Point(1, 2), new Point(2, 1));
    }

    private static void AssertNeighbors(Grid grid, Point point, MovementMode mode, params Point[] expected)
    {
        List<Point> neighbors = grid.GetNeighbors(point, mode).ToList();

        CollectionAssert.AreEquivalent(expected, neighbors);
    }
}
