using System;
using birdle;
using birdle.Data;
using birdle.GameModes;
using Pie.Windowing;

if (!GameSettings.TryLoad(BirdleGame.ConfigFile, out GameSettings settings))
{
    settings = GameSettings.Default;
    settings.Save(BirdleGame.ConfigFile);
    
    MessageBox.Show(MessageBox.MessageBoxType.Information, "First-time launch", "Welcome! You won't see this message again.\nPlease read the README file for a quick explanation of the the game's behaviour,\nthis will eventually be made clearer.");
}

#if DEBUG
BirdleGame.Run(settings, new BirdleMode(settings.Difficulty));
#else
try
{
    BirdleGame.Run(settings, new BirdleMode(settings.Difficulty));
}
catch (Exception e)
{
    MessageBox.Show(MessageBox.MessageBoxType.Error, "Oops!", "The game crashed. Please report this to the developer:\n" + e);
}
#endif