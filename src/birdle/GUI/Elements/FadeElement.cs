using System.Drawing;
using System.Numerics;
using birdle.Graphics;

namespace birdle.GUI.Elements;

public class FadeElement : UIElement
{
    private float _currentTime;
    
    public FadeState State;

    public float FadeTime;

    public Color? Color;
    
    public FadeElement(Color? color, float fadeTime, bool startFadedIn = false) : base(Position.TopLeft)
    {
        Color = color;
        State = FadeState.FadedOut;
        FadeTime = fadeTime;

        if (startFadedIn)
        {
            _currentTime = FadeTime;
            State = FadeState.FadedIn;
        }
    }

    public void FadeIn()
    {
        State = FadeState.FadingIn;
    }

    public void FadeOut()
    {
        State = FadeState.FadingOut;
    }

    public override void Update(Input input, float dt, float scale, ref bool mouseCaptured)
    {
        switch (State)
        {
            case FadeState.None:
            case FadeState.FadedIn:
            case FadeState.FadedOut:
                break;
            
            case FadeState.FadingIn:
                if (_currentTime < FadeTime)
                {
                    _currentTime += dt;
                    break;
                }
                
                State = FadeState.FadedIn;
                _currentTime = FadeTime;
                break;
            
            case FadeState.FadingOut:
                if (_currentTime > 0)
                {
                    _currentTime -= dt;
                    break;
                }

                State = FadeState.FadedOut;
                _currentTime = 0;
                
                break;
        }
    }

    public override void Draw(SpriteRenderer renderer, float scale)
    {
        if (_currentTime <= 0)
            return;

        int alpha = int.Clamp((int) ((_currentTime / FadeTime) * 255), 0, 255);
        Color color = System.Drawing.Color.FromArgb(alpha, Color ?? ColorScheme.BackgroundColor);
        renderer.DrawRectangle(Vector2.Zero, renderer.Device.Viewport.Size, color, 0, Vector2.Zero);
    }

    public enum FadeState
    {
        None,
        FadingIn,
        FadedIn,
        FadingOut,
        FadedOut
    }
}