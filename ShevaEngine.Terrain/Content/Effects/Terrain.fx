#include "../../../ShevaEngine.Core/Content/Effects/Defines.fx"
#include "../../../ShevaEngine.Core/Content/Effects/VertexStructures.fx"
#include "../../../ShevaEngine.Core/Content/Effects/Lightning.fx"
#include "../../../ShevaEngine.Core/Content/Effects/Utils.fx"
#include "../../../ShevaEngine.Core/Content/Effects/Skinned.fx"
#include "../../../ShevaEngine.Core/Content/Effects/TextureVariation.fx"

static const float pi = 3.14159265;
static const float cos30Deg = cos(pi / 6);
float TextureSize = 512;
float TileSize = 4.0f;
float VerticalScale = 25.0f;
float WaterLevel = 0;

Texture2D HeightmapTexture;
sampler2D HeightmapTextureSampler = sampler_state
{
	Texture = <HeightmapTexture>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = POINT;
    MinFilter = POINT;
    MagFilter = POINT;
};

Texture2D SplatMapTexture;
sampler SplatMapSample = sampler_state
{
	texture = <SplatMapTexture>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

Texture2D TextureChannel0;
sampler TextureChannel0Sample = sampler_state
{
	texture = <TextureChannel0>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

Texture2D TextureChannel0Norm;
sampler TextureChannel0NormSample = sampler_state
{
	texture = <TextureChannel0Norm>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

Texture2D TextureChannel1;
sampler TextureChannel1Sample = sampler_state
{
	texture = <TextureChannel1>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

Texture2D TextureChannel1Norm;
sampler TextureChannel1NormSample = sampler_state
{
	texture = <TextureChannel1Norm>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

Texture2D TextureChannel2;
sampler TextureChannel2Sample = sampler_state
{
	texture = <TextureChannel2>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

Texture2D TextureChannel2Norm;
sampler TextureChannel2NormSample = sampler_state
{
	texture = <TextureChannel2Norm>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};


Texture2D TextureChannel3;
sampler TextureChannel3Sample = sampler_state
{
	texture = <TextureChannel3>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};

Texture2D TextureChannel3Norm;
sampler TextureChannel3NormSample = sampler_state
{
	texture = <TextureChannel3Norm>;
	AddressU = Wrap;
	AddressV = Wrap;
	MipFilter = LINEAR;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
};


 float3 FindNormal(sampler2D tex, float2 uv, float u)
{
    //u is one uint size, ie 1.0/texture size
    float2 offsets[4];
    offsets[0] = uv + float2(-u, 0);
    offsets[1] = uv + float2(u, 0);
    offsets[2] = uv + float2(0, -u);
    offsets[3] = uv + float2(0, u);
               
	float hts[4];
	[unroll]
    for(int i = 0; i < 4; i++)
    {
        hts[i] = tex2Dlod(tex, float4(offsets[i],0,0)).x * 25.0f;
    }
               
    float2 _step = float2(1.0, 0.0);
               
    float3 va = normalize( float3(_step.xy, hts[1]-hts[0]) );
    float3 vb = normalize( float3(_step.yx, hts[3]-hts[2]) );
               
    return normalize(cross(va,vb).yzx); 
}

VertexShaderOutputPNTWD MainVSP(const VertexShaderInputP input, matrix transform : COLOR0)
{
	VertexShaderOutputPNTWD output;    
    
	matrix transformTrans = transpose(transform);	
    
    int x = transformTrans[0][3];
    int y = transformTrans[1][3];
       
    float4 position = float4(input.Position * TileSize 
        + float3(
        x * TileSize * 3 + ((y % 2 != 0) ? TileSize * 1.5f : 0),
        0,
        (float) (y * TileSize * cos30Deg)), 1.0);
    
    float4 texColor = tex2Dlod(HeightmapTextureSampler, float4(position.xz / TextureSize, 0, 0));
    
    position.y = texColor.r * VerticalScale - WaterLevel;
    
    output.WorldPosition = position;
    output.Position = mul(mul(position, ViewMatrix), ProjMatrix);
    output.Normal = FindNormal(HeightmapTextureSampler, position.zx / TextureSize, 1 / TextureSize);
    output.TextureCoordinates0 = position.xz / TextureSize;
	output.Depth = output.Position.zw;

	return output;
}

OutputWithDepth MainPSPN(VertexShaderOutputPNTWD input)
{	
	OutputWithDepth result;

	float4 splat = tex2D(SplatMapSample, input.TextureCoordinates0);		

	float2 uv = input.TextureCoordinates0 * 128;

	// float4x4 col;	
	// col[0] = tex2D(TextureChannel0Sample, uv);
	// col[1] = tex2D(TextureChannel1Sample, uv);
	// col[2] = tex2D(TextureChannel2Sample, uv);
	// col[3] = tex2D(TextureChannel3Sample, uv);

	// float4x4 norm;
	// // Normal
	// norm[0] = tex2D(TextureChannel0NormSample, uv);
	// norm[1] = tex2D(TextureChannel1NormSample, uv);
	// norm[2] = tex2D(TextureChannel2NormSample, uv);
	// norm[3] = tex2D(TextureChannel3NormSample, uv);

	float4x4 col;	
	col[0] = textureNoTile(TextureChannel0, TextureChannel0Sample, uv);
	col[1] = textureNoTile(TextureChannel1, TextureChannel1Sample, uv);
	col[2] = textureNoTile(TextureChannel2, TextureChannel2Sample, uv);
	col[3] = textureNoTile(TextureChannel3, TextureChannel3Sample, uv);

	float4x4 norm;
	// Normal
	norm[0] = textureNoTile(TextureChannel0Norm, TextureChannel0NormSample, uv);
	norm[1] = textureNoTile(TextureChannel1Norm, TextureChannel1NormSample, uv);
	norm[2] = textureNoTile(TextureChannel2Norm, TextureChannel2NormSample, uv);
	norm[3] = textureNoTile(TextureChannel3Norm, TextureChannel3NormSample, uv);

	float3 n = 2.0f * mul(splat, norm) - 1.0f;
	n = normalize(n * 0.8f + input.Normal);
	n = normalize(input.Normal);
	
	float3 light = ComputeDiffuseLighting(n, input.WorldPosition);	
	
	//result.RenderTarget1 = float4(saturate(light) * mul(splat, col), 1);	
    result.RenderTarget1 = float4(saturate(light) * splat, 1);
	//result.RenderTarget1 = float4(input.Normal, 1.0f);
	// result.RenderTarget1 = float4(light, 1.0f);
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
};

technique Shadows
{
	pass P12
	{
		VertexShader = compile VS_SHADERMODEL MainVSP();
		PixelShader = compile PS_SHADERMODEL MainPSPNShadows();
	}
};