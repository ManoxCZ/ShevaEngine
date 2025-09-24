#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_5_0
	#define PS_SHADERMODEL ps_5_0
#endif

float PI = 3.1415159;
float InnerRadius = 6356.7523142;
float OuterRadius = 6356.7523142 * 1.0157313;
float fScale = 1.0 / (6356.7523142 * 1.0157313 - 6356.7523142);
float2 v2dRayleighMieScaleHeight = { 0.25, 0.1 };
float NumSamples = 20;
float3 v3SunDir = { 0, 1, 0 };
float Kr4PI = 0.0025f * 4.0f * 3.1415159;
float Km4PI = 0.0010f * 4.0f * 3.1415159;
float3 InvWavelength;
float3 WavelengthMie;
float KrESun = 0.0025f * 20.0f;
float KmESun = 0.0010f * 20.0f;

struct VertexShaderInput
{
	float4 Position : POSITION0;
    float2 TexCoord : TEXCOORD0;
};

struct VertexShaderOutput
{
	float4 Position : SV_POSITION;
    float2 Pos : TEXCOORD0;
};

VertexShaderOutput MainVS(in VertexShaderInput input)
{
	VertexShaderOutput output = (VertexShaderOutput)0;

	output.Position = input.Position;
    output.Pos = input.TexCoord;

	return output;
}

float HitOuterSphere(float3 O, float3 Dir)
{
    float3 L = -O;

    float B = dot(L, Dir);
    float C = dot(L, L);
    float D = C - B * B;
    float q = sqrt(OuterRadius * OuterRadius - D);
    float t = B;
    t += q;

    return t;
}

float2 GetDensityRatio(float fHeight)
{
    const float fAltitude = (fHeight - InnerRadius) * fScale;
    return exp(-fAltitude / v2dRayleighMieScaleHeight.xy);
}

float2 t(float3 P, float3 Px)
{
    float2 OpticalDepth = 0;

    float3 v3Vector = Px - P;
    float fFar = length(v3Vector);
    float3 v3Dir = v3Vector / fFar;
			
    float fSampleLength = fFar / NumSamples;
    float fScaledLength = fSampleLength * fScale;
    float3 v3SampleRay = v3Dir * fSampleLength;
    P += v3SampleRay * 0.5f;
			    
    for (int i = 0; i < NumSamples; i++)
    {
        float fHeight = length(P);
        OpticalDepth += GetDensityRatio(fHeight);
        P += v3SampleRay;
    }

    OpticalDepth *= fScaledLength;
    return OpticalDepth;
}

struct PS_OUTPUT_UPDATE
{
    float4 RayLeigh : COLOR0;
    float4 Mie : COLOR1;
};

PS_OUTPUT_UPDATE MainPS(VertexShaderOutput input)
{
    PS_OUTPUT_UPDATE output = (PS_OUTPUT_UPDATE) 0;
	
    float2 Tex0 = (input.Pos);
	 
    const float3 v3PointPv = float3(0, InnerRadius + 1e-3, 0);
    const float AngleY = 100.0 * Tex0.x * PI / 180.0;
    const float AngleXZ = PI * Tex0.y;
	
    float3 v3Dir;
    v3Dir.x = sin(AngleY) * cos(AngleXZ);
    v3Dir.y = cos(AngleY);
    v3Dir.z = sin(AngleY) * sin(AngleXZ);
    v3Dir = normalize(v3Dir);

    float fFarPvPa = HitOuterSphere(v3PointPv, v3Dir);
    float3 v3Ray = v3Dir;

    float3 v3PointP = v3PointPv;
    float fSampleLength = fFarPvPa / NumSamples;
    float fScaledLength = fSampleLength * fScale;
    float3 v3SampleRay = v3Ray * fSampleLength;
    v3PointP += v3SampleRay * 0.5f;
				
    float3 v3RayleighSum = 0;
    float3 v3MieSum = 0;
    
    for (int k = 0; k < NumSamples; k++)
    {
        float PointPHeight = length(v3PointP);

        float2 DensityRatio = GetDensityRatio(PointPHeight);
        DensityRatio *= fScaledLength;

        float2 ViewerOpticalDepth = t(v3PointP, v3PointPv);
						
        float dFarPPc = HitOuterSphere(v3PointP, v3SunDir);
        float2 SunOpticalDepth = t(v3PointP, v3PointP + v3SunDir * dFarPPc);

        float2 OpticalDepthP = SunOpticalDepth.xy + ViewerOpticalDepth.xy;
        float3 v3Attenuation = exp(-Kr4PI * InvWavelength * OpticalDepthP.x - Km4PI * OpticalDepthP.y);

        v3RayleighSum += DensityRatio.x * v3Attenuation;
        v3MieSum += DensityRatio.y * v3Attenuation;

        v3PointP += v3SampleRay;
    }

    float3 RayLeigh = v3RayleighSum * KrESun;
    float3 Mie = v3MieSum * KmESun;
    RayLeigh *= InvWavelength;
    Mie *= WavelengthMie;
	
    output.RayLeigh = float4(RayLeigh, 1);
    output.Mie = float4(Mie, 1);
    return output;
}

technique SkyDomeUpdate
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};