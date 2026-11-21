using System.Diagnostics;
using PathLab.Core.Abstractions;
using PathLab.Core.Structures;

namespace PathLab.Core.Algorithms;

public class Dijkstra : PathFinderBase
{
    public Dijkstra(MovementMode movementMode) : base(movementMode) { }

    protected override SearchResult Search(Grid grid, Point start, Point goal, Stopwatch stopwatch)
    {
        List<Point> explored = new List<Point>();
        double[,] distance = new double[grid.Height, grid.Width];
        bool[,] visited = new bool[grid.Height, grid.Width];
        Point?[,] previous = new Point? [grid.Height, grid.Width];
        FillWithInfinity(distance);
        distance[start.Row, start.Column] = 0;
        
        MinHeap<(Point point, double cost)> queue =
            new MinHeap<(Point point, double cost)>(
                Comparer<(Point point, double cost)>.Create((a, b) => a.cost.CompareTo(b.cost)));
        queue.Insert((start, 0));

        while (!queue.IsEmpty)
        {
            (Point current, _) = queue.ExtractMin();

            if (visited[current.Row, current.Column])
            {
                continue;
            }

            visited[current.Row, current.Column] = true;
            explored.Add(current);

            if (current == goal)
            {
                return new SearchResult(true, BuildPath(previous, goal), distance [goal.Row, goal.Column], explored, stopwatch.ElapsedMilliseconds);
            }

            foreach (Point neighbor in grid.GetNeighbors(current, MovementMode))
            {
                double newCost = distance[current.Row, current.Column] + grid.StepCost(current, neighbor);

                if (newCost < distance[neighbor.Row, neighbor.Column])
                {
                    distance[neighbor.Row, neighbor.Column] = newCost;
                    previous[neighbor.Row, neighbor.Column] = current;
                    queue.Insert((neighbor, distance[neighbor.Row, neighbor.Column]));
                }
            }
        }

        return SearchResult.NotFound(explored, stopwatch.ElapsedMilliseconds);
    }
}