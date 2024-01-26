using System;
using System.Drawing;
using System.Numerics;
using Pie.Text;

namespace birdle.Graphics;

public class Font : IDisposable
{
    private static FreeType _freeType;

    private Face _face;

    static Font()
    {
        _freeType = new FreeType();
    }
    
    public Font(string path)
    {
        _face = _freeType.CreateFace(path);
    }

    public void Draw(SpriteRenderer renderer, uint size, string text, Vector2 position, Color color)
    {
        
    }
    
    public void Dispose()
    {
        _face.Dispose();
    }
}