using System.Drawing;
using System.IO;
using Pie;
using StbImageSharp;

namespace birdle.Graphics;

public static class GraphicsExtensions
{
    public static Texture CreateTexture(this GraphicsDevice device, string path)
    {
        using Stream stream = File.OpenRead(path);
        ImageResult result = ImageResult.FromStream(stream, ColorComponents.RedGreenBlueAlpha);

        return device.CreateTexture(new Size(result.Width, result.Height), result.Data);
    }
    
    public static Texture CreateTexture(this GraphicsDevice device, Size size, byte[] data)
    {
        TextureDescription description = TextureDescription.Texture2D(size.Width, size.Height, Format.R8G8B8A8_UNorm, 0,
            1, TextureUsage.ShaderResource);

        Texture texture = device.CreateTexture(description, data);
        device.GenerateMipmaps(texture);

        return texture;
    }
}