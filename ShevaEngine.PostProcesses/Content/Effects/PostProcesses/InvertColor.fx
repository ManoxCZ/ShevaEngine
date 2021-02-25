#include "PPVertexShader.fxh"

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

float4 Invert(VertexShaderOutput input) : COLOR0
{

	float4 col = (float4)1 - tex2D(SpriteTextureSampler, input.TexCoord);

	return col;
}

technique PostInvert
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL Invert();
	}
}