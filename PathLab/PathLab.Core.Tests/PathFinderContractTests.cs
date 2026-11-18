namespace PathLab.Core.Tests;

public abstract class PathFinderContractTests
{
    protected abstract IPathFinder CreateFinder(MovementMode mode);

    [TestMethod]
    public void Find_HandMadeMapFourDirections_ReturnsCostSeven()
    {
        Grid grid = BuildHandMadeMap();
        IPathFinder finder = CreateFinder(MovementMode.FourDirections);

        SearchResult result = finder.Find(grid, new Point(0, 0), new Point(3, 4));

        Assert.IsTrue(result.Found);
        Assert.AreEqual(7, result.Cost);
    }

    [TestMethod]
    public void Find_NoPathBetweenStartAndGoal_ReturnsNotFound()
    {
        Grid grid = new Grid(5, 4);
        for (int column = 0; column < 5; column++)
        {
            grid.SetCell(new Point(1, column), CellType.Wall);
        }
        IPathFinder finder = CreateFinder(MovementMode.EightDirections);

        SearchResult result = finder.Find(grid, new Point(0, 0), new Point(3, 4));

        Assert.IsFalse(result.Found);
        Assert.AreEqual(0, result.Cost);
        Assert.IsEmpty(result.Path);
    }

    [TestMethod]
    public void Find_StartEqualsGoal_ReturnsCostZeroAndSingleCellPath()
    {
        IPathFinder finder = CreateFinder(MovementMode.EightDirections);

        SearchResult result = finder.Find(new Grid(5, 4), new Point(0, 0), new Point(0, 0));

        Assert.IsTrue(result.Found);
        Assert.AreEqual(0, result.Cost);
        Assert.HasCount(1, result.Path);
    }

    [TestMethod]
    public void Find_StartOutsideGrid_ThrowsException()
    {
        IPathFinder finder = CreateFinder(MovementMode.EightDirections);

        ArgumentOutOfRangeException exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => finder.Find(new Grid(5, 4), new Point(6, 6), new Point(3, 4)));

        Assert.AreEqual("start", exception.ParamName);
    }
    
    [TestMethod]
    public void Find_GoalOutsideGrid_ThrowsException(){
        Grid grid = BuildHandMadeMap();
        IPathFinder finder = CreateFinder(MovementMode.FourDirections);

        ArgumentOutOfRangeException exception = Assert.ThrowsExactly<ArgumentOutOfRangeException>(
            () => finder.Find(grid, new Point(0, 0), new Point(6, 6)));

        Assert.AreEqual("goal", exception.ParamName);
    }
    
    [TestMethod]
    public void Find_StartIsWall_ReturnsNotFound()
    {
        Grid grid = BuildHandMadeMap();
        grid.SetCell(new Point(0,0), CellType.Wall);
        
        IPathFinder finder = CreateFinder(MovementMode.FourDirections);
        SearchResult result = finder.Find(grid, new Point(0, 0), new Point(3, 4));
        
        Assert.IsFalse(result.Found);
        Assert.AreEqual(0, result.Cost);
        Assert.IsEmpty(result.Path);
    }
    
    [TestMethod]
    public void Find_GoalIsWall_ReturnsNotFound(){
        Grid grid = BuildHandMadeMap();
        grid.SetCell(new Point(3,4), CellType.Wall);
        
        IPathFinder finder = CreateFinder(MovementMode.FourDirections);
        SearchResult result = finder.Find(grid, new Point(0, 0), new Point(3, 4));
        
        Assert.IsFalse(result.Found);
        Assert.AreEqual(0, result.Cost);
        Assert.IsEmpty(result.Path);
    }
    
    [TestMethod]
    public void Find_LargeOpenMap_ReportsPositiveElapsedTime()
    {
        Grid grid = new Grid(300, 300);

        IPathFinder finder = CreateFinder(MovementMode.FourDirections);
        SearchResult result = finder.Find(grid, new Point(0, 0), new Point(299, 299));

        Assert.IsTrue(result.Found);
        Assert.IsGreaterThan(0, result.ElapsedMs);
    }

    [TestMethod]
    public void Find_SmallMap_ReportsNonNegativeElapsedTime()
    {
        Grid grid = new Grid(3, 3);

        IPathFinder finder = CreateFinder(MovementMode.FourDirections);
        SearchResult result = finder.Find(grid, new Point(0, 0), new Point(2, 2));

        Assert.IsGreaterThanOrEqualTo(0, result.ElapsedMs);
    }
    
    protected static Grid BuildHandMadeMap()
    {
        Grid grid = new Grid(5, 4);
        foreach (Point wall in new[]
        {
            new Point(0, 3), new Point(1, 1), new Point(1, 3),
            new Point(2, 1), new Point(3, 3)
        })
        {
            grid.SetCell(wall, CellType.Wall);
        }
        return grid;
    }
}