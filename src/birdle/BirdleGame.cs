using System;
using System.Diagnostics;
using System.Drawing;
using System.IO;
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
using StbImageSharp;

namespace birdle;

public static class BirdleGame
{
    public const string GameTitle = "birdle";
    public const string ConfigFile = "Config.cfg";
    public const string LogDir = "Logs";

    private static StreamWriter _logFile;
    
    private static bool _shouldClose;

    private static GameMode _currentGameMode;
    private static GameMode _newGameMode;

    public static GameSettings Settings;
    
    public static Window Window;
    public static GraphicsDevice GraphicsDevice;
    public static SpriteRenderer SpriteRenderer;

    public static UI UI;
    public static Input Input;

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

        Directory.CreateDirectory(LogDir);
        _logFile = new StreamWriter(Path.Combine(LogDir, DateTime.Now.ToString("yyyy-MM-dd-hh-mm-ss") + ".log"))
        {
            AutoFlush = true
        };

        PieLog.DebugLog += Log;

        ImageResult icon = ImageResult.FromMemory(File.ReadAllBytes("icon.bmp"), ColorComponents.RedGreenBlueAlpha);
        
        Window = new WindowBuilder()
            .Size(settings.WindowSize.Width, settings.WindowSize.Height)
            .Title(GameTitle)
            .FullscreenMode(settings.WindowFullscreen ? FullscreenMode.BorderlessFullscreen : FullscreenMode.Windowed)
            .Resizable()
            .Icon(new Icon((uint) icon.Width, (uint) icon.Height, icon.Data))
            .GraphicsDeviceOptions(new GraphicsDeviceOptions()
            {
#if DEBUG
                Debug = true,
#endif
                DepthStencilBufferFormat = null // Disable the depth-stencil buffer, the game is 2D only.
            })
            .Build(out GraphicsDevice);

        if (Settings.WindowPosition.X != -1 && Settings.WindowPosition.Y != -1)
            Window.Position = Settings.WindowPosition;

        SpriteRenderer = new SpriteRenderer(GraphicsDevice);
        
        Font font = new Font(GraphicsDevice, "Content/Fonts/Questrial-Regular.ttf");
        UI = new UI(font, settings.DarkMode ? ColorScheme.Dark : ColorScheme.Default);
        UI.Scale = Settings.UiScale;

        Input = new Input();

        AudioDevice = new AudioDevice(48000, 32);
        
        Stopwatch deltaWatch = Stopwatch.StartNew();
        
        _currentGameMode.Initialize();

        while (!_shouldClose)
        {
            Input.Update();
            
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
                                Input.RegisterKeyDown(keyEvent.Key);
                                break;
                            
                            case WindowEventType.KeyRepeat:
                                KeyDown!.Invoke(keyEvent.Key, true);
                                break;
                            
                            case WindowEventType.KeyUp:
                                Input.RegisterKeyUp(keyEvent.Key);
                                break;
                        }

                        break;
                    
                    case MouseMoveEvent mouseMove:
                        Input.MousePosition = new Vector2(mouseMove.MouseX, mouseMove.MouseY);
                        break;
                    
                    case MouseButtonEvent buttonEvent:
                        switch (buttonEvent.EventType)
                        {
                            case WindowEventType.MouseButtonDown:
                                Input.RegisterButtonDown(buttonEvent.Button);
                                break;
                            
                            case WindowEventType.MouseButtonUp:
                                Input.RegisterButtonUp(buttonEvent.Button);
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
                UI.ClearElements();
                _newGameMode.Initialize();
                _currentGameMode = _newGameMode;
                _newGameMode = null;
            }
            
            double deltad = deltaWatch.Elapsed.TotalSeconds;
            deltaWatch.Restart();
            float delta = (float) deltad;
            
            _currentGameMode.Update(delta);
            UI.Update(Input, GraphicsDevice.Viewport.Size, delta);
            
            _currentGameMode.Draw(delta);
            UI.Draw(SpriteRenderer);
            
            GraphicsDevice.Present(1);
        }

        Settings.WindowSize = Window.Size;
        Settings.WindowFullscreen = Window.FullscreenMode != FullscreenMode.Windowed;
        Settings.WindowPosition = Window.Position;
        Settings.Save(ConfigFile);
        
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
        _logFile.WriteLine($"[{type}] {message}");
        
        if (type == LogType.Critical)
            throw new Exception($"Critical error: {message}");
        
        Console.WriteLine($"[{type}] {message}");
    }

    public delegate void OnTextInput(char c);

    public delegate void OnKeyDown(Key key, bool repeat);
}