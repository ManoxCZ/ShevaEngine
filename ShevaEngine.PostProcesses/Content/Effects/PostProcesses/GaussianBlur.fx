#include "PPVertexShader.fxh"
// Pixel shader applies a one dimensional gaussian blur filter.
// This is used twice by the bloom postprocess, first to
// blur horizontally, and then again to blur vertically.

#define SAMPLE_COUNT 5

uniform extern float2 SampleOffsets[SAMPLE_COUNT];
uniform extern float SampleWeights[SAMPLE_COUNT];

sampler TextureSampler : register(s0);


float4 GaussianBlurPS(VertexShaderOutput input) : COLOR0
{
    float4 color = tex2D(TextureSampler, input.TexCoord) * SampleWeights[0];
    
    for (int i = 1; i < SAMPLE_COUNT; i++) 
    {
        color += tex2D(TextureSampler, input.TexCoord + SampleOffsets[i]) * SampleWeights[i];
        color += tex2D(TextureSampler, input.TexCoord - SampleOffsets[i]) * SampleWeights[i];
    }

    return color;

	// float4 c = 0;
    
    // // Combine a number of weighted image filter taps.
    // for (int i = 0; i < SAMPLE_COUNT; i++)
    // {
    //     c += tex2D(TextureSampler, saturate(input.TexCoord + SampleOffsets[i])) * SampleWeights[i];
    // }
    
    // return c;
}


technique GaussianBlur
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL GaussianBlurPS();
        
    }
}