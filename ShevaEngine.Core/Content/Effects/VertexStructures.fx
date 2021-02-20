struct VertexShaderInputPN
{
	float3 Position : POSITION;
	float3 Normal : NORMAL;	
};

struct VertexShaderOutputPN
{
	float4 Position : SV_POSITION;
	float3 Normal : TEXCOORD0;	
};

struct VertexShaderInputPT
{
	float3 Position : POSITION;
	float2 TextureCoordinates0 : TEXCOORD0;
};

struct VertexShaderOutputPT
{
	float4 Position : SV_POSITION;
	float2 TextureCoordinates0 : TEXCOORD0;
};

struct VertexShaderOutputPNW
{
	float4 Position : SV_POSITION;
	float3 Normal : TEXCOORD0;	
    float4 WorldPosition : TEXCOORD1;
};

struct VertexShaderOutputPNWD
{
	float4 Position : SV_POSITION;
	float3 Normal : TEXCOORD0;	
    float4 WorldPosition : TEXCOORD1;
	float2 Depth : COLOR;
};

struct VertexShaderOutputPNTW
{
	float4 Position : SV_POSITION;
	float3 Normal : TEXCOORD0;	
	float2 TextureCoordinates0 : TEXCOORD1;
    float4 WorldPosition : TEXCOORD2;
};

struct VertexShaderOutputPNTWD
{
	float4 Position : SV_POSITION;
	float3 Normal : TEXCOORD0;	
	float2 TextureCoordinates0 : TEXCOORD1;
    float4 WorldPosition : TEXCOORD2;
	float2 Depth : COLOR;
};

struct VertexShaderInputPNT
{
	float3 Position : POSITION;
	float3 Normal : NORMAL;
	float2 TextureCoordinates0 : TEXCOORD0;
};

struct VertexShaderOutputPNT
{
	float4 Position : SV_POSITION;
	float3 Normal : TEXCOORD0;
	float2 TextureCoordinates0 : TEXCOORD1;
};

struct VertexShaderInputPNTT
{
	float3 Position : POSITION;
	float3 Normal : NORMAL;
	float2 TextureCoordinates0 : TEXCOORD0;
    float2 TextureCoordinates1 : TEXCOORD1;
};

struct VertexShaderOutputPNTT
{
	float4 Position : SV_POSITION;
	float3 Normal : TEXCOORD0;
	float2 TextureCoordinates0 : TEXCOORD1;
    float2 TextureCoordinates1 : TEXCOORD2;
};

struct VertexShaderOutputPTIP
{
	float4 Position : SV_POSITION;
	float2 TexCoord : TEXCOORD0;
	float4 InvPosition : TEXCOORD1;
};

struct VertexShaderInputPBBN
{
	float3 Position : POSITION;
	uint4 BlendIndices : BLENDINDICES0;
    float4 BlendWeight : BLENDWEIGHT0;
	float3 Normal : NORMAL;	
};

struct VertexShaderInputPBBNT
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;	
	float2 TextureCoordinates0 : TEXCOORD0;
	uint4 BlendIndices : BLENDINDICES0;
    float4 BlendWeight : BLENDWEIGHT0;	
};

struct VertexShaderInputPBBNTTT
{
	float4 Position : POSITION0;
	float3 Normal : NORMAL0;	
	float2 TexCoord0 : TEXCOORD0;
	float2 TexCoord1 : TEXCOORD1;
	float2 TexCoord2 : TEXCOORD2;
	uint4 BlendIndices : BLENDINDICES0;
    float4 BlendWeight : BLENDWEIGHT0;	
};

struct VertexShaderInputPNTTB
{
	float3 Position : POSITION;
	float3 Normal : NORMAL0;	
	float2 TextureCoordinates0 : TEXCOORD0;
    float3 Tangent : TANGENT;
	float3 Binormal : BINORMAL;	
};

struct VertexShaderInputPTNTB
{
	float3 Position : POSITION;
	float2 TextureCoordinates0 : TEXCOORD0;
	float3 Normal : NORMAL0;	
    float3 Tangent : TANGENT;
	float3 Binormal : BINORMAL;	
};

struct VertexShaderOutputPNTWTB
{
	float4 Position : SV_POSITION;
	float3 Normal : TEXCOORD0;	
	float2 TextureCoordinates0 : TEXCOORD1;
    float4 WorldPosition : TEXCOORD2;
	float3 Tangent : TANGENT;
	float3 Binormal : BINORMAL;	
};

struct VertexShaderOutputPNTWTBD
{
	float4 Position : SV_POSITION;
	float3 Normal : TEXCOORD0;	
	float2 TextureCoordinates0 : TEXCOORD1;
    float4 WorldPosition : TEXCOORD2;
	float3 Tangent : TANGENT;
	float3 Binormal : BINORMAL;	
	float2 Depth : COLOR;
};

struct VertexShaderInputPBBNTTB
{
	float3 Position : POSITION0;
	float3 Normal : NORMAL0;	
	float2 TextureCoordinates0 : TEXCOORD0;
	float3 Tangent : TANGENT;
	float3 Binormal : BINORMAL;	
	uint4 BlendIndices : BLENDINDICES0;
    float4 BlendWeight : BLENDWEIGHT0;	
};

struct VertexShaderInputPNTTT
{
	float3 Position : POSITION0;
	float3 Normal : NORMAL0;	
	float4 TextureCoordinates0 : TEXCOORD0;
	float4 TextureCoordinates1 : TEXCOORD1;
	float4 TextureCoordinates2 : TEXCOORD2;		
};