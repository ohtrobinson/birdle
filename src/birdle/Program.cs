using System;
using birdle;
using birdle.Data;
using birdle.GameModes;
using Pie;
using Pie.Windowing;

bool firstTimeLaunch = false;

GameSettings settings;

try
{
    if (!GameSettings.TryLoad(BirdleGame.ConfigFile, out settings))
    {
        settings = GameSettings.Default;
        firstTimeLaunch = true;

        settings.Save(BirdleGame.ConfigFile);
#if !DEBUG
        MessageBox.Show(MessageBox.MessageBoxType.Information, "First-time launch",
            "Welcome! You won't see this message again.\nPlease read the README file for a quick explanation of the the game's behaviour,\nthis will eventually be made clearer.");
#endif
    }
}
catch (Exception e)
{
    settings = GameSettings.Default;
    settings.Save(BirdleGame.ConfigFile);
    firstTimeLaunch = true;
    
    MessageBox.Show(MessageBox.MessageBoxType.Warning, "Configuration error", $"There was an error reading the configuration file: {e.Message}\nYour settings have been reset.");
}

#if DEBUG
BirdleGame.Run(settings, new MenuMode());
#else
try
{
    BirdleGame.Run(settings, new BootMode(firstTimeLaunch));
}
catch (Exception e)
{
    BirdleGame.Log(LogType.Error, e.ToString());
    MessageBox.Show(MessageBox.MessageBoxType.Error, "Oops!", "The game crashed. Please open a GitHub issue and report this to the developer:\n" + e);
}
#endif