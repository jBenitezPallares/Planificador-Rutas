using System.Diagnostics;
using PathLab.Core.Abstractions;
using PathLab.Core.Structures;

namespace PathLab.Core.Algorithms;

public class AStar : PathFinderBase
{
    private readonly IHeuristic _heuristic;
    public AStar(MovementMode movementMode, IHeuristic heuristic) : base(movementMode) 
    {
        _heuristic = heuristic ?? throw new ArgumentNullException(nameof(heuristic), "La heurística no puede ser nula");
    }
 
    protected override SearchResult Search(Grid grid, Point start, Point goal, Stopwatch stopwatch)
    {
        List<Point> explored = new List<Point>();
        double[,] distance = new double[grid.Height, grid.Width];
        bool[,] visited = new bool[grid.Height, grid.Width];
        Point?[,] previous = new Point? [grid.Height, grid.Width];
        FillWithInfinity(distance);
        distance[start.Row, start.Column] = 0;
        
        MinHeap<(Point point, double score)> queue =
            new MinHeap<(Point point, double score)>(
                Comparer<(Point point, double score)>.Create((a, b) => a.score.CompareTo(b.score)));
        double startScore = _heuristic.Estimate(start, goal);
        queue.Insert((start, startScore));

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
                return new SearchResult(true, BuildPath(previous, goal), distance[goal.Row, goal.Column], explored, stopwatch.ElapsedMilliseconds);
            }

            foreach (Point neighbor in grid.GetNeighbors(current, MovementMode))
            {
                double newCost = distance[current.Row, current.Column] + grid.StepCost(current, neighbor);

                if (newCost < distance[neighbor.Row, neighbor.Column])
                {
                    distance[neighbor.Row, neighbor.Column] = newCost;
                    previous[neighbor.Row, neighbor.Column] = current;
                    double score = newCost + _heuristic.Estimate(neighbor, goal);
                    queue.Insert((neighbor, score));
                }
            }
        }
        
        return SearchResult.NotFound(explored, stopwatch.ElapsedMilliseconds);
    }
}