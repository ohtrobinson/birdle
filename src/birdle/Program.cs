using birdle;
using birdle.GameModes;

GameSettings settings = new GameSettings()
{
    DarkMode = false
};

BirdleGame.Run(settings, new MenuMode());