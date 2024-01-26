using System;
using System.Drawing;
using System.Numerics;
using birdle.Audio;
using birdle.Graphics;
using Pie;
using Pie.Audio;
using Pie.Windowing;
using Pie.Windowing.Events;

namespace birdle;

public class BirdleGame : IDisposable
{
    public const string GameTitle = "birdle";

    private bool _shouldClose;

    public GameSettings Settings;
    public ColorScheme ColorScheme;
    
    public Window Window;
    public GraphicsDevice GraphicsDevice;
    public SpriteRenderer SpriteRenderer;

    public AudioDevice AudioDevice;

    public Font Font;

    public BirdleGame(GameSettings settings)
    {
        Settings = settings;

        ColorScheme = settings.DarkMode ? ColorScheme.Dark : ColorScheme.Default;

        PieLog.DebugLog += Log;
    }
    
    public void Run()
    {
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
            
            
            
            GraphicsDevice.Present(1);
        }
    }

    public void Close()
    {
        _shouldClose = true;
    }

    public void Dispose()
    {
        Font.Dispose();
        
        AudioDevice.Dispose();
        
        SpriteRenderer.Dispose();
        GraphicsDevice.Dispose();
        Window.Dispose();

        PieLog.DebugLog -= Log;
    }

    public static void Log(LogType type, string message)
    {
        if (type == LogType.Critical)
            throw new Exception($"Critical error: {message}");
        
        Console.WriteLine($"[{type}] {message}");
    }
}