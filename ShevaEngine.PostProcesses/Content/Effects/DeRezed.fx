#include "PPVertexShader.fxh"


Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

int  NumberOfTiles = 8;

float4 DeRezPixelShader(VertexShaderOutput input) : COLOR0
{
	float2 uv = input.TexCoord;

	// Get Pixel size based on number of tiles..
	float size = 1.0 / NumberOfTiles;


	float4 col = tex2D(SpriteTextureSampler, (uv - fmod(uv, size.xx)) + (size / 2.0).xx);

	return col;
}

technique DeRez
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL DeRezPixelShader();

		//ZWriteEnable = false;
	}
}