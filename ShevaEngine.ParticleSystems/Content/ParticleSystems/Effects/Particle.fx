#include "../../../../ShevaEngine.Core/Content/Effects/Defines.fx"
#include "../../../../ShevaEngine.Core/Content/Effects/VertexStructures.fx"

float Lifetime;
float4 StartColor;
float4 EndColor;
float StartSize;
float EndSize;

Texture2D Texture;
sampler2D TextureSampler = sampler_state
{
	Texture = <Texture>;
};

VertexShaderOutputPCTWD MainVSPT(const VertexShaderInputPT input, matrix transform : COLOR0)
{
	VertexShaderOutputPCTWD output;

	matrix transformTrans = transpose(transform);

	float lifeRatio = transformTrans[3][3] / Lifetime;

	transformTrans[3][3] = 1;	

	float3 scaledPosition = lerp(EndSize, StartSize, lifeRatio) * input.Position;	
	output.WorldPosition = mul(float4(scaledPosition, 1.0), transformTrans);	
	output.Position = mul(mul(mul(float4(scaledPosition, 1.0), transformTrans), ViewMatrix), ProjMatrix);
	output.Color = lerp(EndColor, StartColor, lifeRatio);	
	output.TextureCoordinates0 = input.TextureCoordinates0;
	output.Depth = output.Position.zw;

	return output;
}


OutputWithDepth MainPSPNT(VertexShaderOutputPCTWD input)
{	
	OutputWithDepth result;

	float4 texColor = tex2D(TextureSampler,input.TextureCoordinates0);	

	clip (input.Color.a * texColor.a - 0.1);

	result.RenderTarget1 = float4(input.Color.rgb * texColor.rgb, input.Color.a * texColor.a);			
	result.RenderTarget2 = float4(input.Depth.x,1.0f,1.0f,1.0f);	

	return result;
}

technique Default
{
	pass PT20
	{
		VertexShader = compile VS_SHADERMODEL MainVSPT();
		PixelShader = compile PS_SHADERMODEL MainPSPNT();
	}
};



