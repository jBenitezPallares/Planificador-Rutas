namespace PathLab.Core;

public interface IPathFinder
{
    SearchResult Find(Grid grid, Point start, Point goal);
}