using System.Numerics;

namespace birdle.GUI;

public struct Position
{
    public Anchor Anchor;

    public Vector2 Offset;

    public Position(Anchor anchor, Vector2 offset)
    {
        Anchor = anchor;
        Offset = offset;
    }

    public Position(Anchor anchor)
    {
        Anchor = anchor;
        Offset = Vector2.Zero;
    }

    public Position(Vector2 position)
    {
        Anchor = Anchor.TopLeft;
        Offset = position;
    }
}