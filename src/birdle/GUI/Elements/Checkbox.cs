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

    public override void Update(float dt, float scale, ref bool mouseCaptured)
    {
        base.Update(dt, scale, ref mouseCaptured);

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
            boxColor = ColorScheme.ClickColor;
        else if (IsHovered)
            boxColor = ColorScheme.BackgroundColor;
        
        uint fontSize = (uint) (FontSize * scale);
        Size size = new Size((int) (Size.Width * scale), (int) (Size.Height * scale));
        int borderWidth = int.Max((int) (1 * scale), 1);
        
        renderer.DrawRectangle(WorldPosition, size, boxColor, 0, Vector2.Zero);
        renderer.DrawBorderRectangle(WorldPosition, size, ColorScheme.BorderColor, borderWidth, Vector2.Zero);

        int padding = (int) (5 * scale);
        
        if (Checked)
        {
            renderer.DrawRectangle(WorldPosition + new Vector2(padding), size - new Size(padding * 2, padding * 2),
                ColorScheme.BadColor, 0, Vector2.Zero);
        }

        Size textSize = Font.MeasureString(fontSize, Text);

        Font.Draw(renderer, fontSize, Text, WorldPosition + new Vector2(-textSize.Width - padding, textSize.Height / 2),
            ColorScheme.TextColor);
    }
}