using System.Drawing;
using System.Numerics;
using birdle.Graphics;

namespace birdle.GUI;

public abstract class UIElement
{
    public Position Position;

    public ColorScheme ColorScheme;

    public virtual Size Size { get; set; }

    protected internal Vector2 WorldPosition;

    protected UIElement(UI ui, Position position)
    {
        Position = position;
        ColorScheme = ui.ColorScheme;
    }

    public virtual void Update(float dt, float scale)
    {
        
    }

    public abstract void Draw(SpriteRenderer renderer, float scale);
}