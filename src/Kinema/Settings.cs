using System.Text.Json;

namespace Kinema;

public class Settings
{
    public string Engine { get; set; } = "mvtools";
    public int TargetFps { get; set; } = 60;
    public int MaxWidth { get; set; } = 1920;
    public bool AutoInterp { get; set; } = false;
    public bool Vsr { get; set; } = false;
    public bool RtxHdr { get; set; } = false;
    public double SubScale { get; set; } = 1.0;
    public int SubPos { get; set; } = 100;
    public string SubFont { get; set; } = "Microsoft JhengHei";
    public string SubColor { get; set; } = "#FFFFFF";
    public int SubBorder { get; set; } = 3;
    public bool SubAssOverride { get; set; } = false;
    public int Volume { get; set; } = 100;

    public static string Dir => Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "KINEMA");
    static string FilePath => Path.Combine(Dir, "settings.json");

    public static Settings Load()
    {
        try
        {
            if (File.Exists(FilePath))
                return JsonSerializer.Deserialize<Settings>(File.ReadAllText(FilePath)) ?? new Settings();
        }
        catch { }
        return new Settings();
    }

    public void Save()
    {
        try
        {
            Directory.CreateDirectory(Dir);
            File.WriteAllText(FilePath, JsonSerializer.Serialize(this, new JsonSerializerOptions { WriteIndented = true }));
        }
        catch { }
    }
}
