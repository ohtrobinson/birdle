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

    protected internal Vector2 WorldPosition;

    protected bool IsHovered;
    protected bool IsMouseButtonHeld;
    protected bool IsClicked;

    protected UIElement(UI ui, Position position)
    {
        Position = position;
        ColorScheme = ui.ColorScheme;
    }

    public virtual void Update(Input input, float dt, float scale, ref bool mouseCaptured)
    {
        Vector2 mPos = input.MousePosition;
        
        IsClicked = false;
        
        if (!mouseCaptured &&
            mPos.X >= WorldPosition.X && mPos.Y >= WorldPosition.Y &&
            mPos.X < WorldPosition.X + Size.Width && mPos.Y < WorldPosition.Y + Size.Height)
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