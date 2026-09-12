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
        snapshot.ChatExcludedFromPlan = Regex.IsMatch(clean, @"chat conversations? (?:are |is )?not included", RegexOptions.IgnoreCase);
        snapshot.ChatPercent = snapshot.ChatExcludedFromPlan ? null : plan.percent;
        snapshot.ChatReset = snapshot.ChatExcludedFromPlan ? "Not included in this plan" : plan.reset;
        snapshot.WorkPercent = plan.percent;
        snapshot.WorkReset = plan.reset;
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
        var percent = int.TryParse(match.Groups["percent"].Value, out var value) && value is >= 0 and <= 100 ? value : null;
        return (percent, match.Groups["reset"].Value.Trim(), match.Groups["name"].Value.Trim());
    }

    [GeneratedRegex(@"\b(?<name>(?:weekly|monthly|daily)\s+limit)\b.{0,260}?\breset(?:s)?\s+in\s+(?<reset>\d+\s*[dhm](?:\s*\d+\s*[dhm])*)\b.{0,120}?\b(?<percent>\d{1,3})\s*%\s*left\b", RegexOptions.IgnoreCase)]
    private static partial Regex PlanLimitRegex();
    [GeneratedRegex(@"\b([\d,]+)\s+tokens?\b", RegexOptions.IgnoreCase)]
    private static partial Regex TokenRegex();
}
