#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0

matrix ViewMatrix;
matrix ProjMatrix;
float3 CameraPosition;
float GameTime;

float4 Color;

struct OutputWithDepth
{
    float4 RenderTarget1 : SV_Target0;
    float4 RenderTarget2 : SV_Target1;
};
