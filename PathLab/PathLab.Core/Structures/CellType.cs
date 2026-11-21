namespace PathLab.Core.Structures;

/// <summary>
/// The kind of terrain of a cell, which decides whether it can be crossed and what it costs.
/// </summary>
/// <remarks>
/// <see cref="Free"/> is the first value, so a new <see cref="Grid"/> starts with every cell free.
/// The cost of entering a cell is read with <see cref="CellTypeExtensions.GetCost"/>.
/// </remarks>
public enum CellType
{
    /// <summary>Open ground. Entering it costs 1. It is the default terrain.</summary>
    Free,

    /// <summary>Can be crossed, but entering it costs 5.</summary>
    Mud,

    /// <summary>Can be crossed, but entering it costs 10.</summary>
    Water,

    /// <summary>Cannot be entered, so it has no cost and no search ever explores it.</summary>
    Wall
}
