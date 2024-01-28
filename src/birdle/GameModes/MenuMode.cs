using birdle.GUI;
using birdle.GUI.Elements;

namespace birdle.GameModes;

public class MenuMode : GameMode
{
    public override void Initialize()
    {
        base.Initialize();

        TextElement text = new TextElement(new Position(Anchor.TopCenter), "Birdle.", 100);
        UI.AddElement(text);
    }
}