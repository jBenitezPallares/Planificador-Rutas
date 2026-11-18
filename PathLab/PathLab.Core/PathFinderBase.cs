using System.Diagnostics;

namespace PathLab.Core;

public abstract class PathFinderBase : IPathFinder
{
    protected readonly MovementMode MovementMode;

    protected PathFinderBase(MovementMode movementMode)
    {
        MovementMode = movementMode;
    }

    public SearchResult Find(Grid grid, Point start, Point goal)
    {
        if (!grid.Contains(start))
            throw new ArgumentOutOfRangeException(nameof(start), start, "El punto de inicio está fuera de la grilla");

        if (!grid.Contains(goal))
            throw new ArgumentOutOfRangeException(nameof(goal), goal, "El punto de destino está fuera de la grilla");

        Stopwatch stopwatch = Stopwatch.StartNew();

        if (grid.IsWall(start) || grid.IsWall(goal))
            return SearchResult.NotFound(new List<Point>(), stopwatch.ElapsedMilliseconds);

        return Search(grid, start, goal, stopwatch);
    }

    protected abstract SearchResult Search(Grid grid, Point start, Point goal, Stopwatch stopwatch);

    protected static void FillWithInfinity(double[,] distance)
    {
        for (int row = 0; row < distance.GetLength(0); row++)
        for (int column = 0; column < distance.GetLength(1); column++)
            distance[row, column] = double.PositiveInfinity;
    }

    protected static List<Point> BuildPath(Point?[,] previous, Point goal)
    {
        List<Point> path = new List<Point>();
        Point? current = goal;

        while (current != null)
        {
            path.Add(current.Value);
            current = previous[current.Value.Row, current.Value.Column];
        }

        path.Reverse();
        return path;
    }
}