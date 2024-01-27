using System;
using System.Collections.Generic;
using System.IO;
using System.Numerics;
using birdle.GUI;
using birdle.GUI.Elements;
using Pie;
using Pie.Windowing;

namespace birdle.GameModes;

public class BirdleMode : GameMode
{
    private BirdleGrid _grid;
    private TextElement _temp;

    private int _currentColumn;
    private int _currentRow;

    private string _word;

    public override void Initialize()
    {
        base.Initialize();

        _grid = new BirdleGrid(BirdleGame.UI, new Position(Anchor.TopCenter, new Vector2(0, 20)), 6, 5, 50, 5, 40);

        _temp = new TextElement(BirdleGame.UI, new Position(Anchor.MiddleCenter), "Nice.", 200);

        List<string> words = new List<string>();

        using StreamReader reader = File.OpenText("Content/wordrepo.txt");
        string word;
        while ((word = reader.ReadLine()) != null)
            words.Add(word);
        
        _word = words[Random.Shared.Next(words.Count)].ToUpper();
        
        BirdleGame.UI.AddElement(_grid);
        
        BirdleGame.TextInput += BirdleGameOnTextInput;
        BirdleGame.KeyDown += BirdleGameOnKeyDown;
    }

    private void BirdleGameOnKeyDown(Key key, bool repeat)
    {
        switch (key)
        {
            case Key.Enter:
                CheckWord();
                break;
            
            case Key.Backspace:
                if (_currentColumn <= 0)
                    break;

                _currentColumn--;
                _grid.Slots[_currentColumn, _currentRow].Character = char.MinValue;
                
                break;
        }
    }

    private void BirdleGameOnTextInput(char c)
    {
        if (_currentColumn >= _grid.Columns)
            return;

        c = char.ToUpper(c);
        
        BirdleGame.Log(LogType.Debug, $"_grid.Slots[{_currentColumn}, {_currentRow}].Character = '{c}'");

        _grid.Slots[_currentColumn, _currentRow].Character = c;

        _currentColumn++;
    }

    private void CheckWord()
    {
        int numCorrect = 0;
        
        for (int i = 0; i < _grid.Columns; i++)
        {
            ref BirdleGrid.Slot slot = ref _grid.Slots[i, _currentRow];
            Console.WriteLine(_currentRow);

            if (_word[i] == slot.Character)
            {
                slot.State = BirdleGrid.SlotState.Good;
                numCorrect++;
            }
            else if (_word.Contains(slot.Character))
                slot.State = BirdleGrid.SlotState.Almost;
            else
                slot.State = BirdleGrid.SlotState.Bad;
        }

        if (numCorrect == _word.Length)
            BirdleGame.UI.AddElement(_temp);
        else
        {
            _currentRow++;

            if (_currentRow >= _grid.Rows)
            {
                _temp.FontSize = 100;
                _temp.Text = $"Oof. The word was \"{_word.ToLower()}\".";
                
                BirdleGame.UI.AddElement(_temp);
            }
            
            _currentColumn = 0;
        }
    }

    public override void Update(float dt)
    {
        base.Update(dt);
    }
}