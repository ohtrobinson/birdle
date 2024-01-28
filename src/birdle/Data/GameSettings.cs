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

    public void Save(string path)
    {
        Dictionary<string, string> config = new Dictionary<string, string>()
        {
            //["window.size.width"] = WindowSize.Width.ToString(),
            //["window.size.height"] = WindowSize.Height.ToString(),
            //["window.fullscreen"] = WindowFullscreen ? "true" : "false",
            //["window.position.x"] = WindowPosition.X.ToString(),
            //["window.position.y"] = WindowPosition.Y.ToString(),
            ["darkmode"] = DarkMode ? "true" : "false",
            ["difficulty"] = Difficulty.ToString().ToLower()
        };
        
        File.WriteAllText(path, QuickConfig.ToQuickConfig(config));
    }

    public static bool TryLoad(string path, out GameSettings settings)
    {
        settings = Default;
        
        if (!File.Exists(path))
            return false;

        Dictionary<string, string> config = QuickConfig.Parse(File.ReadAllText(path));
        
        //settings.WindowSize.Width = int.Parse(config["window.size.width"]);
        //settings.WindowSize.Height = int.Parse(config["window.size.height"]);
        //settings.WindowFullscreen = bool.Parse(config["window.fullscreen"]);
        //settings.WindowPosition.X = int.Parse(config["window.position.x"]);
        //settings.WindowPosition.Y = int.Parse(config["window.position.y"]);
        settings.DarkMode = bool.Parse(config["darkmode"]);
        settings.Difficulty = Enum.Parse<Difficulty>(config["difficulty"], true);

        return true;
    }

    public static GameSettings Default => new GameSettings()
    {
        WindowSize = new Size(800, 600),
        WindowFullscreen = false,
        DarkMode = false,
        Difficulty = Difficulty.Normal
    };
}