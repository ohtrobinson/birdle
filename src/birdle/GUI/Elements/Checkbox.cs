using System;
using System.Drawing;
using System.Numerics;
using birdle.Graphics;

namespace birdle.GUI.Elements;

public class Checkbox : UIElement
{
    public Action<bool> Check;
    
    public string Text;

    public uint FontSize;

    public Font Font;

    public bool Checked;

    public Checkbox(Position position, Size size, string text, uint fontSize, Action<bool> check, bool isChecked = false) : base(position)
    {
        Size = size;

        Text = text;
        FontSize = fontSize;
        Font = UI.Font;
        Checked = isChecked;

        Check = check;
    }

    public override void Update(Input input, float dt, float scale, ref bool mouseCaptured)
    {
        base.Update(input, dt, scale, ref mouseCaptured);

        if (IsClicked)
        {
            Checked = !Checked;
            Check.Invoke(Checked);
        }
    }

    public override void Draw(SpriteRenderer renderer, float scale)
    {
        Color boxColor = ColorScheme.EmptyColor;
        
        if (IsMouseButtonHeld)
            boxColor = Color.Red;
        else if (IsHovered)
            boxColor = ColorScheme.BackgroundColor;
        
        renderer.DrawRectangle(WorldPosition, Size, boxColor, 0, Vector2.Zero);
        renderer.DrawBorderRectangle(WorldPosition, Size, ColorScheme.BorderColor, 1, Vector2.Zero);

        if (Checked)
        {
            renderer.DrawRectangle(WorldPosition + new Vector2(5), Size - new Size(10, 10), ColorScheme.BadColor, 0,
                Vector2.Zero);
        }

        Size textSize = Font.MeasureString(FontSize, Text);

        Font.Draw(renderer, FontSize, Text, WorldPosition + new Vector2(-textSize.Width - 5, textSize.Height / 2),
            ColorScheme.TextColor);
    }
}