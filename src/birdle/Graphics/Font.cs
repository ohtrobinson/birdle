using System;
using System.Collections.Generic;
using System.Drawing;
using System.Numerics;
using Pie;
using Pie.Text;

namespace birdle.Graphics;

public class Font : IDisposable
{
    private static FreeType _freeType;

    private Face _face;

    private Dictionary<uint, Dictionary<char, Character>> _cachedCharacters;

    static Font()
    {
        _freeType = new FreeType();
    }
    
    public Font(string path)
    {
        _face = _freeType.CreateFace(path);
        _cachedCharacters = new Dictionary<uint, Dictionary<char, Character>>();
    }

    public void Draw(SpriteRenderer renderer, uint size, string text, Vector2 position, Color color)
    {
        Vector2 currentPosition = position + new Vector2(0, size);

        if (!_cachedCharacters.TryGetValue(size, out Dictionary<char, Character> characters))
        {
            characters = new Dictionary<char, Character>();
            _cachedCharacters.Add(size, characters);
        }

        foreach (char c in text)
        {
            if (!characters.TryGetValue(c, out Character character))
            {
                BirdleGame.Log(LogType.Verbose, $"Creating character '{c}' at size {size}.");
                
                Pie.Text.Character ftChar = _face.GetCharacter(c, size);

                Size charSize = new Size(ftChar.Width, ftChar.Height);
                Vector2 bearing = new Vector2(ftChar.BitmapLeft, ftChar.BitmapTop);

                Texture texture = null;
                
                if (charSize != Size.Empty) 
                    texture = renderer.Device.CreateTexture(charSize, ftChar.Bitmap);

                character = new Character()
                {
                    Texture = texture,
                    Size = charSize,
                    Bearing = bearing,
                    Advance = ftChar.Advance
                };
                
                characters.Add(c, character);
            }
            
            switch (c)
            {
                case ' ':
                    currentPosition.X += character.Advance;
                    continue;
                
                case '\n':
                    currentPosition.Y += size;
                    currentPosition.X = position.X;
                    continue;
            }
            
            renderer.Draw(character.Texture, currentPosition + new Vector2(character.Bearing.X, -character.Bearing.Y), color);
            currentPosition.X += character.Advance;
        }
    }
    
    public void Dispose()
    {
        _face.Dispose();
    }

    private struct Character
    {
        public Texture Texture;
        public Size Size;
        public Vector2 Bearing;
        public int Advance;
    }
}