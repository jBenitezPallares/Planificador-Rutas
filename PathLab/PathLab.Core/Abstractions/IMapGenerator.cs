using PathLab.Core.Structures;

namespace PathLab.Core.Abstractions;

public interface IMapGenerator
{
    Grid Generate(int width, int height);
}