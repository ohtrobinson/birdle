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

        TextElement text = new TextElement(BirdleGame.UI, new Position(Anchor.TopCenter), "Birdle.", 100);
        BirdleGame.UI.AddElement(text);
    }
}