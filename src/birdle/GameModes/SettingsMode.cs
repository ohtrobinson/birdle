using System.Drawing;
using System.Numerics;
using birdle.GUI;
using birdle.GUI.Elements;
using Pie.Windowing;

namespace birdle.GameModes;

public class SettingsMode : GameMode
{
    private bool _isFirstLaunch;
    
    private TextElement _welcomeText;
    private Checkbox _darkModeCheckbox;
    private Button _difficultyButton;
    private Checkbox _fullscreenCheckbox;
    private Button _doneButton;

    private FadeElement _fade;

    private bool _done;

    public SettingsMode(bool isFirstLaunch)
    {
        _isFirstLaunch = isFirstLaunch;
    }
    
    public override void Initialize()
    {
        base.Initialize();
        
        const int spacing = 20;
        Size buttonSize = new Size(100, 30);
        Size checkBoxSize = new Size(30, 30);
        const int checkBoxOffset = 50;

        const int numElements = 6;
        Position position = new Position(Anchor.MiddleCenter, new Vector2(0, -(numElements * (buttonSize.Height + spacing)) / 2));

        _welcomeText = new TextElement(position, _isFirstLaunch ? "Welcome!" : "Settings", 50);
        UI.AddElement(_welcomeText);

        position.Offset.Y += _welcomeText.FontSize + spacing;
        position.Offset.X = 0;
        
        ref Difficulty difficulty = ref BirdleGame.Settings.Difficulty;

        _difficultyButton = new Button(position, buttonSize, difficulty.ToString(), 20, () =>
        {
            ref Difficulty difficulty = ref BirdleGame.Settings.Difficulty;

            difficulty++;

            if (difficulty > Difficulty.Hard)
                difficulty = Difficulty.Beginner;

            _difficultyButton.Text = difficulty.ToString();
        });
        UI.AddElement(_difficultyButton);
        
        position.Offset.Y += buttonSize.Height + spacing;
        position.Offset.X = checkBoxOffset;
        
        _darkModeCheckbox = new Checkbox(position, checkBoxSize, "Dark Mode", 20, b =>
        {
            BirdleGame.Settings.DarkMode = b;
            UI.ColorScheme = b ? ColorScheme.Dark : ColorScheme.Default;
        }, BirdleGame.Settings.DarkMode);
        UI.AddElement(_darkModeCheckbox);

        position.Offset.Y += checkBoxSize.Height + spacing;
        position.Offset.X = checkBoxOffset;

        _fullscreenCheckbox = new Checkbox(position, checkBoxSize, "Full screen", 20, b =>
        {
            BirdleGame.Window.FullscreenMode = b ? FullscreenMode.BorderlessFullscreen : FullscreenMode.Windowed;
        }, BirdleGame.Window.FullscreenMode != FullscreenMode.Windowed);
        UI.AddElement(_fullscreenCheckbox);

        position.Offset.Y += checkBoxSize.Height + spacing;
        position.Offset.X = 0;
        
        TextElement scaleText = new TextElement(position, UI.Scale.ToString("0.00"), 20);

        position.Offset.X = scaleText.Size.Width;
        Button iScaleButton = new Button(position, checkBoxSize, "+", 20, () =>
        {
            UI.Scale += 0.05f;
            scaleText.Text = UI.Scale.ToString("0.00");
        });

        position.Offset.X = -scaleText.Size.Width;
        Button dScaleButton = new Button(position, checkBoxSize, "-", 20, () =>
        {
            UI.Scale -= 0.05f;
            scaleText.Text = UI.Scale.ToString("0.00");
        });
        
        UI.AddElement(scaleText);
        UI.AddElement(iScaleButton);
        UI.AddElement(dScaleButton);

        position.Offset.Y += buttonSize.Height + spacing;
        position.Offset.X = 0;

        _doneButton = new Button(position, buttonSize, "Done", 20, () =>
        {
            BirdleGame.Settings.UiScale = UI.Scale;
            BirdleGame.Settings.Save(BirdleGame.ConfigFile);
            _done = true;
        });
        UI.AddElement(_doneButton);

        _fade = new FadeElement(null, 0.5f, true);
        UI.AddElement(_fade);
        
        _fade.FadeOut();
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        if (_done)
        {
            if (_fade.State == FadeElement.FadeState.FadedIn)
                BirdleGame.ChangeGameMode(new MenuMode());
            
            _fade.FadeIn();
        }
    }
}