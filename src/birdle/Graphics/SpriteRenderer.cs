using System;
using System.Drawing;
using System.IO;
using System.Numerics;
using System.Runtime.CompilerServices;
using Pie;
using Pie.ShaderCompiler;
using Pie.Utils;

namespace birdle.Graphics;

public class SpriteRenderer : IDisposable
{
    public const uint MaxSprites = 1024;

    private const uint NumVertices = 4;
    private const uint NumIndices = 6;

    private const uint MaxVertices = NumVertices * MaxSprites;
    private const uint MaxIndices = NumIndices * MaxSprites;
    
    private GraphicsDevice _device;

    private VertexPositionColorTexture[] _vertices;
    private ushort[] _indices;

    private GraphicsBuffer _vertexBuffer;
    private GraphicsBuffer _indexBuffer;

    private GraphicsBuffer _cameraBuffer;

    private Shader _shader;
    private InputLayout _inputLayout;

    private RasterizerState _rasterizerState;
    private BlendState _blendState;
    private SamplerState _samplerState;

    private bool _isBegun;
    private Texture _currentTexture;
    private uint _currentSprite;

    public readonly Texture White;

    public GraphicsDevice Device => _device;
    
    public SpriteRenderer(GraphicsDevice device)
    {
        _device = device;

        _vertices = new VertexPositionColorTexture[MaxVertices];
        _indices = new ushort[MaxIndices];

        _vertexBuffer = device.CreateBuffer(BufferType.VertexBuffer, MaxVertices * VertexPositionColorTexture.SizeInBytes, true);
        _indexBuffer = device.CreateBuffer(BufferType.IndexBuffer, MaxIndices * sizeof(uint), true);

        _cameraBuffer = device.CreateBuffer(BufferType.UniformBuffer, (uint) Unsafe.SizeOf<Matrix4x4>(), true);

        string shaderText = File.ReadAllText("Content/Shaders/Sprite.hlsl");
        _shader = device.CreateShader(new[]
        {
            new ShaderAttachment(ShaderStage.Vertex, shaderText, Language.HLSL, "Vertex"),
            new ShaderAttachment(ShaderStage.Pixel, shaderText, Language.HLSL, "Pixel"),
        });

        _inputLayout = device.CreateInputLayout(new[]
        {
            new InputLayoutDescription(Format.R32G32B32_Float, 0, 0, InputType.PerVertex), // position
            new InputLayoutDescription(Format.R32G32B32A32_Float, 12, 0, InputType.PerVertex), // tint
            new InputLayoutDescription(Format.R32G32_Float, 28, 0, InputType.PerVertex) // texCoord
        });

        _rasterizerState = device.CreateRasterizerState(RasterizerStateDescription.CullNone);
        _blendState = device.CreateBlendState(BlendStateDescription.NonPremultiplied);
        _samplerState = device.CreateSamplerState(SamplerStateDescription.LinearClamp);

        White = device.CreateTexture(new Size(1, 1), new byte[] { 255, 255, 255, 255 });
    }

    public void Begin()
    {
        if (_isBegun)
            throw new Exception("Cannot begin, has already begun.");

        _isBegun = true;
        
        Rectangle viewport = _device.Viewport;
        Matrix4x4 projection = Matrix4x4.CreateOrthographicOffCenter(viewport.X, viewport.X + viewport.Width,
            viewport.Y + viewport.Height, viewport.Y, -1, 1);
        
        _device.UpdateBuffer(_cameraBuffer, 0, projection);
    }

    public void End()
    {
        if (!_isBegun)
            throw new Exception("Cannot end, has already ended.");

        _isBegun = false;
        
        Flush();
    }

