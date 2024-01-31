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

    public uint FontSize;

    public Slot[,] Slots;

    public Font Font;
    
    public BirdleGrid(Position position, int rows, int columns, int rectSize, int spacing, uint fontSize) : base(position)
    {
        Rows = rows;
        Columns = columns;
        RectangleSize = rectSize;
        Spacing = spacing;
        FontSize = fontSize;

        Slots = new Slot[columns, rows];

        Font = UI.Font;
    }

    public override void Update(float dt, float scale, ref bool mouseCaptured)
    {
        base.Update(dt, scale, ref mouseCaptured);
        
        Size = new Size(Columns * (RectangleSize + Spacing), Rows * (RectangleSize + Spacing));
    }

    public override void Draw(SpriteRenderer renderer, float scale)
    {
        int rectSize = (int) (RectangleSize * scale);
        int spacing = (int) (Spacing * scale);
        
        Size size = new Size(rectSize, rectSize);
        Vector2 origin = new Vector2(0.5f);
        
        //renderer.DrawRectangle(WorldPosition, Size, Color.Red, 0, Vector2.Zero);
        
        for (int r = 0; r < Rows; r++)
        {
            for (int c = 0; c < Columns; c++)
            {
                Slot slot = Slots[c, r];

                Vector2 position = WorldPosition + new Vector2(c * (rectSize + spacing), r * (rectSize + spacing));
                // We need to account for the fact that the origin is in the middle of the rectangle.
                
                Color color = slot.State switch
                {
                    SlotState.None => ColorScheme.EmptyColor,
                    SlotState.Bad => ColorScheme.BadColor,
                    SlotState.Almost => ColorScheme.AlmostColor,
                    SlotState.Good => ColorScheme.GoodColor,
                    SlotState.Oops => ColorScheme.OopsColor,
                    _ => throw new ArgumentOutOfRangeException()
                };
                
                renderer.DrawRectangle(position + new Vector2(rectSize / 2), size, color, 0, origin);

                int borderWidth = int.Max(1, (int) (1 * scale));
                renderer.DrawBorderRectangle(position + new Vector2(rectSize / 2), size, ColorScheme.BorderColor, borderWidth, origin);
                
                if (slot.Character == 0)
                    continue;

                uint fontSize = (uint) (FontSize * scale);
                
                Size textSize = Font.MeasureString(fontSize, slot.Character.ToString());

                Font.Draw(renderer, fontSize, slot.Character.ToString(),
                    position + new Vector2(rectSize / 2 - textSize.Width / 2, rectSize / 2 - textSize.Height / 2),
                    ColorScheme.TextColor);
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
        Good,
        Oops
    }
}