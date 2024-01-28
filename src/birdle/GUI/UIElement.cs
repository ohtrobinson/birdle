﻿using System.Drawing;
using System.Numerics;
using birdle.Graphics;

namespace birdle.GUI;

public abstract class UIElement
{
    public Position Position;

    public ColorScheme ColorScheme;

    public Size Size;

    protected internal Vector2 WorldPosition;

    protected UIElement(UI ui, Position position)
    {
        Position = position;
        ColorScheme = ui.ColorScheme;
    }

    public virtual void Update(float dt, float scale, ref bool mouseCaptured)
    {
        
    }

    public abstract void Draw(SpriteRenderer renderer, float scale);
}