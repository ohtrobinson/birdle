using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using birdle.Graphics;

namespace birdle.GUI;

public class UI
{
    private List<UIElement> _elements;
    private ColorScheme _colorScheme;
    
    public float Scale;

    public Font Font;

    public ColorScheme ColorScheme
    {
        get => _colorScheme;
        set
        {
            _colorScheme = value;
            
            foreach (UIElement element in _elements)
                element.ColorScheme = value;
        }
    }

    public UI(Font font, ColorScheme colorScheme, float scale = 1.0f)
    {
        Scale = scale;
        Font = font;
        _colorScheme = colorScheme;

        _elements = new List<UIElement>();
    }

    public void AddElement(UIElement element)
    {
        _elements.Add(element);
    }

    public void ClearElements()
    {
        _elements.Clear();
    }

    public void Update(Size screenSize, float dt)
    {
        bool mouseCaptured = false;
        
        for (int i = _elements.Count - 1; i >= 0; i--)
        {
            UIElement element = _elements[i];
            
            element.WorldPosition = CalculateWorldPos(screenSize, element.Position, element.Size);
            element.Update(dt, Scale, ref mouseCaptured);
        }
    }

    public void Draw(SpriteRenderer renderer)
    {
        foreach (UIElement element in _elements)
            element.Draw(renderer, Scale);
    }

    public Vector2 CalculateWorldPos(Size screenSize, Position position, Size elementSize)
    {
        Vector2 scaledOffset = new Vector2((int) (position.Offset.X * Scale), (int) (position.Offset.Y * Scale));

        Vector2 worldPosition = position.Anchor switch
        {
            Anchor.TopLeft => Vector2.Zero,
            Anchor.TopCenter => new Vector2(screenSize.Width / 2 - elementSize.Width / 2, 0),
            Anchor.TopRight => new Vector2(screenSize.Width - elementSize.Width, 0),
            Anchor.MiddleLeft => new Vector2(0, screenSize.Height / 2 - elementSize.Height / 2),
            Anchor.MiddleCenter => new Vector2(screenSize.Width / 2 - elementSize.Width / 2, screenSize.Height / 2 - elementSize.Height / 2),
            Anchor.MiddleRight => new Vector2(screenSize.Width - elementSize.Width, screenSize.Height / 2 - elementSize.Height / 2),
            Anchor.BottomLeft => new Vector2(0, screenSize.Height - elementSize.Height),
            Anchor.BottomCenter => new Vector2(screenSize.Width / 2 - elementSize.Width / 2, screenSize.Height - elementSize.Height),
            Anchor.BottomRight => new Vector2(screenSize.Width - elementSize.Width, screenSize.Height - elementSize.Height),
            _ => throw new ArgumentOutOfRangeException()
        };

        worldPosition += scaledOffset;

        return worldPosition;
    }
}