using System;
using System.Diagnostics;
using System.Drawing;
using System.Numerics;
using birdle.Audio;
using birdle.Data;
using birdle.GameModes;
using birdle.Graphics;
using birdle.GUI;
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
    
    public static Window Window;
    public static GraphicsDevice GraphicsDevice;
    public static SpriteRenderer SpriteRenderer;

    public static UI UI;

    public static AudioDevice AudioDevice;

    public static event OnTextInput TextInput;
    public static event OnKeyDown   KeyDown;

    static BirdleGame()
    {
        TextInput = delegate { };
        KeyDown = delegate { };
    }
    
    public static void Run(GameSettings settings, GameMode initialMode)
    {
        Settings = settings;
        _currentGameMode = initialMode;

        PieLog.DebugLog += Log;
        
        Window = new WindowBuilder()
            .Size(800, 600)
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
        
        Font font = new Font(GraphicsDevice, "Content/Fonts/Questrial-Regular.ttf");
        UI = new UI(font, settings.DarkMode ? ColorScheme.Dark : ColorScheme.Default);

        AudioDevice = new AudioDevice(48000, 32);
        
        Stopwatch deltaWatch = Stopwatch.StartNew();
        
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
                    
                    case TextInputEvent textInput:
                        foreach (char c in textInput.Text)
                            TextInput!.Invoke(c);

                        break;
                    
                    case KeyEvent keyEvent:
                        switch (keyEvent.EventType)
                        {
                            case WindowEventType.KeyDown:
                                KeyDown!.Invoke(keyEvent.Key, false);
                                break;
                            
                            case WindowEventType.KeyRepeat:
                                KeyDown!.Invoke(keyEvent.Key, true);
                                break;
                        }

                        break;
                }
            }
            
            GraphicsDevice.ClearColorBuffer(UI.ColorScheme.BackgroundColor);

            if (_newGameMode != null)
            {
                _currentGameMode.Dispose();
                _currentGameMode = null;
                GC.Collect();
                _newGameMode.Initialize();
                _currentGameMode = _newGameMode;
            }
            
            double deltad = deltaWatch.Elapsed.TotalSeconds;
            deltaWatch.Restart();
            float delta = (float) deltad;
            
            _currentGameMode.Update(delta);
            UI.Update(GraphicsDevice.Viewport.Size, delta);
            
            _currentGameMode.Draw(delta);
            UI.Draw(SpriteRenderer);
            
            GraphicsDevice.Present(1);
        }
        
        UI.Font.Dispose();
        
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

    public delegate void OnTextInput(char c);

    public delegate void OnKeyDown(Key key, bool repeat);
}