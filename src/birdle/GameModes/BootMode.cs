using System.Drawing;
using System.Numerics;
using birdle.Audio;
using Pie;
using birdle.Graphics;
using birdle.GUI;

namespace birdle.GameModes;

public class BootMode : GameMode
{
    private Texture _texture;
    private Sound _sound;

    private float _timer;
    private float _alpha;

    private bool _hasPlayedSound = false;

    private bool _firstTimeLaunch;

    public BootMode(bool firstTimeLaunch)
    {
        _firstTimeLaunch = firstTimeLaunch;
    }
    
    public override void Initialize()
    {
        base.Initialize();

        _texture = BirdleGame.GraphicsDevice.CreateTexture("Content/aglogo.png");
        _sound = new Sound(BirdleGame.AudioDevice, "Content/Audio/intro.wav");
        
        _alpha = 0;
    }

    public override void Update(float dt)
    {
        base.Update(dt);

        _timer += dt;

        const float startFadeOut = 1.5f;

        if (_timer >= 0.5f)
        {
            _alpha = 1.0f;
            if (!_hasPlayedSound)
            {
                _sound.Play();
                _hasPlayedSound = true;
            }
        }

        if (_timer >= startFadeOut)
            _alpha = 1.0f - ((_timer - startFadeOut) / 0.5f);

        if (_timer >= 2.0f)
        {
            BirdleGame.ChangeGameMode(_firstTimeLaunch ? new SettingsMode(true) : new MenuMode());
        }
    }

    public override void Draw(float dt)
    {
        base.Draw(dt);
        
        Vector2 texSize = new Vector2(_texture.Description.Width, _texture.Description.Height);
        Size viewportSize = BirdleGame.GraphicsDevice.Viewport.Size;
        Vector2 center = new Vector2(viewportSize.Width, viewportSize.Height) / 2;

        int alpha = int.Clamp((int) (_alpha * 255), 0, 255);
        Color color = Color.FromArgb(alpha, UI.ColorScheme.TextColor);
        
        BirdleGame.SpriteRenderer.Begin();
        BirdleGame.SpriteRenderer.Draw(_texture, center, color, 0, new Vector2(0.5f * UI.Scale), new Vector2(0.5f));
        BirdleGame.SpriteRenderer.End();
    }

    public override void Dispose()
    {
        base.Dispose();
        
        _texture.Dispose();
        //_sound.Dispose();
    }
}