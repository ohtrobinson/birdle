using System.Numerics;
using birdle.GUI;
using birdle.GUI.Elements;

namespace birdle.GameModes;

public class BirdleMode : GameMode
{
    private BirdleGrid _grid;

    public override void Initialize()
    {
        base.Initialize();

        _grid = new BirdleGrid(BirdleGame.UI, new Position(Anchor.TopCenter, new Vector2(0, 20)), 6, 5, 50, 5);
        
        BirdleGame.UI.AddElement(_grid);
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        _grid.Slots[0, 0] = new BirdleGrid.Slot(BirdleGrid.SlotState.Bad, 'R');
        _grid.Slots[1, 0] = new BirdleGrid.Slot(BirdleGrid.SlotState.Almost, 'E');
        _grid.Slots[2, 0] = new BirdleGrid.Slot(BirdleGrid.SlotState.Bad, 'N');
        _grid.Slots[3, 0] = new BirdleGrid.Slot(BirdleGrid.SlotState.Bad, 'T');
        _grid.Slots[4, 0] = new BirdleGrid.Slot(BirdleGrid.SlotState.Almost, 'S');
        
        _grid.Slots[0, 1] = new BirdleGrid.Slot(BirdleGrid.SlotState.Bad, 'H');
        _grid.Slots[1, 1] = new BirdleGrid.Slot(BirdleGrid.SlotState.Almost, 'E');
        _grid.Slots[2, 1] = new BirdleGrid.Slot(BirdleGrid.SlotState.Bad, 'I');
        _grid.Slots[3, 1] = new BirdleGrid.Slot(BirdleGrid.SlotState.Almost, 'S');
        _grid.Slots[4, 1] = new BirdleGrid.Slot(BirdleGrid.SlotState.Bad, 'T');
        
        _grid.Slots[0, 2] = new BirdleGrid.Slot(BirdleGrid.SlotState.Good, 'S');
        _grid.Slots[1, 2] = new BirdleGrid.Slot(BirdleGrid.SlotState.Bad, 'T');
        _grid.Slots[2, 2] = new BirdleGrid.Slot(BirdleGrid.SlotState.Good, 'A');
        _grid.Slots[3, 2] = new BirdleGrid.Slot(BirdleGrid.SlotState.Bad, 'R');
        _grid.Slots[4, 2] = new BirdleGrid.Slot(BirdleGrid.SlotState.Bad, 'T');
        
        _grid.Slots[0, 3] = new BirdleGrid.Slot(BirdleGrid.SlotState.Good, 'S');
        _grid.Slots[1, 3] = new BirdleGrid.Slot(BirdleGrid.SlotState.Almost, 'E');
        _grid.Slots[2, 3] = new BirdleGrid.Slot(BirdleGrid.SlotState.Good, 'A');
        _grid.Slots[3, 3] = new BirdleGrid.Slot(BirdleGrid.SlotState.Bad, 'R');
        _grid.Slots[4, 3] = new BirdleGrid.Slot(BirdleGrid.SlotState.Good, 'E');
        
        _grid.Slots[0, 4] = new BirdleGrid.Slot(BirdleGrid.SlotState.Good, 'S');
        _grid.Slots[1, 4] = new BirdleGrid.Slot(BirdleGrid.SlotState.Good, 'P');
        _grid.Slots[2, 4] = new BirdleGrid.Slot(BirdleGrid.SlotState.Good, 'A');
        _grid.Slots[3, 4] = new BirdleGrid.Slot(BirdleGrid.SlotState.Good, 'C');
        _grid.Slots[4, 4] = new BirdleGrid.Slot(BirdleGrid.SlotState.Good, 'E');
    }
}