using System.Numerics;

namespace birdle.GameModes;

public class MenuMode : GameMode
{
    public override void Draw(float dt)
    {
        base.Draw(dt);
        
        BirdleGame.Font.Draw(BirdleGame.SpriteRenderer, 24, "Hello welcome to birdle", new Vector2(10), BirdleGame.ColorScheme.TextColor);
    }
}