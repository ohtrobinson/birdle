using System;
using System.Drawing;
using System.Numerics;
using birdle.Graphics;
using Pie.Windowing;

namespace birdle.GUI;

public abstract class UIElement
{
    public Position Position;

    public ColorScheme ColorScheme;

    public Size Size;

    public bool Visible;

    protected internal Vector2 WorldPosition;

    protected bool IsHovered;
    protected bool IsMouseButtonHeld;
    protected bool IsClicked;

    protected UIElement(Position position)
    {
        Position = position;
        ColorScheme = UI.ColorScheme;
        Visible = true;
    }

    public virtual void Update(Input input, float dt, float scale, ref bool mouseCaptured)
    {
        Vector2 mPos = input.MousePosition;
        
        IsClicked = false;

        // TODO: Okay, this DEFINITELY needs a better way of doing this, rather than multiplying by scale all the time.
        Size size = new Size((int) (Size.Width * scale), (int) (Size.Height * scale));
        
        if (!mouseCaptured &&
            mPos.X >= WorldPosition.X && mPos.Y >= WorldPosition.Y &&
            mPos.X < WorldPosition.X + size.Width && mPos.Y < WorldPosition.Y + size.Height)
        {
            mouseCaptured = true;
            IsHovered = true;

            if (input.MouseButtonDown(MouseButton.Left))
                IsMouseButtonHeld = true;
            else if (IsMouseButtonHeld)
            {
                IsClicked = true;
                IsMouseButtonHeld = false;
            }
        }
        else
        {
            IsHovered = false;
            IsMouseButtonHeld = false;
        }
    }

    public abstract void Draw(SpriteRenderer renderer, float scale);
}