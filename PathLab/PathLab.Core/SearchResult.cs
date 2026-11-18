namespace PathLab.Core;

public class SearchResult
{
    public bool Found { get; }
    public IReadOnlyList<Point> Path { get; }
    public double Cost { get; }
    public IReadOnlyList<Point> ExplorationOrder { get; }
    public long ElapsedMs { get; }

    public SearchResult(bool found, IReadOnlyList<Point> path, double cost, IReadOnlyList<Point> explorationOrder, long ms)
    { 
        Found = found;
        Path = path;
        Cost = cost;
        ExplorationOrder = explorationOrder;
        ElapsedMs = ms;
    }
    
    public static SearchResult NotFound(IReadOnlyList<Point> explored, long elapsedMs)
    {
        return new SearchResult(false, new List<Point>(), 0, explored, elapsedMs);
    }
}

