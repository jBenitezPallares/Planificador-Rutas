using System.Diagnostics;
using PathLab.Core.Abstractions;
using PathLab.Core.Structures;

namespace PathLab.Core.Algorithms;

/// <summary>
/// Finds the cheapest path, guiding the search towards the goal with a heuristic.
/// </summary>
/// <remarks>
/// Cells wait in a priority queue ordered by <c>f = g + h</c>: the cost already paid plus the
/// estimate of what is left. Cells that head for the goal get a lower <c>f</c> and are taken first,
/// so fewer cells are explored than with <see cref="Dijkstra"/>.
/// <para>
/// A cell is closed the first time it is taken out of the queue and never reopened. That keeps
/// the search fast, but it only returns the cheapest path when the heuristic is admissible and
/// consistent (see <see cref="IHeuristic"/>), so each movement mode needs a matching heuristic.
/// <see cref="Heuristics.HeuristicFactory"/> picks the right one.
/// </para>
/// </remarks>
public class AStar : PathFinderBase
{
    private readonly IHeuristic _heuristic;

    /// <summary>
    /// Creates an A* finder.
    /// </summary>
    /// <param name="movementMode">Four directions, or eight when diagonals are allowed.</param>
    /// <param name="heuristic">The estimate of the cost left. It must be valid for the movement mode.</param>
    /// <exception cref="ArgumentNullException"><paramref name="heuristic"/> is <c>null</c>.</exception>
    public AStar(MovementMode movementMode, IHeuristic heuristic) : base(movementMode)
    {
        _heuristic = heuristic ?? throw new ArgumentNullException(nameof(heuristic), "La heurística no puede ser nula");
    }

    /// <inheritdoc />
    /// <remarks>
    /// The cost of the result is the real cost of the path (the cost paid up to the goal), not the
    /// priority the queue used to order the cells.
    /// </remarks>
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
