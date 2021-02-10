float3 AmbientLight = float3(0.20, 0.20, 0.20);
float DepthBias = 0.05;

#define MAX_LIGHTS 4
int LightsCount;
float LightTypes[MAX_LIGHTS];
float3 LightPositions[MAX_LIGHTS];
float4 LightColors[MAX_LIGHTS];
matrix LightViewProjs[MAX_LIGHTS];
float2 LightShadowMapSizes[MAX_LIGHTS];
Texture2D Light1ShadowMap;
sampler2D Light1ShadowMapSampler = sampler_state
{
 	Texture = <Light1ShadowMap>;
	MinFilter = point;
    MagFilter = point;
    MipFilter = point;
    AddressU = clamp;
    AddressV = clamp;
};




// Calculates the shadow term using PCF
float CalcShadowTermPCF(float light_space_depth, float ndotl, float2 shadow_coord, float2 lightShadowMapSize, sampler2D shadowTextureSampler)
{
   float shadow_term = 0;

    //float2 v_lerps = frac(ShadowMapSize * shadow_coord);

    float variableBias =clamp(0.0005 * tan(acos(ndotl)), 0.00001, DepthBias);

    //safe to assume it's a square
    float size = 1.0f / lightShadowMapSize.x;
    	
    float samples[5];
    samples[0] = (light_space_depth - variableBias < 1 - tex2D(shadowTextureSampler, shadow_coord).r);
    samples[1] = (light_space_depth - variableBias < 1 - tex2D(shadowTextureSampler, shadow_coord + float2(size, 0)).r) * frac(shadow_coord.x * lightShadowMapSize.x);
    samples[2] = (light_space_depth - variableBias < 1 - tex2D(shadowTextureSampler, shadow_coord + float2(0, size)).r) * frac(shadow_coord.y * lightShadowMapSize.x);
    samples[3] = (light_space_depth - variableBias < 1 - tex2D(shadowTextureSampler, shadow_coord - float2(size, 0)).r) * (1-frac(shadow_coord.x * lightShadowMapSize.x));
    samples[4] = (light_space_depth - variableBias < 1 - tex2D(shadowTextureSampler, shadow_coord - float2(0, size)).r) * (1 - frac(shadow_coord.y * lightShadowMapSize.x));


    shadow_term = (samples[0] + samples[1] + samples[2] + samples[3] + samples[4]) / 5.0;
    //shadow_term = lerp(lerp(samples[0],samples[1],v_lerps.x),lerp(samples[2],samples[3],v_lerps.x),v_lerps.y);

    return 1 - shadow_term;
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



