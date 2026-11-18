using System.Diagnostics;

namespace PathLab.Core;

public class BreadthFirstSearch: IPathFinder
{
    private readonly MovementMode _movementMode;

    public BreadthFirstSearch(MovementMode movementMode)
    {
        _movementMode = movementMode;
    }
    
    public SearchResult Find(Grid grid, Point start, Point goal)
    {
        if (!grid.Contains(start))
            throw new ArgumentOutOfRangeException(nameof(start), start, "El punto de inicio está fuera de la grilla");

        if (!grid.Contains(goal))
            throw new ArgumentOutOfRangeException(nameof(goal), goal, "El punto de destino está fuera de la grilla");

        Stopwatch stopwatch = Stopwatch.StartNew();
        List<Point> explored = new List<Point>();
        
        if (grid.IsWall(start) || grid.IsWall(goal))
        {
            return SearchResult.NotFound(explored, stopwatch.ElapsedMilliseconds);
        }
 
        double[,] distance = new double[grid.Height, grid.Width];
        Point?[,] previous = new Point? [grid.Height, grid.Width];
        FillWithInfinity(distance);
        distance[start.Row, start.Column] = 0;
        
        Queue<Point> queue = new Queue<Point>();
        queue.Enqueue(start);
        
        while (queue.Count > 0)
        {
            Point current = queue.Dequeue();
            explored.Add(current);

            if (current == goal)
            { 
                return new SearchResult(true, BuildPath(previous, goal), distance[goal.Row, goal.Column], explored, stopwatch.ElapsedMilliseconds);
            }

            foreach (Point neighbor in grid.GetNeighbors(current, _movementMode))
            {
                if (!double.IsPositiveInfinity(distance[neighbor.Row, neighbor.Column]))
                {
                    continue;
                }
                
                distance[neighbor.Row, neighbor.Column] = distance[current.Row, current.Column] + StepCost(grid, current, neighbor);
                previous[neighbor.Row, neighbor.Column] = current;
                queue.Enqueue(neighbor);
            }
        }
        
        return SearchResult.NotFound(explored, stopwatch.ElapsedMilliseconds);
    }
    
    private static void FillWithInfinity(double[,] distance)
    {
        for (int row = 0; row < distance.GetLength(0); row++)
        for (int column = 0; column < distance.GetLength(1); column++)
            distance[row, column] = double.PositiveInfinity;
    }
    
    private static List<Point> BuildPath(Point?[,] previous, Point goal)
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
    
    private static double StepCost(Grid grid, Point from, Point to)
        {
            bool isDiagonal = from.Row != to.Row && from.Column != to.Column;
            double multiplyFactor = MovementModeExtensions.OrthogonalFactor;
    
            if (isDiagonal)
            {
                multiplyFactor = MovementModeExtensions.DiagonalFactor;
            }
    
            return grid.GetCost(to) * multiplyFactor;
        }
}