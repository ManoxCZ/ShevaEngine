#include "PPVertexShader.fxh"

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

float4 Invert(VertexShaderOutput input) : SV_Target0
{

	float4 col = (float4)1 - SpriteTexture.Sample(SpriteTextureSampler, input.TexCoord);

	return col;
}

technique PostInvert
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL Invert();
	}
}