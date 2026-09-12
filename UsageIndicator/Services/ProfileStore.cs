using System.IO;
using System.Text.Json;
using UsageIndicator.Models;

namespace UsageIndicator.Services;

public static class ProfileStore
{
    private static readonly string AppFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "AIUsageIndicator");
    private static readonly string DataPath = Path.Combine(AppFolder, "accounts.json");

    public static List<AccountProfile> Load()
    {
        try
        {
            return File.Exists(DataPath)
                ? JsonSerializer.Deserialize<List<AccountProfile>>(File.ReadAllText(DataPath)) ?? []
                : [];
        }
        catch { return []; }
    }

    public static void Save(List<AccountProfile> accounts)
    {
        Directory.CreateDirectory(AppFolder);
        File.WriteAllText(DataPath, JsonSerializer.Serialize(accounts, new JsonSerializerOptions { WriteIndented = true }));
    }

    public static string BrowserProfilePath(string accountId)
    {
        var path = Path.Combine(AppFolder, "browser-profiles", accountId);
        Directory.CreateDirectory(path);
        return path;
    }
}
