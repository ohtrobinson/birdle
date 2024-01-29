using System.Drawing;
using System.Numerics;
using birdle.GUI;
using birdle.GUI.Elements;

namespace birdle.GameModes;

public class MenuMode : GameMode
{
    public override void Initialize()
    {
        base.Initialize();

        Position position = new Position(Anchor.MiddleCenter, new Vector2(0, -60));
        Size buttonSize = new Size(100, 30);
        const int fontSize = 20;
        const int spacing = 10;
        
        TextElement text = new TextElement(position, "Birdle.", 50);
        UI.AddElement(text);

        position.Offset.Y += 60;
        Button birdleButton = new Button(position, buttonSize, "Play", fontSize,
            () => BirdleGame.ChangeGameMode(new BirdleMode(BirdleGame.Settings.Difficulty)));
        UI.AddElement(birdleButton);

        position.Offset.Y += buttonSize.Height + spacing;
        Button quitButton = new Button(position, buttonSize, "Quit", fontSize, () => BirdleGame.Close());
        UI.AddElement(quitButton);
    }
}