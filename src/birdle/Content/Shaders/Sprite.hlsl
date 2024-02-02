struct VSInput
{
    float3 position: POSITION0;
    float4 tint:    COLOR0;
    float2 texCoord: TEXCOORD0;
};

struct VSOutput
{
    float4 position: SV_Position;
    float4 tint:    COLOR0;
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

Texture2D sprite   : register(t1);
SamplerState state : register(s1);

VSOutput Vertex(const in VSInput input)
{
    VSOutput output;

    output.position = mul(CameraMatrix, float4(input.position, 1.0));
    output.tint = input.tint;
    output.texCoord = input.texCoord;
    
    return output;
}

PSOutput Pixel(const in VSOutput input)
{
    PSOutput output;

    output.color = sprite.Sample(state, input.texCoord) * input.color;
    
    return output;
}