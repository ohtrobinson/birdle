using birdle.Graphics;

namespace birdle.GUI;

public class UI
{
    public float Scale;

    public Font Font;

    public UI(Font font, float scale = 1.0f)
    {
        Scale = scale;

        Font = font;
    }

    public void Update(float dt)
    {
        
    }
}