using System.Drawing;
using System.Numerics;
using birdle.Graphics;

namespace birdle.GUI.Elements;

public class BirdleGrid : UIElement
{
    public int Rows;

    public int Columns;
    
    public int RectangleSize;

    public int Spacing;
    
    public BirdleGrid(UI ui, Position position, int rows, int columns, int rectSize, int spacing) : base(ui, position)
    {
        Rows = rows;
        Columns = columns;
        RectangleSize = rectSize;
        Spacing = spacing;
    }

    public override void Update(float dt, float scale)
    {
        Size = new Size(Columns * (RectangleSize + Spacing), Rows * (RectangleSize + Spacing));
        
        base.Update(dt, scale);
    }

    public override void Draw(SpriteRenderer renderer, float scale)
    {
        Size size = new Size(RectangleSize, RectangleSize);
        Vector2 origin = new Vector2(0.5f);
        
        //renderer.DrawRectangle(WorldPosition, Size, Color.Black, 0, Vector2.Zero);
        
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                Vector2 position = WorldPosition +
                                   new Vector2(c * (RectangleSize + Spacing), r * (RectangleSize + Spacing));
                // We need to account for the fact that the origin is in the middle of the rectangle.
                position += new Vector2(RectangleSize / 2);
                
                renderer.DrawRectangle(position, size, ColorScheme.AlmostColor, 0, origin);
            }
        }
    }
}