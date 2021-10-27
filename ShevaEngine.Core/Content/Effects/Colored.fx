#include "Defines.fx"
#include "VertexStructures.fx"
#include "Lightning.fx"
#include "Skinned.fx"

VertexShaderOutputPNWD MainVSP(const VertexShaderInputP input, matrix transform : COLOR0)
{
    VertexShaderOutputPNWD output;

    matrix transformTrans = transpose(transform);

    output.WorldPosition = mul(float4(input.Position, 1.0), transformTrans);
    output.Position = mul(mul(mul(float4(input.Position, 1.0), transformTrans), ViewMatrix), ProjMatrix);
    output.Normal = mul(float3(0, 1, 0), (float3x3) transformTrans);
    output.Depth = output.Position.zw;

    return output;
}

VertexShaderOutputPNWD MainVSPN(const VertexShaderInputPN input, matrix transform : COLOR0)
{
	VertexShaderOutputPNWD output;

	matrix transformTrans = transpose(transform);

	output.WorldPosition = mul(float4(input.Position, 1.0), transformTrans);	
	output.Position = mul(mul(mul(float4(input.Position, 1.0), transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans);	
	output.Depth = output.Position.zw;

	return output;
}

VertexShaderOutputPNWD MainVSPNT(const VertexShaderInputPNT input, matrix transform : COLOR0)
{
	VertexShaderOutputPNWD output;

	matrix transformTrans = transpose(transform);

	output.WorldPosition = mul(float4(input.Position, 1.0), transformTrans);	
	output.Position = mul(mul(mul(float4(input.Position, 1.0), transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans).xyz;	
	output.Depth = output.Position.zw;

	return output;
}

VertexShaderOutputPNWD MainVSPNTT(const VertexShaderInputPNTT input, matrix transform : COLOR0)
{
	VertexShaderOutputPNWD output;

	matrix transformTrans = transpose(transform);

	output.WorldPosition = mul(float4(input.Position, 1.0), transformTrans);	
	output.Position = mul(mul(mul(float4(input.Position, 1.0), transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans).xyz;	
	output.Depth = output.Position.zw;

	return output;
}

VertexShaderOutputPNWD MainVSPBBN(const VertexShaderInputPBBN input, matrix transform : COLOR0)
{
	VertexShaderOutputPNWD output;

	matrix transformTrans = transpose(transform);

	output.WorldPosition = mul(float4(input.Position, 1.0), transformTrans);	
	output.Position = mul(mul(mul(float4(input.Position, 1.0), transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans);	
	output.Depth = output.Position.zw;

	return output;
}

VertexShaderOutputPNWD MainVSPBBNT(const VertexShaderInputPBBNT input, matrix transform : COLOR0)
{
	VertexShaderOutputPNWD output;

	matrix transformTrans = transpose(transform);

	output.WorldPosition = mul(input.Position, transformTrans);	
	output.Position = mul(mul(mul(input.Position, transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans);	
	output.Depth = output.Position.zw;

	return output;
}

VertexShaderOutputPNWD MainVSPBBNTAnimated(VertexShaderInputPBBNT input, matrix transform : COLOR0)
{
	VertexShaderOutputPNWD output;

	matrix transformTrans = transpose(transform);

	Skin(input, 4);

	output.WorldPosition = mul(input.Position, transformTrans);	
	output.Position = mul(mul(mul(input.Position, transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans);	
	output.Depth = output.Position.zw;

	return output;
}

VertexShaderOutputPNWD MainVSPBBNTTT(const VertexShaderInputPBBNTTT input, matrix transform : COLOR0)
{
	VertexShaderOutputPNWD output;

	matrix transformTrans = transpose(transform);

	output.WorldPosition = mul(input.Position, transformTrans);	
	output.Position = mul(mul(mul(input.Position, transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans);	
	output.Depth = output.Position.zw;

	return output;
}

VertexShaderOutputPNWD MainVSPBBNTTTAnimated(VertexShaderInputPBBNTTT input, matrix transform : COLOR0)
{
	VertexShaderOutputPNWD output;

	matrix transformTrans = transpose(transform);

	Skin(input, 4);

	output.WorldPosition = mul(input.Position, transformTrans);	
	output.Position = mul(mul(mul(input.Position, transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans);	
	output.Depth = output.Position.zw;

	return output;
}

VertexShaderOutputPNWD MainVSPNTTT(const VertexShaderInputPNTTT input, matrix transform : COLOR0)
{
	VertexShaderOutputPNWD output;

	matrix transformTrans = transpose(transform);

	output.WorldPosition = mul(input.Position, transformTrans);	
	output.Position = mul(mul(mul(input.Position, transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans);	
	output.Depth = output.Position.zw;

	return output;
}

OutputWithDepth MainPSPN(VertexShaderOutputPNWD input)
{	
	OutputWithDepth result;

	float3 light = ComputeDiffuseLighting(normalize(input.Normal), input.WorldPosition);	
	float3 specular = ComputeSpecularLighting(normalize(input.Normal), normalize(CameraPosition - input.WorldPosition.xyz), input.WorldPosition);	

	result.RenderTarget1 = float4(saturate(AmbientLight + light) * Color.rgb + saturate(specular), Color.w);	
	result.RenderTarget2 = float4(input.Depth.x,1.0f,1.0f,1.0f);	

	return result;
}

float4 MainPSPNShadows(VertexShaderOutputPNWD input) : COLOR
{	
	return float4(input.Depth.x / input.Depth.y,0.0f,0.0f,0.0f);	
	//return float4(input.Depth,0.0f,0.0f,0.0f);	
}

technique Default
{
    pass P12
    {
        VertexShader = compile VS_SHADERMODEL MainVSP();
        PixelShader = compile PS_SHADERMODEL MainPSPN();
    }

	pass PN24
	{
		VertexShader = compile VS_SHADERMODEL MainVSPN();
		PixelShader = compile PS_SHADERMODEL MainPSPN();
	}

	pass PNT32
	{
		VertexShader = compile VS_SHADERMODEL MainVSPNT();
		PixelShader = compile PS_SHADERMODEL MainPSPN();
	}

	pass PNTT40
	{
		VertexShader = compile VS_SHADERMODEL MainVSPNTT();
		PixelShader = compile PS_SHADERMODEL MainPSPN();
	}

	pass PBBN44
	{
		VertexShader = compile VS_SHADERMODEL MainVSPBBN();
		PixelShader = compile PS_SHADERMODEL MainPSPN();
	}

	pass PBBNT52
	{
		VertexShader = compile VS_SHADERMODEL MainVSPBBNT();
		PixelShader = compile PS_SHADERMODEL MainPSPN();
	}

	pass PBBNT52Animated
	{
		VertexShader = compile VS_SHADERMODEL MainVSPBBNTAnimated();
		PixelShader = compile PS_SHADERMODEL MainPSPN();
	}

	pass PBBNTTT68
	{
		VertexShader = compile VS_SHADERMODEL MainVSPBBNTTT();
		PixelShader = compile PS_SHADERMODEL MainPSPN();
	}

	pass PBBNTTT68Animated
	{
		VertexShader = compile VS_SHADERMODEL MainVSPBBNTTTAnimated();
		PixelShader = compile PS_SHADERMODEL MainPSPN();
	}

	pass PNTTT72
	{
		VertexShader = compile VS_SHADERMODEL MainVSPNTTT();
		PixelShader = compile PS_SHADERMODEL MainPSPN();
	}
};

technique Shadows
{
    pass P12
    {
        VertexShader = compile VS_SHADERMODEL MainVSP();
        PixelShader = compile PS_SHADERMODEL MainPSPNShadows();
    }

	pass PN24
	{
		VertexShader = compile VS_SHADERMODEL MainVSPN();
		PixelShader = compile PS_SHADERMODEL MainPSPNShadows();
	}

	pass PNT32
	{
		VertexShader = compile VS_SHADERMODEL MainVSPNT();
		PixelShader = compile PS_SHADERMODEL MainPSPNShadows();
	}

	pass PNTT40
	{
		VertexShader = compile VS_SHADERMODEL MainVSPNTT();
		PixelShader = compile PS_SHADERMODEL MainPSPNShadows();
	}

	pass PBBN44
	{
		VertexShader = compile VS_SHADERMODEL MainVSPBBN();
		PixelShader = compile PS_SHADERMODEL MainPSPNShadows();
	}

	pass PBBNT52
	{
		VertexShader = compile VS_SHADERMODEL MainVSPBBNT();
		PixelShader = compile PS_SHADERMODEL MainPSPNShadows();
	}

	pass PBBNT52Animated
	{
		VertexShader = compile VS_SHADERMODEL MainVSPBBNTAnimated();
		PixelShader = compile PS_SHADERMODEL MainPSPNShadows();
	}

	pass PBBNTTT68
	{
		VertexShader = compile VS_SHADERMODEL MainVSPBBNTTT();
		PixelShader = compile PS_SHADERMODEL MainPSPNShadows();
	}

	pass PBBNTTT68Animated
	{
		VertexShader = compile VS_SHADERMODEL MainVSPBBNTTTAnimated();
		PixelShader = compile PS_SHADERMODEL MainPSPNShadows();
	}

	pass PNTTT72
	{
		VertexShader = compile VS_SHADERMODEL MainVSPNTTT();
		PixelShader = compile PS_SHADERMODEL MainPSPNShadows();
	}
};