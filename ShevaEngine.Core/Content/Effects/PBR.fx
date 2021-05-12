#include "Defines.fx"
#include "VertexStructures.fx"
#include "Lightning.fx"

Texture2D Texture;
sampler2D TextureSampler = sampler_state
{
	Texture = <Texture>;
};

Texture2D NormalTexture;
sampler2D NormalTextureSampler = sampler_state
{
	Texture = <NormalTexture>;
};

Texture2D RoughnessTexture;
sampler2D RoughnessTextureSampler = sampler_state
{
	Texture = <RoughnessTexture>;
};

VertexShaderOutputPNTWD MainVSPT(const VertexShaderInputPT input, matrix transform : COLOR0)
{
	VertexShaderOutputPNTWD output;

	matrix transformTrans = transpose(transform);

	output.WorldPosition = mul(float4(input.Position, 1.0), transformTrans);	
	output.Position = mul(mul(mul(float4(input.Position, 1.0), transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = float3(0,1,0);	
	output.TextureCoordinates0 = input.TextureCoordinates0;
	output.Depth = output.Position.zw;

	return output;
}

VertexShaderOutputPNTWD MainVSPNT(const VertexShaderInputPNT input, matrix transform : COLOR0)
{
	VertexShaderOutputPNTWD output;

	matrix transformTrans = transpose(transform);

	output.WorldPosition = mul(float4(input.Position, 1.0), transformTrans);	
	output.Position = mul(mul(mul(float4(input.Position, 1.0), transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans);	
	output.TextureCoordinates0 = input.TextureCoordinates0;
	output.Depth = output.Position.zw;

	return output;
}

VertexShaderOutputPNTWD MainVSPNTT(const VertexShaderInputPNTT input, matrix transform : COLOR0)
{
	VertexShaderOutputPNTWD output;

	matrix transformTrans = transpose(transform);

	output.WorldPosition = mul(float4(input.Position, 1.0), transformTrans);	
	output.Position = mul(mul(mul(float4(input.Position, 1.0), transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans);	
	output.TextureCoordinates0 = input.TextureCoordinates0;
	output.Depth = output.Position.zw;

	return output;
}

VertexShaderOutputPNTWD MainVSPNTT(const VertexShaderInputPNT input, matrix transform : COLOR0)
{
	VertexShaderOutputPNTWD output;
	
	matrix transformTrans = transpose(transform);

	output.WorldPosition = mul(float4(input.Position, 1.0), transformTrans);	
	output.Position = mul(mul(mul(float4(input.Position, 1.0), transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans);	
	output.TextureCoordinates0 = input.TextureCoordinates0;
	output.Depth = output.Position.zw;

	return output;
}

VertexShaderOutputPNTWTBD MainVSPNTTB(const VertexShaderInputPNTTB input, matrix transform : COLOR0)
{
	VertexShaderOutputPNTWTBD output;
	
	matrix transformTrans = transpose(transform);

	output.WorldPosition = mul(float4(input.Position, 1.0), transformTrans);	
	output.Position = mul(mul(mul(float4(input.Position, 1.0), transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans);	
	output.TextureCoordinates0 = input.TextureCoordinates0;
	output.Tangent = mul(input.Tangent, (float3x3)transformTrans);	
	output.Binormal = mul(input.Binormal, (float3x3)transformTrans);
	output.Depth = output.Position.zw;

	return output;
}

VertexShaderOutputPNTWTBD MainVSPTNTB(const VertexShaderInputPTNTB input, matrix transform : COLOR0)
{
	VertexShaderOutputPNTWTBD output;
	
	matrix transformTrans = transpose(transform);

	output.WorldPosition = mul(float4(input.Position, 1.0), transformTrans);	
	output.Position = mul(mul(mul(float4(input.Position, 1.0), transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans);	
	output.TextureCoordinates0 = input.TextureCoordinates0;
	output.Tangent = mul(input.Tangent, (float3x3)transformTrans);	
	output.Binormal = mul(input.Binormal, (float3x3)transformTrans);
	output.Depth = output.Position.zw;

	return output;
}

OutputWithDepth MainPSPNT(VertexShaderOutputPNTWD input)
{	
	OutputWithDepth result;

	float4 texColor = tex2D(TextureSampler,input.TextureCoordinates0);

	float3 light = ComputeDiffuseLighting(normalize(input.Normal));	
	float3 specular = ComputeSpecularLighting(normalize(input.Normal), 
		(input.WorldPosition.xyz - CameraPosition));	

	result.RenderTarget1 = float4(saturate(AmbientLight + light) * Color.rgb * texColor.rgb + saturate(specular), texColor.w);

	result.RenderTarget2 = float4(input.Depth.x,0.0f,0.0f,0.0f);	

	return result;
}

OutputWithDepth MainPSPNTWTB(VertexShaderOutputPNTWTBD input) 
{	
	OutputWithDepth result;

	float4 texColor = tex2D(TextureSampler,input.TextureCoordinates0);
	
	//read normal from tex
   	float3 tangentNormal = tex2D( NormalTextureSampler, input.TextureCoordinates0).xyz;
   	tangentNormal = normalize( tangentNormal * 2 - 1 ); //convert 0~1 to -1~+1.

   	//read from vertex shader
   	float3x3 TBN = float3x3( normalize(input.Tangent), normalize(input.Binormal),
      normalize(input.Normal) ); //transforms world=>tangent space	

   	float3 normal = mul( tangentNormal, TBN ); //note: mat * scalar		

	float3 light = ComputeDiffuseLighting(normal, input.WorldPosition);	
	float3 specular = ComputeSpecularLighting(normal, 
		normalize(input.WorldPosition.xyz - CameraPosition), input.WorldPosition);	

	result.RenderTarget1 = float4(saturate(AmbientLight + light) * Color.rgb * texColor.rgb + saturate(specular), texColor.w);

	result.RenderTarget2 = float4(input.Depth.x,0.0f,0.0f,0.0f);	

	return result;
}

technique Default
{
	pass PT20
	{
		VertexShader = compile VS_SHADERMODEL MainVSPT();
		PixelShader = compile PS_SHADERMODEL MainPSPNT();
	}


	pass PNT32
	{
		VertexShader = compile VS_SHADERMODEL MainVSPNT();
		PixelShader = compile PS_SHADERMODEL MainPSPNT();
	}

	pass PNTT40
	{
		VertexShader = compile VS_SHADERMODEL MainVSPNTT();
		PixelShader = compile PS_SHADERMODEL MainPSPNT();
	}

	pass PNTTB56
	{
		VertexShader = compile VS_SHADERMODEL MainVSPNTTB();
		PixelShader = compile PS_SHADERMODEL MainPSPNTWTB();
	}

	pass PTNTB56
	{
		VertexShader = compile VS_SHADERMODEL MainVSPTNTB();
		PixelShader = compile PS_SHADERMODEL MainPSPNTWTB();
	}
};