using System;
using System.Drawing;
using System.Numerics;
using birdle.Graphics;

namespace birdle.GUI.Elements;

public class Button : UIElement
{
    public Action Click;
    
    public string Text;

    public uint FontSize;

    public Font Font;
    
    public Button( Position position, Size size, string text, uint fontSize, Action click) : base(position)
    {
        Size = size;

        Font = UI.Font;
        Text = text;
        FontSize = fontSize;

        Click = click;
    }

    public override void Update(float dt, float scale, ref bool mouseCaptured)
    {
        base.Update(dt, scale, ref mouseCaptured);
        
        if (IsClicked)
            Click.Invoke();
    }

    public override void Draw(SpriteRenderer renderer, float scale)
    {
        Color buttonColor = ColorScheme.EmptyColor;
        
        if (IsMouseButtonHeld)
            buttonColor = ColorScheme.ClickColor;
        else if (IsHovered)
            buttonColor = ColorScheme.BackgroundColor;
        
        uint fontSize = (uint) (FontSize * scale);
        Size size = new Size((int) (Size.Width * scale), (int) (Size.Height * scale));
        int borderWidth = int.Max((int) (1 * scale), 1);
        
        renderer.DrawRectangle(WorldPosition, size, buttonColor, 0, Vector2.Zero);
        renderer.DrawBorderRectangle(WorldPosition, size, ColorScheme.BorderColor, borderWidth, Vector2.Zero);
        
        Size textSize = Font.MeasureString(fontSize, Text);

        Font.Draw(renderer, fontSize, Text,
            WorldPosition + new Vector2(size.Width / 2 - textSize.Width / 2, size.Height / 2 - textSize.Height / 2),
            ColorScheme.TextColor);
    }
}