    public void Draw(Texture texture, Vector2 position, Color tint, float rotation, Vector2 scale, Vector2 origin)
    {
        if (!_isBegun)
            throw new Exception("Cannot draw, has not begun.");
        
        if (texture != _currentTexture || _currentSprite >= MaxSprites)
            Flush();

        _currentTexture = texture;
        
        TextureDescription description = texture.Description;

        Matrix4x4 world = Matrix4x4.CreateTranslation(-origin.X, -origin.Y, 0) *
                          Matrix4x4.CreateScale(description.Width * scale.X, description.Height * scale.Y, 1) *
                          Matrix4x4.CreateRotationZ(rotation) *
                          Matrix4x4.CreateTranslation(position.X, position.Y, 0);

        uint vOffset = _currentSprite * NumVertices;
        uint iOffset = _currentSprite * NumIndices;

        Vector4 nTint = tint.Normalize();
        
        _vertices[vOffset + 0] = new VertexPositionColorTexture(Vector3.Transform(new Vector3(0, 0, 0), world), nTint, new Vector2(0, 0));
        _vertices[vOffset + 1] = new VertexPositionColorTexture(Vector3.Transform(new Vector3(1, 0, 0), world), nTint, new Vector2(1, 0));
        _vertices[vOffset + 2] = new VertexPositionColorTexture(Vector3.Transform(new Vector3(1, 1, 0), world), nTint, new Vector2(1, 1));
        _vertices[vOffset + 3] = new VertexPositionColorTexture(Vector3.Transform(new Vector3(0, 1, 0), world), nTint, new Vector2(0, 1));

        _indices[iOffset + 0] = (ushort) (0 + vOffset);
        _indices[iOffset + 1] = (ushort) (1 + vOffset);
        _indices[iOffset + 2] = (ushort) (3 + vOffset);
        _indices[iOffset + 3] = (ushort) (1 + vOffset);
        _indices[iOffset + 4] = (ushort) (2 + vOffset);
        _indices[iOffset + 5] = (ushort) (3 + vOffset);

        _currentSprite++;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DrawRectangle(Vector2 position, Size size, Color color, float rotation, Vector2 normalizedOrigin)
    {
        Draw(White, position, color, rotation, new Vector2(size.Width, size.Height), normalizedOrigin);
    }

    public void DrawBorderRectangle(Vector2 position, Size size, Color color, int borderWidth, Vector2 origin)
    {
        position -= origin * new Vector2(size.Width, size.Height);
        
        DrawRectangle(position, new Size(size.Width, borderWidth), color, 0, Vector2.Zero);
        DrawRectangle(position, new Size(borderWidth, size.Height), color, 0, Vector2.Zero);
        DrawRectangle(position + new Vector2(size.Width - borderWidth, 0), new Size(borderWidth, size.Height), color, 0, Vector2.Zero);
        DrawRectangle(position + new Vector2(0, size.Height - borderWidth), new Size(size.Width, borderWidth), color, 0, Vector2.Zero);
    }

    private void Flush()
    {
        if (_currentSprite == 0)
            return;

        MappedSubresource vRes = _device.MapResource(_vertexBuffer, MapMode.Write);
        PieUtils.CopyToUnmanaged(vRes.DataPtr, 0, _currentSprite * NumVertices * VertexPositionColorTexture.SizeInBytes, _vertices);
        _device.UnmapResource(_vertexBuffer);

        MappedSubresource iRes = _device.MapResource(_indexBuffer, MapMode.Write);
        PieUtils.CopyToUnmanaged(iRes.DataPtr, 0, _currentSprite * NumIndices * sizeof(ushort), _indices);
        _device.UnmapResource(_indexBuffer);
        
        _device.SetPrimitiveType(PrimitiveType.TriangleList);
        _device.SetRasterizerState(_rasterizerState);
        _device.SetBlendState(_blendState);
        
        _device.SetUniformBuffer(0, _cameraBuffer);
        _device.SetTexture(1, _currentTexture, _samplerState);
        
        _device.SetShader(_shader);
        _device.SetInputLayout(_inputLayout);
        _device.SetVertexBuffer(0, _vertexBuffer, VertexPositionColorTexture.SizeInBytes);
        _device.SetIndexBuffer(_indexBuffer, IndexType.UShort);
        
        _device.DrawIndexed(NumIndices * _currentSprite);

        _currentSprite = 0;
    }
    
    public void Dispose()
    {
        White.Dispose();
        
        _samplerState.Dispose();
        _blendState.Dispose();
        _rasterizerState.Dispose();
        
        _inputLayout.Dispose();
        _shader.Dispose();
        
        _cameraBuffer.Dispose();
        
        _indexBuffer.Dispose();
        _vertexBuffer.Dispose();
    }
}