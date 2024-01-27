using System.Drawing;

namespace birdle.GUI;

public struct ColorScheme
{
    public Color BackgroundColor;

    public Color TextColor;

    public Color GoodColor;

    public Color AlmostColor;

    public Color BadColor;

    public Color EmptyColor;

    public Color OopsColor;

    public Color BorderColor;

    public static ColorScheme Default => new ColorScheme()
    {
        BackgroundColor = Color.Bisque,
        TextColor = Color.Black,
        
        GoodColor = Color.DarkGreen,
        AlmostColor = Color.Goldenrod,
        BadColor = Color.DimGray,
        
        // Bisque but darker
        EmptyColor = Color.FromArgb(230, 205, 176),
        
        OopsColor = Color.Red,
        
        BorderColor = Color.Black
    };

    public static ColorScheme Dark => new ColorScheme()
    {
        BackgroundColor = Color.FromArgb(26, 26, 26),
        TextColor = Color.White,
        
        GoodColor = Color.DarkGreen,
        AlmostColor = Color.Goldenrod,
        BadColor = Color.DimGray,
        
        EmptyColor = Color.FromArgb(29, 29, 29),
        
        OopsColor = Color.DarkRed,
        
        BorderColor = Color.White
    };
}