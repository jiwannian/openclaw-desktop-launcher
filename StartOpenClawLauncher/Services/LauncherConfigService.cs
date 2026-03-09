using System.Globalization;
using System.IO;
using System.Text.Json;
using StartOpenClawLauncher.Models;

namespace StartOpenClawLauncher.Services;

public sealed class LauncherConfigService
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNamingPolicy = JsonNamingPolicy.CamelCase
    };

    private readonly string _configPath = Path.Combine(AppContext.BaseDirectory, "config.json");

    public LauncherSettings LoadOrCreate()
    {
        if (!File.Exists(_configPath))
        {
            var defaults = CreateDefaultSettings();
            Save(defaults);
            return defaults;
        }

        try
        {
            var json = File.ReadAllText(_configPath);
            return JsonSerializer.Deserialize<LauncherSettings>(json, JsonOptions) ?? CreateDefaultSettings();
        }
        catch
        {
            var fallback = CreateDefaultSettings();
            Save(fallback);
            return fallback;
        }
    }

    public void Save(LauncherSettings settings)
    {
        var json = JsonSerializer.Serialize(settings, JsonOptions);
        File.WriteAllText(_configPath, json);
    }

    private static LauncherSettings CreateDefaultSettings()
    {
        var settings = new LauncherSettings
        {
            LanguageCode = DetectSystemLanguageCode()
        };

        return settings;
    }

    private static string DetectSystemLanguageCode()
    {
        var languageName = CultureInfo.CurrentUICulture.Name;
        var isoCode = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName;

        if (languageName.StartsWith("zh", StringComparison.OrdinalIgnoreCase))
        {
            return "zh-CN";
        }

        return isoCode switch
        {
            "hi" => "hi",
            "es" => "es",
            "ar" => "ar",
            "ru" => "ru",
            "pt" => "pt",
            "fr" => "fr",
            "it" => "it",
            "ja" => "ja",
            _ => "en"
        };
    }
}
