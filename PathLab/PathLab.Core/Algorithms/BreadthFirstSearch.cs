using System.Diagnostics;
using PathLab.Core.Structures;

namespace PathLab.Core.Algorithms;

/// <summary>
/// Finds the path with the fewest cells, without looking at what each cell costs.
/// </summary>
/// <remarks>
/// It explores the grid in rings around the start using a plain first-in, first-out queue, and
/// each cell is discovered only once, by the shortest route in cells. That makes it the cheapest
/// algorithm to run, but it ignores mud and water: it can return a path with fewer cells that
/// costs more than another route. The cost it reports is the real cost of the path it found,
/// so the result can be compared with the one from <see cref="Dijkstra"/>.
/// </remarks>
public class BreadthFirstSearch: PathFinderBase
{
    /// <summary>
    /// Creates a breadth-first finder.
    /// </summary>
    /// <param name="movementMode">Four directions, or eight when diagonals are allowed.</param>
    public BreadthFirstSearch(MovementMode movementMode) : base(movementMode) { }

    /// <inheritdoc />
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
