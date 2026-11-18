using PathLab.Core.Algorithms;
using PathLab.Core.Structures;

namespace PathLab.Core.Abstractions;

public interface IPathFinder
{
    SearchResult Find(Grid grid, Point start, Point goal);
}