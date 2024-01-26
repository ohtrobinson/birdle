using System;
using System.Drawing;
using System.Numerics;
using birdle.Audio;
using birdle.Data;
using birdle.GameModes;
using birdle.Graphics;
using Pie;
using Pie.Audio;
using Pie.Windowing;
using Pie.Windowing.Events;

namespace birdle;

public static class BirdleGame
{
    public const string GameTitle = "birdle";

    private static bool _shouldClose;

    private static GameMode _currentGameMode;
    private static GameMode _newGameMode;

    public static GameSettings Settings;
    public static ColorScheme ColorScheme;
    
    public static Window Window;
    public static GraphicsDevice GraphicsDevice;
    public static SpriteRenderer SpriteRenderer;

    public static AudioDevice AudioDevice;

    public static Font Font;
    
    public static void Run(GameSettings settings, GameMode initialMode)
    {
        Settings = settings;
        _currentGameMode = initialMode;

        ColorScheme = settings.DarkMode ? ColorScheme.Dark : ColorScheme.Default;

        PieLog.DebugLog += Log;
        
        Window = new WindowBuilder()
            .Size(1280, 720)
            .Title(GameTitle)
            .Resizable()
            .GraphicsDeviceOptions(new GraphicsDeviceOptions()
            {
#if DEBUG
                Debug = true,
#endif
                DepthStencilBufferFormat = null // Disable the depth-stencil buffer, the game is 2D only.
            })
            .Build(out GraphicsDevice);

        SpriteRenderer = new SpriteRenderer(GraphicsDevice);

        AudioDevice = new AudioDevice(48000, 32);

        Font = new Font("Content/Fonts/Questrial-Regular.ttf");
        
        _currentGameMode.Initialize();

        while (!_shouldClose)
        {
            while (Window.PollEvent(out IWindowEvent winEvent))
            {
                switch (winEvent)
                {
                    case QuitEvent:
                        _shouldClose = true;
                        break;
                    
                    case ResizeEvent resize:
                        GraphicsDevice.ResizeSwapchain(new Size(resize.Width, resize.Height));
                        GraphicsDevice.Viewport = new Rectangle(0, 0, resize.Width, resize.Height);
                        break;
                }
            }
            
            GraphicsDevice.ClearColorBuffer(ColorScheme.BackgroundColor);

            if (_newGameMode != null)
            {
                _currentGameMode.Dispose();
                _currentGameMode = null;
                GC.Collect();
                _newGameMode.Initialize();
                _currentGameMode = _newGameMode;
            }
            
            _currentGameMode.Update(1.0f);
            _currentGameMode.Draw(1.0f);
            
            GraphicsDevice.Present(1);
        }
        
        Font.Dispose();
        
        AudioDevice.Dispose();
        
        SpriteRenderer.Dispose();
        GraphicsDevice.Dispose();
        Window.Dispose();

        PieLog.DebugLog -= Log;
    }

    public static void ChangeGameMode(GameMode mode)
    {
        _newGameMode = mode;
    }

    public static void Close()
    {
        _shouldClose = true;
    }

    public static void Log(LogType type, string message)
    {
        if (type == LogType.Critical)
            throw new Exception($"Critical error: {message}");
        
        Console.WriteLine($"[{type}] {message}");
    }
}