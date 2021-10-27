#define PI 3.1415926535897932384626433832795f
#define GAMMA 2.2f

float3 AmbientLight = float3(0.5, 0.5, 0.5);
float DepthBias = 0.05;

#define MAX_LIGHTS 4
int LightsCount;
float LightTypes[MAX_LIGHTS];
float3 LightPositions[MAX_LIGHTS];
float4 LightColors[MAX_LIGHTS];
matrix LightViewProjs[MAX_LIGHTS];
float2 LightShadowMapSizes[MAX_LIGHTS];

Texture2D<float4> Light1ShadowMap : register(t0);
sampler Light1ShadowMapSampler : register(s0)
{
	Texture = <Light1ShadowMap>;
	MinFilter = linear;
	MagFilter = linear;
	MipFilter = linear;
	AddressU = clamp;
	AddressV = clamp;
};




// Calculates the shadow term using PCF
float CalcShadowTermPCF(float light_space_depth, float ndotl, float2 shadow_coord, float2 lightShadowMapSize, sampler2D shadowTextureSampler)
{
	float variableBias = clamp(0.0005 * tan(acos(ndotl)), 0.00001, DepthBias);

	//safe to assume it's a square
	float size = 1.0f / lightShadowMapSize.x;

	float samples = 0;
	if ((light_space_depth - variableBias) < tex2D(shadowTextureSampler, shadow_coord).r)
		samples += 1;

	if ((light_space_depth - variableBias) < tex2D(shadowTextureSampler, shadow_coord + float2(size, 0)).r)
		samples += 1;

	if ((light_space_depth - variableBias) < tex2D(shadowTextureSampler, shadow_coord + float2(0, size)).r)
		samples += 1;

	if ((light_space_depth - variableBias) < tex2D(shadowTextureSampler, shadow_coord - float2(size, 0)).r)
		samples += 1;

	if ((light_space_depth - variableBias) < tex2D(shadowTextureSampler, shadow_coord - float2(0, size)).r)
		samples += 1;

	return samples / 5.0f;
}

float GetShadowContribution(float4 worldPosition, float3 normal, matrix lightViewProj, float3 lightDirection, float2 lightShadowMapSize, sampler2D shadowTextureSampler)
{
	float4 lightingPosition = mul(worldPosition, lightViewProj);
	// Find the position in the shadow map for this pixel
	float2 ShadowTexCoord = mad(0.5f, lightingPosition.xy / lightingPosition.w, float2(0.5f, 0.5f));
	ShadowTexCoord.y = 1.0f - ShadowTexCoord.y;

	// Get the current depth stored in the shadow map
	float ourdepth = (lightingPosition.z / lightingPosition.w);

	return CalcShadowTermPCF(ourdepth, dot(normal, -lightDirection), ShadowTexCoord, lightShadowMapSize, shadowTextureSampler);
}


float3 ComputeDiffuseLighting(float3 normal, float lightType, float3 lightDirection, float3 lightColor)
{
	return lightColor * saturate(dot(normal, -lightDirection));
}

float3 ComputeDiffuseLighting(float3 normal)
{
	float3 result = float3(1, 1, 1);

	for (int i = 0; i < LightsCount; i++)
		result *= ComputeDiffuseLighting(normal, LightTypes[i], LightPositions[i], LightColors[i].rgb);

	return result;
}

float3 ComputeDiffuseLighting(float3 normal, float4 worldPosition)
{
	float3 result = float3(1, 1, 1);

	for (int i = 0; i < LightsCount; i++)
	{
		int lightType = (int)LightTypes[i];

		float shadowContribution = 1;

		if (lightType == 1)
		{
			shadowContribution =
				GetShadowContribution(worldPosition, normal, LightViewProjs[i], LightPositions[i], LightShadowMapSizes[i], Light1ShadowMapSampler);
		}

		result *= ComputeDiffuseLighting(normal, LightTypes[i], LightPositions[i], LightColors[i].rgb) * shadowContribution;
	}

	return result;
}

float ComputeShadowContribution(float3 normal, float4 worldPosition)
{
	float shadowContribution = 1;

	for (int i = 0; i < LightsCount; i++)
	{
		int lightType = (int)LightTypes[i];

		if (lightType == 1)
		{
			shadowContribution *= GetShadowContribution(worldPosition, normal, LightViewProjs[i], LightPositions[i], LightShadowMapSizes[i], Light1ShadowMapSampler);
		}
	}

	return shadowContribution;
}

float3 ComputeSpecularLighting(float3 normal, float3 lightDirection, float3 lightColor, float3 viewDirection, float power)
{
	float3 refl = reflect(lightDirection, normal);

	return (pow(saturate(dot(refl, viewDirection)), power)) * lightColor.rgb;
}

float3 ComputeSpecularLighting(float3 normal, float3 viewDirection)
{
	float3 result = float3(0, 0, 0);

	for (int i = 0; i < LightsCount; i++)
		result += min(LightColors[i].a, 1) * ComputeSpecularLighting(normal, LightPositions[i], LightColors[i].rgb, viewDirection, LightColors[i].a);

	return result;
}

float3 ComputeSpecularLighting(float3 normal, float3 viewDirection, float4 worldPosition)
{
	float3 result = float3(0, 0, 0);

	for (int i = 0; i < LightsCount; i++)
	{
		int lightType = (int)LightTypes[i];

		float shadowContribution = 1;

		if (lightType == 1)
		{
			shadowContribution =
				GetShadowContribution(worldPosition, normal, LightViewProjs[i], LightPositions[i], LightShadowMapSizes[i], Light1ShadowMapSampler);
		}

		result += min(LightColors[i].a, 1) * ComputeSpecularLighting(normal, LightPositions[i], LightColors[i].rgb, viewDirection, LightColors[i].a) * shadowContribution;
	}

	return result;
}

float DistributionGGX(float3 N, float3 H, float roughness)
{
	float a = roughness * roughness;
	float a2 = a * a;
	float NdotH = max(dot(N, H), 0.0);
	float NdotH2 = NdotH * NdotH;

	float nom = a2;
	float denom = (NdotH2 * (a2 - 1.0) + 1.0);
	denom = PI * denom * denom;

	return nom / denom;
}

float GeometrySchlickGGX(float NdotV, float roughness)
{
	float r = (roughness + 1.0);
	float k = (r * r) / 8.0;

	float nom = NdotV;
	float denom = NdotV * (1.0 - k) + k;

	return nom / denom;
}

float GeometrySmith(float3 N, float3 V, float3 L, float roughness)
{
	float NdotV = max(dot(N, V), 0.0);
	float NdotL = max(dot(N, L), 0.0);
	float ggx2 = GeometrySchlickGGX(NdotV, roughness);
	float ggx1 = GeometrySchlickGGX(NdotL, roughness);

	return ggx1 * ggx2;
}

float3 fresnelSchlick(float cosTheta, float3 F0)
{
	return F0 + (1.0 - F0) * pow(max(1.0 - cosTheta, 0.0), 5.0);
}

float3 fresnelSchlickRoughness(float cosTheta, float3 F0, float roughness)
{
	float r = 1.0 - roughness;
	return F0 + (max(float3(r,r,r), F0) - F0) * pow(max(1.0 - cosTheta, 0.0), 5.0);
}

float3 getNormalFromMap(sampler2D NormalMap, float3x3 TBN, float2 texCoords)
{
	float3 tangentNormal = tex2D(NormalMap, texCoords).xyz;
	tangentNormal = normalize(tangentNormal * 2 - 1);

	return mul(tangentNormal, TBN);
}