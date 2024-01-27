using birdle.GUI;
using birdle.GUI.Elements;

namespace birdle.GameModes;

public class BirdleMode : GameMode
{
    private BirdleGrid _grid;

    public override void Initialize()
    {
        base.Initialize();

        _grid = new BirdleGrid(BirdleGame.UI, new Position(Anchor.TopCenter), 6, 5, 50, 5);
        
        BirdleGame.UI.AddElement(_grid);
    }
}