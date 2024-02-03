using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Numerics;
using birdle.Generators;
using birdle.GUI;
using birdle.GUI.Elements;
using Pie;
using Pie.Windowing;

namespace birdle.GameModes;

public class BirdleMode : GameMode
{
    private IGenerator _generator;
    private string _word;
    private char[] _grid;

    private int _numRows;
    private int _numColumns;
    
    private BirdleGrid _gridElement;
    private TextElement _temp;

    private Stopwatch _totalTime;
    private TextElement _time;

    private FadeElement _fade;
    private GameMode _setGameMode;

    private Dictionary<char, Button> _keyboard;

    private Difficulty _difficulty;

    private int _currentColumn;
    private int _currentRow;

    private float _timer;
    private bool _missingLetters;
    private bool _invalidWord;

    public BirdleMode(Difficulty difficulty)
    {
        _difficulty = difficulty;
        _keyboard = new Dictionary<char, Button>();
    }

    public override void Initialize()
    {
        base.Initialize();
        
        List<string> words = new List<string>();

        // TODO: Don't load words every time the scene is loaded. This should be done on game startup.
        using StreamReader reader = File.OpenText("Content/Words/words5.wb");
        string word;
        while ((word = reader.ReadLine()) != null)
        {
            if (string.IsNullOrEmpty(word) || word.StartsWith('#'))
                continue;
            words.Add(word);
        }

        _generator = new WordGenerator(words.ToArray());
        _word = _generator.Generate();
        
        _numColumns = _word.Length;

        _numRows = _difficulty switch
        {
            Difficulty.Beginner => 9,
            Difficulty.Easy => 6,
            Difficulty.Normal => 6,
            Difficulty.Hard => 5,
            _ => throw new ArgumentOutOfRangeException()
        };

        _grid = new char[_numColumns * _numRows];

        _gridElement = new BirdleGrid(new Position(Anchor.TopCenter, new Vector2(0, 20)), _numRows, _numColumns, 50, 5, 40);
        UI.AddElement(_gridElement);

        _temp = new TextElement(new Position(Anchor.BottomCenter, new Vector2(0, -100)), "Well done!\nPress space to restart.", 30)
        {
            Visible = false
        };
        UI.AddElement(_temp);

        _time = new TextElement(new Position(Anchor.TopRight, new Vector2(-5, 5)), "00:00", 20);
        UI.AddElement(_time);

        const string keyboardKeys = "qwertyuiop\nasdfghjkl\nzxcvbnm";
        Size keySize = new Size(30, 30);
        const uint keyFontSize = 20;
        const int keySpacing = 10;

        Vector2 position = new Vector2(0, -150);

        foreach (string row in keyboardKeys.Split('\n'))
        {
            int numKeys = row.Length;
            Console.WriteLine(numKeys);

            position.X = -(numKeys * (keySize.Width + keySpacing) - keySpacing) / 2 + (keySize.Width / 2);
            
            foreach (char k in row)
            {
                Button button = new Button(new Position(Anchor.BottomCenter, position), keySize,
                    char.ToString(char.ToUpper(k)), keyFontSize, () => BirdleGameOnTextInput(k));
                UI.AddElement(button);
                
                _keyboard.Add(k, button);

                position.X += keySize.Width + keySpacing;
            }

            position.Y += keySize.Height + keySpacing;
        }

        position.X = -(100 + keySpacing) / 2;
        
        Button enterButton = new Button(new Position(Anchor.BottomCenter, position), new Size(100, 30), "Enter", 20,
            () => BirdleGameOnKeyDown(Key.Enter, false));
        UI.AddElement(enterButton);
        _keyboard.Add('\r', enterButton);

        position.X = (100 + keySpacing) / 2;
        Button backButton = new Button(new Position(Anchor.BottomCenter, position), new Size(100, 30), "Back", 20,
            () => BirdleGameOnKeyDown(Key.Backspace, false));
        UI.AddElement(backButton);
        _keyboard.Add('\b', backButton);
        
        _fade = new FadeElement(null, 0.5f, true);
        UI.AddElement(_fade);
        
        BirdleGame.TextInput += BirdleGameOnTextInput;
        BirdleGame.KeyDown += BirdleGameOnKeyDown;
        
        _totalTime = Stopwatch.StartNew();
        
        _fade.FadeOut();
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
                SetCharacter(_currentColumn, _currentRow, char.MinValue);
                
                break;
            
            case Key.Escape:
                _setGameMode = new MenuMode();
                _fade.FadeIn();
                break;
            
            // TODO: Yknow.. this is temporary.
            case Key.Space when !_totalTime.IsRunning:
                _setGameMode = new BirdleMode(_difficulty);
                _fade.FadeIn();
                break;
        }
    }

    private void BirdleGameOnTextInput(char c)
    {
        if (!char.IsLetter(c))
            return;
        
        if (_currentColumn >= _gridElement.Columns)
            return;
        
        BirdleGame.Log(LogType.Debug, $"SetCharacter({_currentColumn}, {_currentRow}, '{c}')");
        
        SetCharacter(_currentColumn, _currentRow, c);

        _currentColumn++;
    }

    private void SetCharacter(int column, int row, char c)
    {
        _grid[GetIndex(column, row)] = c;
        _gridElement.Slots[column, row].Character = char.ToUpper(c);
    }

    private int GetIndex(int column, int row)
    {
        return row * _numColumns + column;
    }

    private void CheckWord()
    {
        if (_currentColumn < _gridElement.Columns)
        {
            _missingLetters = true;
            return;
        }

        if (_difficulty is Difficulty.Normal or Difficulty.Hard)
        {
            int index = GetIndex(0, _currentRow);
            string word = new string(_grid[index..(index + _numColumns)]);

            if (!_generator.CheckIfValid(word))
            {
                _invalidWord = true;
                return;
            }
        }
        
        int numCorrect = 0;
        
        for (int i = 0; i < _gridElement.Columns; i++)
        {
            BirdleGame.Log(LogType.Debug, $"Process column {i}");
            
            ref BirdleGrid.Slot slot = ref _gridElement.Slots[i, _currentRow];
            char character = _grid[GetIndex(i, _currentRow)];

            if (_word[i] == character)
            {
                slot.State = BirdleGrid.SlotState.Good;
                _keyboard[character].ColorScheme.EmptyColor = UI.ColorScheme.GoodColor;
                numCorrect++;
            }
            else if (_word.Contains(character))
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
                    if (c == character)
                        numOccurrencesInWord++;
                }

                int numOccurrencesInGuess = 0;
                for (int j = 0; j < _gridElement.Columns; j++)
                {
                    int index = GetIndex(j, _currentRow);
                    if (_grid[index] == character && _grid[index] == _word[j])
                        numOccurrencesInGuess++;
                }

                if (numOccurrencesInGuess == numOccurrencesInWord)
                {
                    slot.State = BirdleGrid.SlotState.Bad;
                    _keyboard[character].ColorScheme.EmptyColor = UI.ColorScheme.BadColor;
                }
                else
                {
                    slot.State = BirdleGrid.SlotState.Almost;
                    if (_keyboard[character].ColorScheme.EmptyColor != UI.ColorScheme.GoodColor)
                        _keyboard[character].ColorScheme.EmptyColor = UI.ColorScheme.AlmostColor;
                }

                BirdleGame.Log(LogType.Debug, $"word: {numOccurrencesInWord} guess: {numOccurrencesInGuess}");
            }
            else
            {
                slot.State = BirdleGrid.SlotState.Bad;
                _keyboard[character].ColorScheme.EmptyColor = UI.ColorScheme.BadColor;
            }
        }

        if (numCorrect == _word.Length)
        {
            _totalTime.Stop();
            
            foreach ((_, Button button) in _keyboard)
                button.Visible = false;
            
            _temp.Visible = true;
        }
        else
        {
            _currentRow++;

            if (_currentRow >= _gridElement.Rows)
            {
                _totalTime.Stop();

                foreach ((_, Button button) in _keyboard)
                    button.Visible = false;
                
                _temp.FontSize = 30;
                _temp.Text = $"\nThe word was \"{_word.ToLower()}\".\nPress space to restart.";

                _temp.Visible = true;
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

            for (int i = _currentColumn; i < _gridElement.Columns; i++)
                _gridElement.Slots[i, _currentRow].State = BirdleGrid.SlotState.Oops;
        }
        else if (_invalidWord)
        {
            _timer += dt;

            for (int i = 0; i < _gridElement.Columns; i++)
                _gridElement.Slots[i, _currentRow].State = BirdleGrid.SlotState.Oops;
        }
        
        if (_timer >= 1.0f)
        {
            _missingLetters = false;
            _invalidWord = false;
            _timer = 0;
                
            for (int i = 0; i < _gridElement.Columns; i++)
                _gridElement.Slots[i, _currentRow].State = BirdleGrid.SlotState.None;
        }
        
        if (_fade.State == FadeElement.FadeState.FadedIn)
            BirdleGame.ChangeGameMode(_setGameMode);
    }

    public override void Dispose()
    {
        BirdleGame.TextInput -= BirdleGameOnTextInput;
        BirdleGame.KeyDown -= BirdleGameOnKeyDown;
        
        base.Dispose();
    }
}