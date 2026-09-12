using System.IO;
using System.Text.Json;
using UsageIndicator.Models;

namespace UsageIndicator.Services;

public static class SettingsStore
{
    private static readonly string Folder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "ChatGPTUsageIndicator");
    private static readonly string FilePath = Path.Combine(Folder, "settings.json");

    public static UsageSettings Load()
    {
        try
        {
            if (File.Exists(FilePath))
                return JsonSerializer.Deserialize<UsageSettings>(File.ReadAllText(FilePath)) ?? new UsageSettings();
        }
        catch { }
        return new UsageSettings();
    }

    public static void Save(UsageSettings settings)
    {
        Directory.CreateDirectory(Folder);
        File.WriteAllText(FilePath, JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true }));
    }
}
