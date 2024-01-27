using System.Drawing;
using birdle.Graphics;

namespace birdle.GUI.Elements;

public class TextElement : UIElement
{
    public Font Font;
    
    public string Text;

    public uint FontSize;

    public TextElement(UI ui, Position position, string text, uint fontSize) : base(ui, position)
    {
        Text = text;
        FontSize = fontSize;

        Font = ui.Font;

        Size = ui.Font.MeasureString(fontSize, text);
    }

    public override void Update(float dt, float scale)
    {
        base.Update(dt, scale);

        Size = Font.MeasureString(FontSize, Text);
    }

    public override void Draw(SpriteRenderer renderer, float scale)
    {
        Font.Draw(renderer, (uint) (FontSize * scale), Text, WorldPosition, ColorScheme.TextColor);
    }
}