#include "PPVertexShader.fxh"

// Pixel shader extracts the brighter areas of an image.
// This is the first step in applying a bloom postprocess.

uniform extern float BloomThreshold;

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};


float4 BrightPassPS(VertexShaderOutput input) : COLOR0
{
    // Look up the original image color.
    float4 c = tex2D(SpriteTextureSampler, input.TexCoord);

    // Adjust it to keep only values brighter than the specified threshold.
    return saturate((c - BloomThreshold) / (1 - BloomThreshold));
}


technique BloomExtract
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL BrightPassPS();
        
        //ZWriteEnable = false;
    }
}