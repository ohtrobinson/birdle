using System;
using System.Collections.Generic;
using System.Diagnostics;
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

    private Stopwatch _totalTime;
    private TextElement _time;

    private Difficulty _difficulty;

    private int _currentColumn;
    private int _currentRow;

    private string _word;

    private float _timer;
    private bool _missingLetters;
    private bool _invalidWord;

    private List<string> _knownWords;

    public BirdleMode(Difficulty difficulty)
    {
        _difficulty = difficulty;
    }

    public override void Initialize()
    {
        base.Initialize();

        int numRows = _difficulty switch
        {
            Difficulty.Beginner => 9,
            Difficulty.Easy => 6,
            Difficulty.Normal => 6,
            Difficulty.Hard => 5,
            _ => throw new ArgumentOutOfRangeException()
        };

        _grid = new BirdleGrid(BirdleGame.UI, new Position(Anchor.TopCenter, new Vector2(0, 20)), numRows, 5, 50, 5,
            40);

        _temp = new TextElement(BirdleGame.UI, new Position(Anchor.BottomCenter, new Vector2(0, -20)), "Nice.", 200);

        _time = new TextElement(BirdleGame.UI, new Position(Anchor.TopRight, new Vector2(-5, 5)), "00:00", 20);

        _knownWords = new List<string>();

        using StreamReader reader = File.OpenText("Content/wordrepo.txt");
        string word;
        while ((word = reader.ReadLine()) != null)
        {
            if (string.IsNullOrEmpty(word) || word.StartsWith('#'))
                continue;
            _knownWords.Add(word);
        }

        _word = _knownWords[Random.Shared.Next(_knownWords.Count)].ToUpper();
        
        BirdleGame.UI.AddElement(_grid);
        BirdleGame.UI.AddElement(_time);
        
        BirdleGame.TextInput += BirdleGameOnTextInput;
        BirdleGame.KeyDown += BirdleGameOnKeyDown;
        
        _totalTime = Stopwatch.StartNew();
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
            
            // TODO: Yknow.. this is temporary.
            case Key.Space when !_totalTime.IsRunning:
                BirdleGame.ChangeGameMode(new BirdleMode(_difficulty));
                break;
        }
    }

    private void BirdleGameOnTextInput(char c)
    {
        if (!char.IsLetter(c))
            return;
        
        if (_currentColumn >= _grid.Columns)
            return;

        c = char.ToUpper(c);
        
        BirdleGame.Log(LogType.Debug, $"_grid.Slots[{_currentColumn}, {_currentRow}].Character = '{c}'");

        _grid.Slots[_currentColumn, _currentRow].Character = c;

        _currentColumn++;
    }

    private void CheckWord()
    {
        if (_currentColumn < _grid.Columns)
        {
            _missingLetters = true;
            return;
        }

        if (_difficulty is Difficulty.Normal or Difficulty.Hard)
        {
            string word = "";
            // YUCK!!!!
            // TODO: Replace this utter crap. You need a better system of storing the currently active word.
            for (int i = 0; i < _grid.Columns; i++)
                word += _grid.Slots[i, _currentRow].Character;

            if (!_knownWords.Contains(word.ToLower()))
            {
                _invalidWord = true;
                return;
            }
        }
        
        int numCorrect = 0;
        
        for (int i = 0; i < _grid.Columns; i++)
        {
            BirdleGame.Log(LogType.Debug, $"Process column {i}");
            
            ref BirdleGrid.Slot slot = ref _grid.Slots[i, _currentRow];

            if (_word[i] == slot.Character)
            {
                slot.State = BirdleGrid.SlotState.Good;
                numCorrect++;
            }
            else if (_word.Contains(slot.Character))
            {
                // This algorithm works out if a character should be displayed as "almost", or "bad".
                // Say a word has 2 occurrences of the letter 'a', and the user's guess has 3 occurrences of 'a', one
                // of which is in the correct slot. Because of this, we still display as having an 'a' available, and
                // even if the entire guess was 'a's, all occurrences would show up as "almost" (to not give it away).
                // However, if the user gets both 'a's in the right slots, the game will no longer show any available
                // 'a's, and will instead mark all other 'a's as "bad", to show that there are no more a's available.
                // Does this make sense? This feels like a horrible explanation...
                
                // This algorithm takes the very naive approach of counting occurrences in the word, then counting the
                // number of CORRECT occurrences in the guess (as in - the guess slot matches the word). If the count
                // matches, then we know there are no more of that letter available, so show all other occurrences as
                // "bad", otherwise show them as "almost".
                
                int numOccurrencesInWord = 0;
                foreach (char c in _word)
                {
                    if (c == slot.Character)
                        numOccurrencesInWord++;
                }

                int numOccurrencesInGuess = 0;
                for (int j = 0; j < _grid.Columns; j++)
                {
                    if (_grid.Slots[j, _currentRow].Character == slot.Character &&
                        _grid.Slots[j, _currentRow].Character == _word[j])
                        numOccurrencesInGuess++;
                }

                if (numOccurrencesInGuess == numOccurrencesInWord)
                    slot.State = BirdleGrid.SlotState.Bad;
                else
                    slot.State = BirdleGrid.SlotState.Almost;
                
                BirdleGame.Log(LogType.Debug, $"word: {numOccurrencesInWord} guess: {numOccurrencesInGuess}");
            }
            else
                slot.State = BirdleGrid.SlotState.Bad;
        }

        if (numCorrect == _word.Length)
        {
            _totalTime.Stop();
            BirdleGame.UI.AddElement(_temp);
        }
        else
        {
            _currentRow++;

            if (_currentRow >= _grid.Rows)
            {
                _totalTime.Stop();
                _temp.FontSize = 50;
                _temp.Text = $"Oof. The word was \"{_word.ToLower()}\".";
                
                BirdleGame.UI.AddElement(_temp);
            }
            
            _currentColumn = 0;
        }
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        TimeSpan elapsed = _totalTime.Elapsed;
        int minutes = elapsed.Minutes;
        int seconds = elapsed.Seconds;

        _time.Text = $"{minutes:00}:{seconds:00}";

        if (_missingLetters)
        {
            _timer += dt;

            for (int i = _currentColumn; i < _grid.Columns; i++)
                _grid.Slots[i, _currentRow].State = BirdleGrid.SlotState.Oops;
        }
        else if (_invalidWord)
        {
            _timer += dt;

            for (int i = 0; i < _grid.Columns; i++)
                _grid.Slots[i, _currentRow].State = BirdleGrid.SlotState.Oops;
        }
        
        if (_timer >= 1.0f)
        {
            _missingLetters = false;
            _invalidWord = false;
            _timer = 0;
                
            for (int i = 0; i < _grid.Columns; i++)
                _grid.Slots[i, _currentRow].State = BirdleGrid.SlotState.None;
        }
    }

    public override void Dispose()
    {
        BirdleGame.TextInput -= BirdleGameOnTextInput;
        BirdleGame.KeyDown -= BirdleGameOnKeyDown;
        
        base.Dispose();
    }
}