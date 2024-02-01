using System.Drawing;
using System.Numerics;
using birdle.GUI;
using birdle.GUI.Elements;

namespace birdle.GameModes;

public class MenuMode : GameMode
{
    private FadeElement _fade;
    private GameMode _setGameMode;
    
    public override void Initialize()
    {
        base.Initialize();
        
        Size buttonSize = new Size(100, 30);
        const int fontSize = 20;
        const int spacing = 20;

        const int numElements = 4;
        Position position = new Position(Anchor.MiddleCenter, new Vector2(0, -(numElements * (buttonSize.Height + spacing)) / 2));
        
        TextElement text = new TextElement(position, "Birdle", 50);
        UI.AddElement(text);

        position.Offset.Y += text.FontSize + spacing;
        Button birdleButton = new Button(position, buttonSize, "Play", fontSize, () =>
        {
            _setGameMode = new BirdleMode(BirdleGame.Settings.Difficulty);
            _fade.FadeIn();
        });
        UI.AddElement(birdleButton);

        position.Offset.Y += buttonSize.Height + spacing;
        Button settingsButton = new Button(position, buttonSize, "Settings", fontSize, () =>
        {
            _setGameMode = new SettingsMode(false);
            _fade.FadeIn();
        });
        UI.AddElement(settingsButton);

        position.Offset.Y += buttonSize.Height + spacing;
        Button quitButton = new Button(position, buttonSize, "Quit", fontSize, () => BirdleGame.Close());
        UI.AddElement(quitButton);
        
        TextElement version = new TextElement(new Position(Anchor.BottomLeft, new Vector2(5, -5)), BirdleGame.Version, 20);
        UI.AddElement(version);

        _fade = new FadeElement(null, 0.5f, true);
        _fade.FadeOut();
        UI.AddElement(_fade);
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        if (_fade.State == FadeElement.FadeState.FadedIn)
            BirdleGame.ChangeGameMode(_setGameMode);
    }
}