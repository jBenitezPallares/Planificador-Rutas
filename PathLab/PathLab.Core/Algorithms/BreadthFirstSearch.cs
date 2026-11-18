using System.Diagnostics;
using PathLab.Core.Abstractions;
using PathLab.Core.Structures;

namespace PathLab.Core.Algorithms;

public class BreadthFirstSearch: PathFinderBase
{
    public BreadthFirstSearch(MovementMode movementMode) : base(movementMode) { }
    
    protected override SearchResult Search(Grid grid, Point start, Point goal, Stopwatch stopwatch)
    {
        List<Point> explored = new List<Point>();
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

            foreach (Point neighbor in grid.GetNeighbors(current, MovementMode))
            {
                if (!double.IsPositiveInfinity(distance[neighbor.Row, neighbor.Column]))
                {
                    continue;
                }
                
                distance[neighbor.Row, neighbor.Column] = distance[current.Row, current.Column] + grid.StepCost(current, neighbor);
                previous[neighbor.Row, neighbor.Column] = current;
                queue.Enqueue(neighbor);
            }
        }
        
        return SearchResult.NotFound(explored, stopwatch.ElapsedMilliseconds);
    }
}