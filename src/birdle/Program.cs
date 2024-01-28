using birdle;
using birdle.Data;
using birdle.GameModes;

if (!GameSettings.TryLoad(BirdleGame.ConfigFile, out GameSettings settings))
{
    settings = GameSettings.Default;
    settings.Save(BirdleGame.ConfigFile);
}

BirdleGame.Run(settings, new BirdleMode(settings.Difficulty));