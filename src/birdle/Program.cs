using System;
using birdle;
using birdle.Data;
using birdle.GameModes;
using Pie;
using Pie.Windowing;

bool firstTimeLaunch = false;

if (!GameSettings.TryLoad(BirdleGame.ConfigFile, out GameSettings settings))
{
    settings = GameSettings.Default;
    firstTimeLaunch = true;
    
    settings.Save(BirdleGame.ConfigFile);
#if !DEBUG
    MessageBox.Show(MessageBox.MessageBoxType.Information, "First-time launch", "Welcome! You won't see this message again.\nPlease read the README file for a quick explanation of the the game's behaviour,\nthis will eventually be made clearer.");
#endif
}

#if DEBUG
BirdleGame.Run(settings, new BootMode(firstTimeLaunch));
#else
try
{
    BirdleGame.Run(settings, new BootMode(firstTimeLaunch));
}
catch (Exception e)
{
    BirdleGame.Log(LogType.Critical, e.ToString());
    MessageBox.Show(MessageBox.MessageBoxType.Error, "Oops!", "The game crashed. Please report this to the developer:\n" + e);
}
#endif