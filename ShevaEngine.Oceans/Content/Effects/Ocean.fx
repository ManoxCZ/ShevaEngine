#include "../../../ShevaEngine.Core/Content/Effects/Defines.fx"
#include "../../../ShevaEngine.Core/Content/Effects/VertexStructures.fx"
#include "../../../ShevaEngine.Core/Content/Effects/Lightning.fx"
#include "../../../ShevaEngine.Core/Content/Effects/Utils.fx"
#include "../../../ShevaEngine.Core/Content/Effects/TextureVariation.fx"
#include "GerstnerWaves.fx"

matrix InverseViewProjMatrix;

Texture2D<float4> RefractionTexture : register(t1);
sampler RefractionTextureSampler : register(s1)
{	
	Texture = <RefractionTexture>;
	AddressU = Clamp;
    AddressV = Clamp;
};

Texture2D<float4> NormalTexture: register(t2);
sampler NormalTextureSampler  : register(s2)
{	
	Texture = <NormalTexture>;	
	AddressU = Wrap;
    AddressV = Wrap;	
};

Texture2D<float4> DepthTexture: register(t3);
sampler DepthTextureSampler  : register(s3)
{	
	Texture = <DepthTexture>;
	AddressU = Clamp;
    AddressV = Clamp;
	MinFilter = Point; 
  	MagFilter = Point; 
};

Texture2D<float4> CausticsTexture: register(t4);
sampler CausticsTextureSampler  : register(s4)
{	
	Texture = <CausticsTexture>;
	AddressU = Wrap;
    AddressV = Wrap;	
};

float4 OceanColor;
float4 SkyColor;
float DepthFactor;
float LightFactor;


VertexShaderOutputPNTWD MainVSPT(const VertexShaderInputPT input, matrix transform : COLOR0)
{
	VertexShaderOutputPNTWD output;

	float4 start = float4(input.Position.x, input.Position.y, 0, 1);
	float4 end = float4(input.Position.x, input.Position.y, 1, 1);

	float4 startPoint = mul(start, InverseViewProjMatrix);
	float4 endPoint = mul(end, InverseViewProjMatrix);

	startPoint /= startPoint.w;
	endPoint /= endPoint.w;

	float3 direction = normalize(endPoint.xyz - startPoint.xyz);			  
	float t = -startPoint.y / (direction.y);
	
	output.WorldPosition = float4(startPoint.xyz + t * (direction), 1);

	float3 waveP = float3(0.0, 0.0, 0.0);
	float3 waveN = float3(0.0, 0.0, 0.0);
	
	GerstnerWaves(waveP, waveN, output.WorldPosition.xz, GameTime);

	output.WorldPosition.xyz += waveP.xzy;	
	output.Position = mul(mul(output.WorldPosition, ViewMatrix), ProjMatrix);	
	output.Normal = waveN.xzy;
	output.TextureCoordinates0 = (output.Position.xy / output.Position.w + 1) * 0.5;
	output.TextureCoordinates0.y = 1 - output.TextureCoordinates0.y;
	output.Depth = output.Position.zw;



	return output;
}


float fresnel(in float3 I, in float3 N, in float power)	
{	
	return pow(1.0-abs(dot(I, N)), power);	
}

float4 MainPSPN(VertexShaderOutputPNTWD input) : COLOR
{			
	float depth = tex2D(DepthTextureSampler, input.TextureCoordinates0).x;

	if (input.Depth.x > depth && depth > 0.0001)		
		return tex2D(RefractionTextureSampler, input.TextureCoordinates0);

	float ratio = length(CameraPosition.xyz - input.WorldPosition.xyz) / 50;

	float2 normUV1 = input.WorldPosition.xz + float2(GameTime / 12, GameTime / 16);
	float2 normUV2 = input.WorldPosition.xz * 0.2 + float2(-GameTime / 20, GameTime / 10);

	float3 normTextureColor =		
	  	tex2D(NormalTextureSampler, normUV1).xzy +
	  	tex2D(NormalTextureSampler, normUV2).xzy;
	
	float3 normal = normalize(normTextureColor + lerp(input.Normal, float3(0,1,0), ratio));	

	
	//return float4(tex2D(NormalTextureSampler, input.WorldPosition.xz % 1.0f).xyz, 1);

	float3 view = normalize(CameraPosition - input.WorldPosition.xyz);
	float3 light = lerp(ComputeDiffuseLighting(normal, input.WorldPosition), float3(1,1,1), LightFactor);		

	float3 specular = ComputeSpecularLighting(-normal, -view, input.WorldPosition);		

	float fres = fresnel(view, normal , 4);	
	
	float3 sky1 = SkyColor.xyz * 1;
	float3 sky2 = SkyColor.xyz * 0.6;
	float3 refl = lerp(sky2, sky1, fres);

	float2 refrUV = input.TextureCoordinates0 + ((normal.xz - 0.25) * 0.04);		
	depth = tex2D(DepthTextureSampler, refrUV).x;
	
	float3 refr = OceanColor.xyz;	
	float3 emission = float3(0,0,0);

	if (input.Depth.x < (depth + 0.5))		
	{
		refr = tex2D(RefractionTextureSampler, refrUV).xyz;			

		emission = tex2D(CausticsTextureSampler, input.WorldPosition.xz * 0.5).xyz * float3(0.5,0.40,0.25) * 0.5f;
		emission = lerp(emission, float3(0,0,0), min(1, abs(depth - input.Depth.x) * DepthFactor));		

		refr = lerp(refr, OceanColor.xyz, min(1, abs(depth - input.Depth.x) * DepthFactor));		
	}	 	
	else	
	{
		refr = tex2D(RefractionTextureSampler, input.TextureCoordinates0).xyz;			

		depth = tex2D(DepthTextureSampler, input.TextureCoordinates0).x;

		refr = lerp(refr, OceanColor.xyz, min(1, abs(depth - input.Depth.x) * DepthFactor));		
	}	 						

	return float4(saturate(AmbientLight + light) * lerp(refr, refl, 0 + fres) + emission + saturate(specular), 1);	
}

technique Default
{
	pass PT20
	{
		VertexShader = compile VS_SHADERMODEL MainVSPT();
		PixelShader = compile PS_SHADERMODEL MainPSPN();
	}	
};