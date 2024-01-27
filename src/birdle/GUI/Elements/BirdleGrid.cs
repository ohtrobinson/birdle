using System;
using System.Drawing;
using System.Numerics;
using birdle.Graphics;

namespace birdle.GUI.Elements;

public class BirdleGrid : UIElement
{
    public readonly int Rows;

    public readonly int Columns;
    
    public int RectangleSize;

    public int Spacing;

    public Slot[,] Slots;

    public Font Font;
    
    public BirdleGrid(UI ui, Position position, int rows, int columns, int rectSize, int spacing) : base(ui, position)
    {
        Rows = rows;
        Columns = columns;
        RectangleSize = rectSize;
        Spacing = spacing;

        Slots = new Slot[columns, rows];

        Font = ui.Font;
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
                Slot slot = Slots[c, r];
                
                Vector2 position = WorldPosition +
                                   new Vector2(c * (RectangleSize + Spacing), r * (RectangleSize + Spacing));
                // We need to account for the fact that the origin is in the middle of the rectangle.
                
                Color color = slot.State switch
                {
                    SlotState.None => ColorScheme.EmptyColor,
                    SlotState.Bad => ColorScheme.BadColor,
                    SlotState.Almost => ColorScheme.AlmostColor,
                    SlotState.Good => ColorScheme.GoodColor,
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                renderer.DrawRectangle(position + new Vector2(RectangleSize / 2), size, color, 0, origin);
                
                renderer.DrawBorderRectangle(position + new Vector2(RectangleSize / 2), size, ColorScheme.BorderColor, 1, origin);
                
                if (slot.State == SlotState.None)
                    continue;

                Size textSize = Font.MeasureString(40, slot.Character.ToString());
                
                Font.Draw(renderer, 40, slot.Character.ToString(), position + new Vector2(RectangleSize / 2 - textSize.Width / 2, 0), Color.White);
            }
        }
    }

    public struct Slot
    {
        public SlotState State;
        public char Character;

        public Slot(SlotState state, char character)
        {
            State = state;
            Character = character;
        }
    }

    public enum SlotState
    {
        None,
        Bad,
        Almost,
        Good
    }
}