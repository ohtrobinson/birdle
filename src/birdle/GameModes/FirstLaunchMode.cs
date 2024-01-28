using System.Drawing;
using System.Numerics;
using birdle.GUI;
using birdle.GUI.Elements;
using Pie.Windowing;

namespace birdle.GameModes;

public class FirstLaunchMode : GameMode
{
    private TextElement _welcomeText;
    private Checkbox _darkModeCheckbox;
    private Button _difficultyButtton;
    private Checkbox _fullscreenCheckbox;
    private Button _doneButton;

    private FadeElement _fade;

    private bool _done;
    
    public override void Initialize()
    {
        base.Initialize();

        int topPosition = -200;
        const int spacing = 20;

        int checkBoxOffset = 50;

        _welcomeText = new TextElement(new Position(Anchor.MiddleCenter, new Vector2(0, topPosition)), "Welcome.", 50);
        UI.AddElement(_welcomeText);

        topPosition += spacing * 4;

        _darkModeCheckbox = new Checkbox(new Position(Anchor.MiddleCenter, new Vector2(checkBoxOffset, topPosition)),
            new Size(30, 30), "Dark Mode", 20, b =>
            {
                BirdleGame.Settings.DarkMode = b;
                UI.ColorScheme = b ? ColorScheme.Dark : ColorScheme.Default;
            }, BirdleGame.Settings.DarkMode);
        UI.AddElement(_darkModeCheckbox);

        topPosition += _darkModeCheckbox.Size.Height + spacing;

        ref Difficulty difficulty = ref BirdleGame.Settings.Difficulty;

        _difficultyButtton = new Button(new Position(Anchor.MiddleCenter, new Vector2(0, topPosition)),
            new Size(100, 30), difficulty.ToString(), 20,
            () =>
            {
                ref Difficulty difficulty = ref BirdleGame.Settings.Difficulty;

                difficulty++;

                if (difficulty > Difficulty.Hard)
                    difficulty = Difficulty.Beginner;

                _difficultyButtton.Text = difficulty.ToString();
            });
        UI.AddElement(_difficultyButtton);

        topPosition += _difficultyButtton.Size.Height + spacing;

        _fullscreenCheckbox = new Checkbox(new Position(Anchor.MiddleCenter, new Vector2(checkBoxOffset, topPosition)),
            new Size(30, 30), "Full screen", 20,
            b =>
            {
                BirdleGame.Window.FullscreenMode = b ? FullscreenMode.BorderlessFullscreen : FullscreenMode.Windowed;
            }, BirdleGame.Window.FullscreenMode != FullscreenMode.Windowed);
        UI.AddElement(_fullscreenCheckbox);

        topPosition += _fullscreenCheckbox.Size.Height + spacing;

        _doneButton = new Button(new Position(Anchor.MiddleCenter, new Vector2(0, topPosition)), new Size(100, 30),
            "Done", 20,
            () =>
            {
                _done = true;
            });
        UI.AddElement(_doneButton);

        _fade = new FadeElement(Position.TopLeft, BirdleGame.Window.Size, null, 0.5f, true);
        UI.AddElement(_fade);
        
        _fade.FadeOut();
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        if (_done)
        {
            if (_fade.State == FadeElement.FadeState.FadedIn)
                BirdleGame.ChangeGameMode(new BirdleMode(BirdleGame.Settings.Difficulty));
            
            _fade.FadeIn();
        }
    }
}