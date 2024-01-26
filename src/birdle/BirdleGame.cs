using System;
using System.Drawing;
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

    public BirdleGame(GameSettings settings)
    {
        Settings = settings;

        ColorScheme = settings.DarkMode ? ColorScheme.Dark : ColorScheme.Default;
    }
    
    public void Run()
    {
        Window = new WindowBuilder()
            .Size(1280, 720)
            .Title(GameTitle)
            .GraphicsDeviceOptions(new GraphicsDeviceOptions()
            {
                DepthStencilBufferFormat = null // Disable the depth-stencil buffer, the game is 2D only.
            })
            .Build(out Device);

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
    }
}