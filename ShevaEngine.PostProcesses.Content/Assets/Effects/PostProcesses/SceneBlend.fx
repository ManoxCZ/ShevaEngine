#include "PPVertexShader.fxh"

sampler2D Scene: register(s0){
	AddressU = Mirror;
	AddressV = Mirror;
};

Texture2D OrgSceneTexture;
sampler2D orgScene = sampler_state
{
	Texture = <OrgScene>;
	AddressU = CLAMP;
	AddressV = CLAMP;
};


float4 BlendPS(VertexShaderOutput input) : SV_Target0
{
	float4 col = SceneTexture.Sample(Scene, input.TexCoord) * orgSceneTexture.Sample(orgScene, input.TexCoord);

	return col;
}

float4 AditivePS(VertexShaderOutput input) : SV_Target0
{
	float4 col = SceneTexture.Sample(Scene, input.TexCoord) + orgSceneTexture.Sample(orgScene, input.TexCoord);

	return col;
}

technique Blend
{
	pass p0
	{
		//VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL BlendPS();
	}
}

technique Aditive
{
	pass p0
	{
		//VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL AditivePS();
	}
}