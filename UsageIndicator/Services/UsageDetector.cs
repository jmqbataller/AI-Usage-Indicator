using System.Text.RegularExpressions;
using UsageIndicator.Models;

namespace UsageIndicator.Services;

public static partial class UsageDetector
{
    public static UsageSnapshot Detect(string pageText)
    {
        var snapshot = new UsageSnapshot { UpdatedAt = DateTimeOffset.Now };
        var clean = Regex.Replace(pageText ?? string.Empty, @"\s+", " ").Trim();
        if (string.IsNullOrWhiteSpace(clean))
        {
            snapshot.Status = "Waiting for the signed-in ChatGPT page to load.";
            return snapshot;
        }

        var plan = FindPlanAllowance(clean);
        snapshot.PlanName = plan.name;
        snapshot.WorkPercent = plan.percent;
        snapshot.WorkReset = plan.reset;
        snapshot.UsageInfo = plan.percent is int percent ? $"{100 - percent}% of this plan used" : "Waiting for plan data";
        var token = TokenRegex().Match(clean);
        if (token.Success) snapshot.TokenInfo = token.Groups[1].Value + " tokens shown by ChatGPT";

        snapshot.Status = plan.percent is not null
            ? $"Live {plan.name.ToLowerInvariant()} detected from the signed-in ChatGPT page."
            : "Signed in. Open the ChatGPT usage dashboard so its plan limit can be detected.";
        return snapshot;
    }

    private static (int? percent, string reset, string name) FindPlanAllowance(string text)
    {
        var match = PlanLimitRegex().Match(text);
        if (!match.Success) return (null, "Not shown", "Plan limit");
        int? percent = int.TryParse(match.Groups["percent"].Value, out var value) && value is >= 0 and <= 100 ? value : null;
        return (percent, match.Groups["reset"].Value.Trim(), match.Groups["name"].Value.Trim());
    }

    [GeneratedRegex(@"\b(?<name>(?:weekly|monthly|daily)\s+limit)\b.{0,260}?\breset(?:s)?\s+in\s+(?<reset>\d+\s*[dhm](?:\s*\d+\s*[dhm])*)\b.{0,120}?\b(?<percent>\d{1,3})\s*%\s*left\b", RegexOptions.IgnoreCase)]
    private static partial Regex PlanLimitRegex();
    [GeneratedRegex(@"\b([\d,]+)\s+tokens?\b", RegexOptions.IgnoreCase)]
    private static partial Regex TokenRegex();
}
