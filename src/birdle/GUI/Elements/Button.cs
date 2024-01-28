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
    
    public Button(UI ui, Position position, Size size, string text, uint fontSize, Action click) : base(ui, position)
    {
        Size = size;

        Font = ui.Font;
        Text = text;
        FontSize = fontSize;

        Click = click;
    }

    public override void Update(Input input, float dt, float scale, ref bool mouseCaptured)
    {
        base.Update(input, dt, scale, ref mouseCaptured);
        
        if (IsClicked)
            Click.Invoke();
    }

    public override void Draw(SpriteRenderer renderer, float scale)
    {
        Color buttonColor = ColorScheme.EmptyColor;
        
        if (IsMouseButtonHeld)
            buttonColor = Color.Red;
        else if (IsHovered)
            buttonColor = ColorScheme.BackgroundColor;
        
        renderer.DrawRectangle(WorldPosition, Size, buttonColor, 0, Vector2.Zero);
        renderer.DrawBorderRectangle(WorldPosition, Size, ColorScheme.BorderColor, 1, Vector2.Zero);

        Size textSize = Font.MeasureString(FontSize, Text);

        Font.Draw(renderer, FontSize, Text,
            WorldPosition + new Vector2(Size.Width / 2 - textSize.Width / 2, Size.Height / 2 - textSize.Height / 2),
            ColorScheme.TextColor);
    }
}