using System.Diagnostics;

namespace PathLab.Core;

public class Dijkstra: IPathFinder
{
    private readonly MovementMode _movementMode;

    public Dijkstra(MovementMode movementMode)
    {
        _movementMode = movementMode;
    }

    public SearchResult Find(Grid grid, Point start, Point goal)
    {
        Stopwatch stopwatch = Stopwatch.StartNew();
        List<Point> explored = new List<Point>();
        if (grid.IsWall(start) || grid.IsWall(goal))
        {
            return SearchResult.NotFound(explored, stopwatch.ElapsedMilliseconds);
        }
        
        double [,]  distance = new double[grid.Height, grid.Width];
        bool [,] visited = new bool[grid.Height, grid.Width];
        Point? [,] previous = new Point? [grid.Height, grid.Width];
        FillWithInfinity(distance);
        distance [start.Row, start.Column] = 0;
        
        MinHeap<(Point point, double cost)> queue = new MinHeap<(Point point, double cost)>(Comparer<(Point point, double cost)>.Create((a,b) => a.cost.CompareTo(b.cost)));
        queue.Insert((start,0));

        while (!queue.IsEmpty)
        {
            (Point current, double cost) = queue.ExtractMin();
            
            if(visited[current.Row, current.Column])
            {
                continue;
            }
            visited[current.Row, current.Column] = true;
            explored.Add(current);

            if (current == goal)
            {
                return new SearchResult(true, BuildPath(previous, goal), cost, explored, stopwatch.ElapsedMilliseconds);
            }

            foreach (Point neighbor in grid.GetNeighbors(current, _movementMode))
            {
                double newCost = distance[current.Row, current.Column] + StepCost(grid, current, neighbor);

                if (newCost < distance[neighbor.Row, neighbor.Column])
                {
                    distance[neighbor.Row, neighbor.Column] = newCost;
                    previous[neighbor.Row, neighbor.Column] = current;
                    queue.Insert((neighbor, distance [neighbor.Row, neighbor.Column]));
                }
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