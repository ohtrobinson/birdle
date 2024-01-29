using System.Drawing;
using birdle.Graphics;

namespace birdle.GUI.Elements;

public class TextElement : UIElement
{
    public Font Font;
    
    public string Text;

    public uint FontSize;

    public TextElement(Position position, string text, uint fontSize) : base(position)
    {
        Text = text;
        FontSize = fontSize;

        Font = UI.Font;
        Size = Font.MeasureString(fontSize, text);
    }

    public override void Update(Input input, float dt, float scale, ref bool mouseCaptured)
    {
        base.Update(input, dt, scale, ref mouseCaptured);

        Size = Font.MeasureString(FontSize, Text);
    }

    public override void Draw(SpriteRenderer renderer, float scale)
    {
        Font.Draw(renderer, (uint) (FontSize * scale), Text, WorldPosition, ColorScheme.TextColor);
    }
}