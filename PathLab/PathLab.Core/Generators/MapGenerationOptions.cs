namespace PathLab.Core.Generators;

public record MapGenerationOptions(
    double WallPercentage,
    double MudPercentage = 0,
    double WaterPercentage = 0);