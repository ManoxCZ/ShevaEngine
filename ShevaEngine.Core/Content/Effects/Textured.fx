#include "Defines.fx"
#include "VertexStructures.fx"
#include "Lightning.fx"
#include "Skinned.fx"

Texture2D Texture;
sampler2D TextureSampler = sampler_state
{
	Texture = <Texture>;
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

VertexShaderOutputPNTWD MainVSPBBNT(const VertexShaderInputPBBNT input, matrix transform : COLOR0)
{
	VertexShaderOutputPNTWD output;
	
	matrix transformTrans = transpose(transform);

	output.WorldPosition = mul(input.Position, transformTrans);	
	output.Position = mul(mul(mul(input.Position, transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans);	
	output.TextureCoordinates0 = input.TextureCoordinates0;
	output.Depth = output.Position.zw;

	return output;
}

VertexShaderOutputPNTWD MainVSPBBNTAnimated(VertexShaderInputPBBNT input, matrix transform : COLOR0)
{
	VertexShaderOutputPNTWD output;
	
	matrix transformTrans = transpose(transform);

	Skin(input, 4);

	output.WorldPosition = mul(input.Position, transformTrans);	
	output.Position = mul(mul(mul(input.Position, transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans);	
	output.Depth = output.Position.zw;
	output.TextureCoordinates0 = input.TextureCoordinates0;	

	return output;
}

VertexShaderOutputPNTWD MainVSPBBNTTBAnimated(VertexShaderInputPBBNTTB input, matrix transform : COLOR0)
{
	VertexShaderOutputPNTWD output;
	
	matrix transformTrans = transpose(transform);

	Skin(input, 4);

	output.WorldPosition = mul(input.Position, transformTrans);	
	output.Position = mul(mul(mul(input.Position, transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans);	
	output.Depth = output.Position.zw;
	output.TextureCoordinates0 = input.TextureCoordinates0;	

	return output;
}

OutputWithDepth MainPSPNT(VertexShaderOutputPNTWD input)
{	
	OutputWithDepth result;

	float4 texColor = tex2D(TextureSampler,input.TextureCoordinates0);

	clip(texColor.a - 0.5f);

	float3 light = ComputeDiffuseLighting(normalize(input.Normal), input.WorldPosition);	
	
	float3 specular = ComputeSpecularLighting(normalize(input.Normal), 
		normalize(CameraPosition - input.WorldPosition.xyz), input.WorldPosition);	

	result.RenderTarget1 = float4(saturate(AmbientLight + light) * Color.rgb * texColor.rgb + saturate(specular), texColor.w);		

	result.RenderTarget2 = float4(input.Depth.x,1.0f,1.0f,1.0f);	

	return result;
}

float4 MainPSPNTShadows(VertexShaderOutputPNTWD input) : COLOR
{	
	return float4(input.Depth.x / input.Depth.y,0.0f,0.0f,0.0f);	
	//return float4(input.Depth,0.0f,0.0f,0.0f);	
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

	pass PBBNT52
	{
		VertexShader = compile VS_SHADERMODEL MainVSPBBNT();
		PixelShader = compile PS_SHADERMODEL MainPSPNT();
	}

	pass PBBNT52Animated
	{
		VertexShader = compile VS_SHADERMODEL MainVSPBBNTAnimated();
		PixelShader = compile PS_SHADERMODEL MainPSPNT();
	}

	pass PBBNTTB76Animated
	{
		VertexShader = compile VS_SHADERMODEL MainVSPBBNTTBAnimated();
		PixelShader = compile PS_SHADERMODEL MainPSPNT();
	}
};

technique DefaultShadows
{
	pass PT20
	{
		VertexShader = compile VS_SHADERMODEL MainVSPT();
		PixelShader = compile PS_SHADERMODEL MainPSPNTShadows();
	}

	pass PNT32
	{
		VertexShader = compile VS_SHADERMODEL MainVSPNT();
		PixelShader = compile PS_SHADERMODEL MainPSPNTShadows();
	}

	pass PNTT40
	{
		VertexShader = compile VS_SHADERMODEL MainVSPNTT();
		PixelShader = compile PS_SHADERMODEL MainPSPNTShadows();
	}

	pass PBBNT52
	{
		VertexShader = compile VS_SHADERMODEL MainVSPBBNT();
		PixelShader = compile PS_SHADERMODEL MainPSPNTShadows();
	}

	pass PBBNT52Animated
	{
		VertexShader = compile VS_SHADERMODEL MainVSPBBNTAnimated();
		PixelShader = compile PS_SHADERMODEL MainPSPNTShadows();
	}

	pass PBBNTTB76Animated
	{
		VertexShader = compile VS_SHADERMODEL MainVSPBBNTTBAnimated();
		PixelShader = compile PS_SHADERMODEL MainPSPNTShadows();
	}
};

