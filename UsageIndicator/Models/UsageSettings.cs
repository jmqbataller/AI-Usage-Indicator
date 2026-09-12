namespace UsageIndicator.Models;

public sealed class UsageSettings
{
    public int CodexPercent { get; set; } = 100;
    public string CodexReset { get; set; } = "Not set";
    public int WorkPercent { get; set; } = 100;
    public string WorkReset { get; set; } = "Not set";
    public double Left { get; set; } = 12;
    public double Top { get; set; } = 12;
}
