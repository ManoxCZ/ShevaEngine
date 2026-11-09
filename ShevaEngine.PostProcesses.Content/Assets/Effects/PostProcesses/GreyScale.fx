#include "PPVertexShader.fxh"


Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};

float4 GreyScale(VertexShaderOutput input) : SV_Target0
{
	float4 col = SpriteTexture.Sample(SpriteTextureSampler, input.TexCoord);

	float value = (col.r + col.g + col.b) / 3;
	col = float4(value, value, value, col.a);

	return col;
}

technique PostInvert
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL GreyScale();
	}
}