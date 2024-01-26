using System.Drawing;

namespace birdle;

public struct ColorScheme
{
    public Color BackgroundColor;

    public Color TextColor;

    public static ColorScheme Default => new ColorScheme()
    {
        BackgroundColor = Color.Bisque,
        TextColor = Color.Black
    };

    public static ColorScheme Dark => new ColorScheme()
    {
        BackgroundColor = Color.FromArgb(26, 26, 26),
        TextColor = Color.White
    };
}