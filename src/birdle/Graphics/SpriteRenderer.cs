using System;
using System.Numerics;
using Pie;
using Pie.Utils;

namespace birdle.Graphics;

public class SpriteRenderer : IDisposable
{
    private GraphicsDevice _device;

    private GraphicsBuffer _vertexBuffer;
    private GraphicsBuffer _indexBuffer;
    
    public SpriteRenderer(GraphicsDevice device)
    {
        _device = device;

        VertexPositionTexture[] vertices = new[]
        {
            new VertexPositionTexture(new Vector3(0.0f, 0.0f, 0.0f), new Vector2(0, 0)),
            new VertexPositionTexture(new Vector3(1.0f, 0.0f, 0.0f), new Vector2(1, 0)),
            new VertexPositionTexture(new Vector3(1.0f, 1.0f, 0.0f), new Vector2(1, 1)),
            new VertexPositionTexture(new Vector3(0.0f, 1.0f, 0.0f), new Vector2(0, 1))
        };

        ushort[] indices = new ushort[]
        {
            0, 1, 3,
            1, 2, 3
        };

        _vertexBuffer = device.CreateBuffer(BufferType.VertexBuffer, vertices);
        _indexBuffer = device.CreateBuffer(BufferType.IndexBuffer, indices);
    }
    
    public void Dispose()
    {
        _indexBuffer.Dispose();
        _vertexBuffer.Dispose();
    }
}