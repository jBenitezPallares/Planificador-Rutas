namespace PathLab.Core.Structures;

public static class CellTypeExtensions
{
    public static int GetCost(this CellType type) => type switch
    {
        CellType.Free => 1,
        CellType.Mud => 5,
        CellType.Water => 10,
        _ => throw new ArgumentOutOfRangeException(nameof(type))
    };
}