namespace PathLab.Core.Generators;

/// <summary>
/// How much of a generated map is covered by each kind of obstacle or terrain.
/// </summary>
/// <remarks>
/// Every value is a fraction of the total number of cells, between 0 and 1 (0.2 means 20 %).
/// The three values together cannot add up to more than 1, because there would be no cells
/// left to hold them; the generator rejects such options with an
/// <see cref="ArgumentOutOfRangeException"/>. Cells not covered by any percentage stay free.
/// <para>
/// The amounts are exact, not probabilistic: each kind gets the percentage times the total
/// number of cells, rounded. Since each amount is rounded on its own, the last kind can end
/// up with one cell less than requested when the rounding adds up to more than the total.
/// </para>
/// </remarks>
/// <param name="WallPercentage">Fraction of the cells that become walls.</param>
/// <param name="MudPercentage">Fraction of the cells that become mud. Zero by default.</param>
/// <param name="WaterPercentage">Fraction of the cells that become water. Zero by default.</param>
public record MapGenerationOptions(
    double WallPercentage,
    double MudPercentage = 0,
    double WaterPercentage = 0);
