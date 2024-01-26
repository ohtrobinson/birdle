struct VSInput
{
    float3 position: POSITION0;
    float2 texCoord: TEXCOORD0;
};

struct VSOutput
{
    float4 position: SV_Position;
    float2 texCoord: TEXCOORD0;
};

struct PSOutput
{
    float4 color: SV_Target0;
};

cbuffer CameraMatrices : register(b0)
{
    float4x4 CameraMatrix;
}

cbuffer DrawInfo : register(b1)
{
    float4x4 WorldMatrix;
    float4 Tint;
}

Texture2D sprite   : register(t2);
SamplerState state : register(s2);

VSOutput Vertex(const in VSInput input)
{
    VSOutput output;

    output.position = mul(CameraMatrix, mul(WorldMatrix, float4(input.position, 1.0)));
    output.texCoord = input.texCoord;
    
    return output;
}

PSOutput Pixel(const in VSOutput input)
{
    PSOutput output;

    output.color = sprite.Sample(state, input.texCoord) * Tint;
    
    return output;
}