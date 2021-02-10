#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_5_0  
	#define PS_SHADERMODEL ps_5_0  
#endif


matrix ViewMatrix;
matrix ProjMatrix;
float3 CameraPosition;
float GameTime;

float4 Color;

struct OutputWithDepth
{
	float4 RenderTarget1 : COLOR0;
	float4 RenderTarget2 : COLOR1;
};