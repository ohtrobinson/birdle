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
            .Build(out Device);

        SpriteRenderer = new SpriteRenderer(Device);

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
                        Device.ResizeSwapchain(new Size(resize.Width, resize.Height));
                        Device.Viewport = new Rectangle(0, 0, resize.Width, resize.Height);
                        break;
                }
            }
            
            Device.ClearColorBuffer(ColorScheme.BackgroundColor);
            
            Font.Draw(SpriteRenderer, 50, "Birdle", new Vector2(Device.Viewport.Width / 2 - 100, 100), ColorScheme.TextColor);
            
            Device.Present(1);
        }
    }

    public void Close()
    {
        _shouldClose = true;
    }

    public void Dispose()
    {
        SpriteRenderer.Dispose();
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