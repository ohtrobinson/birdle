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
    private GraphicsDevice _device;

    private GraphicsBuffer _vertexBuffer;
    private GraphicsBuffer _indexBuffer;

    private GraphicsBuffer _cameraBuffer;
    private GraphicsBuffer _drawInfoBuffer;

    private Shader _shader;
    private InputLayout _inputLayout;

    private RasterizerState _rasterizerState;
    private BlendState _blendState;
    private SamplerState _samplerState;

    public readonly Texture White;

    public GraphicsDevice Device => _device;
    
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

        _cameraBuffer = device.CreateBuffer(BufferType.UniformBuffer, (uint) Unsafe.SizeOf<Matrix4x4>(), true);
        _drawInfoBuffer = device.CreateBuffer(BufferType.UniformBuffer, (uint) Unsafe.SizeOf<DrawInfo>(), true);

        string shaderText = File.ReadAllText("Content/Shaders/Sprite.hlsl");
        _shader = device.CreateShader(new[]
        {
            new ShaderAttachment(ShaderStage.Vertex, shaderText, Language.HLSL, "Vertex"),
            new ShaderAttachment(ShaderStage.Pixel, shaderText, Language.HLSL, "Pixel"),
        });

        _inputLayout = device.CreateInputLayout(new[]
        {
            new InputLayoutDescription(Format.R32G32B32_Float, 0, 0, InputType.PerVertex), // position
            new InputLayoutDescription(Format.R32G32_Float, 12, 0, InputType.PerVertex) // texCoord
        });

        _rasterizerState = device.CreateRasterizerState(RasterizerStateDescription.CullNone);
        _blendState = device.CreateBlendState(BlendStateDescription.NonPremultiplied);
        _samplerState = device.CreateSamplerState(SamplerStateDescription.LinearClamp);

        White = device.CreateTexture(new Size(1, 1), new byte[] { 255, 255, 255, 255 });
    }

    public void Draw(Texture texture, Vector2 position, Color tint, float rotation, Vector2 scale, Vector2 origin)
    {
        TextureDescription description = texture.Description;
        
        Rectangle viewport = _device.Viewport;
        Matrix4x4 projection = Matrix4x4.CreateOrthographicOffCenter(viewport.X, viewport.X + viewport.Width,
            viewport.Y + viewport.Height, viewport.Y, -1, 1);

        Matrix4x4 world = Matrix4x4.CreateTranslation(-origin.X, -origin.Y, 0) *
                          Matrix4x4.CreateScale(description.Width * scale.X, description.Height * scale.Y, 1) *
                          Matrix4x4.CreateRotationZ(rotation) *
                          Matrix4x4.CreateTranslation(position.X, position.Y, 0);

        DrawInfo info = new DrawInfo()
        {
            World = world,
            Tint = tint.Normalize()
        };
        
        _device.UpdateBuffer(_cameraBuffer, 0, projection);
        _device.UpdateBuffer(_drawInfoBuffer, 0, info);
        
        _device.SetPrimitiveType(PrimitiveType.TriangleList);
        _device.SetRasterizerState(_rasterizerState);
        _device.SetBlendState(_blendState);
        
        _device.SetUniformBuffer(0, _cameraBuffer);
        _device.SetUniformBuffer(1, _drawInfoBuffer);
        _device.SetTexture(2, texture, _samplerState);
        
        _device.SetShader(_shader);
        _device.SetInputLayout(_inputLayout);
        _device.SetVertexBuffer(0, _vertexBuffer, VertexPositionTexture.SizeInBytes);
        _device.SetIndexBuffer(_indexBuffer, IndexType.UShort);
        
        // 6 == indices.Length
        _device.DrawIndexed(6);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public void DrawRectangle(Vector2 position, Size size, Color color, float rotation, Vector2 normalizedOrigin)
    {
        Draw(White, position, color, rotation, new Vector2(size.Width, size.Height), normalizedOrigin);
    }
    
    public void Dispose()
    {
        White.Dispose();
        
        _samplerState.Dispose();
        _blendState.Dispose();
        _rasterizerState.Dispose();
        
        _inputLayout.Dispose();
        _shader.Dispose();
        
        _drawInfoBuffer.Dispose();
        _cameraBuffer.Dispose();
        
        _indexBuffer.Dispose();
        _vertexBuffer.Dispose();
    }

    private struct DrawInfo
    {
        public Matrix4x4 World;
        public Vector4 Tint;
    }
}