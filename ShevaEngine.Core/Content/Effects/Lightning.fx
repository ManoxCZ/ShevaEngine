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

	return samples /5.0f;    
}

float GetShadowContribution(float4 worldPosition, float3 normal, matrix lightViewProj, float3 lightDirection,float2 lightShadowMapSize, sampler2D shadowTextureSampler)
{
	float4 lightingPosition = mul(worldPosition, lightViewProj);
    // Find the position in the shadow map for this pixel
    float2 ShadowTexCoord = mad(0.5f , lightingPosition.xy / lightingPosition.w , float2(0.5f, 0.5f));
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
	float3 result = float3(1,1,1);

	for (int i = 0; i < LightsCount; i++)
		result *= ComputeDiffuseLighting(normal, LightTypes[i], LightPositions[i], LightColors[i].rgb);

	return result;	
}

float3 ComputeDiffuseLighting(float3 normal, float4 worldPosition)
{
	float3 result = float3(1,1,1);

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
	float3 result = float3(0,0,0);

	for (int i = 0; i < LightsCount; i++)
	 	result += min(LightColors[i].a, 1) * ComputeSpecularLighting(normal, LightPositions[i], LightColors[i].rgb, viewDirection, LightColors[i].a);

	return result;		
}

float3 ComputeSpecularLighting(float3 normal, float3 viewDirection, float4 worldPosition)
{
	float3 result = float3(0,0,0);

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



