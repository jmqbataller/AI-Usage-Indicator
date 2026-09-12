namespace UsageIndicator.Models;

public sealed class UsageSnapshot
{
    public int? WorkPercent { get; set; }
    public string WorkReset { get; set; } = "Waiting for dashboard";
    public string PlanName { get; set; } = "Plan limit";
    public string TokenInfo { get; set; } = "Not provided by ChatGPT";
    public string UsageInfo { get; set; } = "Waiting for plan data";
    public DateTimeOffset? UpdatedAt { get; set; }
    public string Status { get; set; } = "Sign in, then open the ChatGPT usage dashboard.";
}
