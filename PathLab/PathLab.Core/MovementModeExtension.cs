namespace PathLab.Core;

public static class MovementModeExtensions
{
    public const double DiagonalFactor = 1.414;
    public const double OrthogonalFactor = 1;
    
    private static readonly (int Row, int Column)[] FourDirectionOffsets =
    {
        (-1, 0), (1, 0), (0, -1), (0, 1)
    };

    private static readonly (int Row, int Column)[] EightDirectionOffsets =
    {
        (-1, 0), (1, 0), (0, -1), (0, 1),
        (-1, -1), (-1, 1), (1, -1), (1, 1)
    };

    public static IReadOnlyList<(int Row, int Column)> GetOffsets(this MovementMode mode) => mode switch
    {
        MovementMode.FourDirections => FourDirectionOffsets,
        MovementMode.EightDirections => EightDirectionOffsets,
        _ => throw new ArgumentOutOfRangeException(nameof(mode))
    };
}