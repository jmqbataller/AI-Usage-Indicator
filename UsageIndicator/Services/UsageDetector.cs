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

        (snapshot.ChatPercent, snapshot.ChatReset) = FindAllowance(clean, "Chat");
        (snapshot.WorkPercent, snapshot.WorkReset) = FindAllowance(clean, "Work");
        var token = TokenRegex().Match(clean);
        if (token.Success) snapshot.TokenInfo = token.Groups[1].Value + " tokens shown by ChatGPT";

        snapshot.Status = snapshot.ChatPercent is not null || snapshot.WorkPercent is not null
            ? "Live values detected from the signed-in ChatGPT page."
            : "Signed in. Open the ChatGPT usage dashboard so its Chat and Work values can be detected.";
        return snapshot;
    }

    private static (int? percent, string reset) FindAllowance(string text, string label)
    {
        var labelMatch = Regex.Match(text, $@"\b{Regex.Escape(label)}\b(?<near>.{{0,260}})", RegexOptions.IgnoreCase);
        if (!labelMatch.Success) return (null, "Not shown");
        var near = labelMatch.Groups["near"].Value;
        var percentMatch = PercentRegex().Match(near);
        var resetMatch = ResetRegex().Match(near);
        return (percentMatch.Success && int.TryParse(percentMatch.Groups[1].Value, out var value) && value is >= 0 and <= 100 ? value : null,
            resetMatch.Success ? resetMatch.Groups[1].Value.Trim().Trim('.') : "Not shown");
    }

    [GeneratedRegex(@"\b(\d{1,3})\s*%")]
    private static partial Regex PercentRegex();
    [GeneratedRegex(@"(?:reset(?:s|ting)?(?:\s+in)?|available(?:\s+again)?(?:\s+in)?)\s*[:\-]?\s*([^|•]{1,70})", RegexOptions.IgnoreCase)]
    private static partial Regex ResetRegex();
    [GeneratedRegex(@"\b([\d,]+)\s+tokens?\b", RegexOptions.IgnoreCase)]
    private static partial Regex TokenRegex();
}
