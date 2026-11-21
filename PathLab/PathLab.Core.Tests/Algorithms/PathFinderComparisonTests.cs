using PathLab.Core.Algorithms;
using PathLab.Core.Algorithms.Heuristics;
using PathLab.Core.Generators;
using PathLab.Core.Structures;

namespace PathLab.Core.Tests.Algorithms;

[TestClass]
public class PathFinderComparisonTests
{
    [TestMethod]
    [DataRow(MovementMode.FourDirections)]
    [DataRow(MovementMode.EightDirections)]
    public void Find_AStarAndDijkstra_SameResultOnTwoHundredRandomMaps(MovementMode mode)
    {
        for (int i = 0; i < 200; i++)
        {
            Grid grid = GenerateRandomMap(i);
            Point start = new Point(0, 0);
            Point goal = new Point(grid.Height - 1, grid.Width - 1);

            SearchResult dijkstra = new Dijkstra(mode).Find(grid, start, goal);
            SearchResult aStar = new AStar(mode, HeuristicFactory.CreateFor(mode)).Find(grid, start, goal);
            
            Assert.AreEqual(dijkstra.Found, aStar.Found);
            if (dijkstra.Found)
            {
                Assert.AreEqual(dijkstra.Cost, aStar.Cost, 0.001);
            }
        }
    }

    private static Grid GenerateRandomMap(int seed)
    {
        MapGenerationOptions options = new MapGenerationOptions(0.2, 0.1, 0.05);
        return new RandomMapGenerator(options, new Random(seed)).Generate(20, 20);
    }
}