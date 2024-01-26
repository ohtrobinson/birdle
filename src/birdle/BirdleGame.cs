using System;
using System.Drawing;
using System.Numerics;
using birdle.Graphics;
using Pie;
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
    public GraphicsDevice Device;
    public SpriteRenderer SpriteRenderer;

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
            .GraphicsDeviceOptions(new GraphicsDeviceOptions()
            {
#if DEBUG
                Debug = true,
#endif
                DepthStencilBufferFormat = null // Disable the depth-stencil buffer, the game is 2D only.
            })
            .Build(out Device);

        SpriteRenderer = new SpriteRenderer(Device);

        Texture texture = Device.CreateTexture("/home/skye/Pictures/Screenshots/Screenshot_20240124_194328.png");

        while (!_shouldClose)
        {
            while (Window.PollEvent(out IWindowEvent winEvent))
            {
                switch (winEvent)
                {
                    case QuitEvent:
                        _shouldClose = true;
                        break;
                }
            }
            
            Device.ClearColorBuffer(ColorScheme.BackgroundColor);
            
            SpriteRenderer.Draw(texture, new Vector2(100), Color.Red);
            
            Device.Present(1);
        }
    }

    public void Close()
    {
        _shouldClose = true;
    }

    public void Dispose()
    {
        Device.Dispose();
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