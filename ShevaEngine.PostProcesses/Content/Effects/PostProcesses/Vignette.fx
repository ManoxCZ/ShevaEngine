#include "PPVertexShader.fxh"

#define VignetteCenter float2(0.500, 0.500)  //[0.000 to 1.000, 0.000 to 1.000] Center of effect for VignetteType 1. 2 and 3 do not obey this setting.

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

float2 ScreenSize;
float2 PixelSize;
float Slope = 2;
float Radius = -0.45;
float Amount = -0.15;

float xor( float xor_A, float xor_B )
{
  return saturate( dot(float4(-xor_A ,-xor_A ,xor_A , xor_B) , float4(xor_B, xor_B ,1.0 ,1.0 ) ) ); // -2 * A * B + A + B
}

float4 VignetteType1(VertexShaderOutput input) : COLOR0
{
    float4 texColor = tex2D(SpriteTextureSampler, input.TexCoord);

	float2 tc = input.TexCoord - VignetteCenter;

	tc *= float2((PixelSize.y / PixelSize.x), (PixelSize.y / PixelSize.x));

	tc /= Radius;
	float v = dot(tc,tc);

	texColor.rgb *= (1.0 + pow(v, Slope * 0.5) * Amount); //pow - multiply

    return texColor;
}

technique Type1
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL VignetteType1();	
	}
}