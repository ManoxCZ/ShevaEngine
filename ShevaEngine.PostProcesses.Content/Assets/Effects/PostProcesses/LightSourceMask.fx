#include "PPVertexShader.fxh"

float3 lightPosition;

float4x4 matVP;

float SunSize = 1500;

Texture2D ScreenTexture;

sampler screen : register(s0);

Texture2D FlareTexture;
Texture2D FlareTexture;

sampler Flare = sampler_state
{
    Texture = (flare);
    AddressU = CLAMP;
    AddressV = CLAMP;
};



float4 LightSourceMaskPS(VertexShaderOutput input) : SV_Target0
{
	float4 v = ScreenTexture.Sample(screen,input.TexCoord) * .001;

	// Get the scene
	float4 col = 0;
	
	// Find the suns position in the world and map it to the screen space.
	float4 ScreenPosition = mul(lightPosition,matVP);
	float scale = ScreenPosition.z;
	ScreenPosition.xyz /= ScreenPosition.w;
	ScreenPosition.x = ScreenPosition.x/2.0f+0.5f;
	ScreenPosition.y = (-ScreenPosition.y/2.0f+0.5f);

	// Are we looking in the direction of the sun?
	if(ScreenPosition.w > 0)
	{		
		float2 coord;
		
		float size = SunSize / scale;
					
		float2 center = ScreenPosition.xy;

		coord = .5 - (input.TexCoord - center) / size * .5;
		col += (pow(FlareTexture.Sample(Flare,coord),2) * 1) * 2;						
	}	

	return v+col;
}

technique LightSourceMask
{
	pass p0
	{
		//VertexShader = compile VS_SHADERMODEL VertexShaderFunction();
		PixelShader = compile PS_SHADERMODEL LightSourceMaskPS();
	}
}