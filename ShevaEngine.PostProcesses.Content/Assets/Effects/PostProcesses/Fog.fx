#include "PPVertexShader.fxh"

Texture2D ScreenTexture;

sampler screen : register(s0);

Texture2D depthMap;
Texture2D DepthTexture;

sampler DepthMap = sampler_state
{
	Texture = <depthMap>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};


float camMin;
float camMax;
float fogRange;
float fogDistance;
float4 fogColor;
float fogMinThickness = .999f;
float fogMaxThickness = 0.0f;


float4 FogPS(VertexShaderOutput Input) : SV_Target0
{
	float4 s = ScreenTexture.Sample(screen, Input.TexCoord);
	float depth = 1 - DepthTexture.Sample(DepthMap,Input.TexCoord).r;

	float fSceneZ = ( -camMin * ((camMax /(camMax - camMin))) ) / ( depth - ((camMax /(camMax - camMin))));
	float fFogFactor = clamp(saturate( ( fSceneZ - fogDistance ) / fogRange ),fogMaxThickness,fogMinThickness);

	return lerp(s,fogColor,fFogFactor);

	
}

technique Fog 
{
	pass P0
	{
		PixelShader = compile PS_SHADERMODEL FogPS();
	}
}