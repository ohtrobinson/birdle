using System;
using birdle;
using birdle.Data;
using birdle.GameModes;
using Pie.Windowing;

if (!GameSettings.TryLoad(BirdleGame.ConfigFile, out GameSettings settings))
{
    settings = GameSettings.Default;
    settings.Save(BirdleGame.ConfigFile);
}

try
{
    BirdleGame.Run(settings, new BirdleMode(settings.Difficulty));
}
catch (Exception e)
{
    MessageBox.Show(MessageBox.MessageBoxType.Error, "Oops!", "The game crashed. Please report this to the developer:\n" + e);
}