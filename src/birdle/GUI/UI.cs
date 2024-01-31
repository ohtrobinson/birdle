using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using birdle.Graphics;

namespace birdle.GUI;

public static class UI
{
    private static List<UIElement> _elements;
    private static ColorScheme _colorScheme;
    
    public static float Scale;

    public static Font Font;

    public static ColorScheme ColorScheme
    {
        get => _colorScheme;
        set
        {
            _colorScheme = value;
            
            foreach (UIElement element in _elements)
                element.ColorScheme = value;
        }
    }

    public static void Initialize(Font font, ColorScheme colorScheme, float scale = 1.0f)
    {
        Scale = scale;
        Font = font;
        _colorScheme = colorScheme;

        _elements = new List<UIElement>();
    }

    public static void AddElement(UIElement element)
    {
        _elements.Add(element);
    }

    public static void ClearElements()
    {
        _elements.Clear();
    }

    public static void Update(Size screenSize, float dt)
    {
        bool mouseCaptured = false;
        
        for (int i = _elements.Count - 1; i >= 0; i--)
        {
            UIElement element = _elements[i];
            
            if (!element.Visible)
                continue;
            
            element.WorldPosition = CalculateWorldPos(screenSize, element.Position, element.Size);
            element.Update(dt, Scale, ref mouseCaptured);
        }
    }

    public static void Draw(SpriteRenderer renderer)
    {
        foreach (UIElement element in _elements)
        {
            if (!element.Visible)
                continue;
            
            element.Draw(renderer, Scale);
        }
    }

    public static Vector2 CalculateWorldPos(Size screenSize, Position position, Size elementSize)
    {
        Vector2 scaledOffset = new Vector2((int) (position.Offset.X * Scale), (int) (position.Offset.Y * Scale));
        Size scaledSize = new Size((int) (elementSize.Width * Scale), (int) (elementSize.Height * Scale));

        Vector2 worldPosition = position.Anchor switch
        {
            Anchor.TopLeft => Vector2.Zero,
            Anchor.TopCenter => new Vector2(screenSize.Width / 2 - scaledSize.Width / 2, 0),
            Anchor.TopRight => new Vector2(screenSize.Width - scaledSize.Width, 0),
            Anchor.MiddleLeft => new Vector2(0, screenSize.Height / 2 - scaledSize.Height / 2),
            Anchor.MiddleCenter => new Vector2(screenSize.Width / 2 - scaledSize.Width / 2, screenSize.Height / 2 - scaledSize.Height / 2),
            Anchor.MiddleRight => new Vector2(screenSize.Width - scaledSize.Width, screenSize.Height / 2 - scaledSize.Height / 2),
            Anchor.BottomLeft => new Vector2(0, screenSize.Height - scaledSize.Height),
            Anchor.BottomCenter => new Vector2(screenSize.Width / 2 - scaledSize.Width / 2, screenSize.Height - scaledSize.Height),
            Anchor.BottomRight => new Vector2(screenSize.Width - scaledSize.Width, screenSize.Height - scaledSize.Height),
            _ => throw new ArgumentOutOfRangeException()
        };

        worldPosition += scaledOffset;

        return worldPosition;
    }
}