using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using birdle.GameModes;

namespace birdle.Data;

public struct GameSettings
{
    public Size WindowSize;

    public bool WindowFullscreen;

    public Point WindowPosition;
    
    public bool DarkMode;
    
    public Difficulty Difficulty;

    public float UiScale;

    public void Save(string path)
    {
        Dictionary<string, object> config = new Dictionary<string, object>()
        {
            ["window.size"] = new []{ WindowSize.Width, WindowSize.Height },
            ["window.fullscreen"] = WindowFullscreen,
            ["window.position"] = new []{ WindowPosition.X, WindowPosition.Y },
            ["darkmode"] = DarkMode,
            ["difficulty"] = Difficulty,
            ["ui.scale"] = UiScale
        };
        
        File.WriteAllText(path, QuickConfig.ToQuickConfig(config));
    }

    public static bool TryLoad(string path, out GameSettings settings)
    {
        settings = Default;
        
        if (!File.Exists(path))
            return false;

        Dictionary<string, object> config = QuickConfig.Parse(File.ReadAllText(path));

        settings.WindowSize.Width = (int) (double) ((object[]) config["window.size"])[0];
        settings.WindowSize.Height = (int) (double) ((object[]) config["window.size"])[1];
        settings.WindowFullscreen = (bool) config["window.fullscreen"];
        settings.WindowPosition.X = (int) (double) ((object[]) config["window.position"])[0];
        settings.WindowPosition.Y = (int) (double) ((object[]) config["window.position"])[1];
        settings.DarkMode = (bool) config["darkmode"];
        settings.Difficulty = Enum.Parse<Difficulty>((string) config["difficulty"], true);
        settings.UiScale = (float) (double) config["ui.scale"];

        return true;
    }

    public static GameSettings Default => new GameSettings()
    {
        WindowSize = new Size(800, 600),
        WindowFullscreen = false,
        WindowPosition = new Point(-1),
        DarkMode = false,
        Difficulty = Difficulty.Normal,
        UiScale = 1
    };
}