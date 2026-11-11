// ShevaEngine.Renderer - PBR Forward Renderer Effect
// Physically Based Rendering with Image-Based Lighting support

#include "PBRDefines.fxh"

// ============================================================================
// TEXTURES AND SAMPLERS
// ============================================================================

// Material textures
Texture2D<float4> AlbedoTexture : register(t0);
SamplerState AlbedoSampler : register(s0);

Texture2D<float4> MetallicTexture : register(t1);
Texture2D<float4> RoughnessTexture : register(t2);
SamplerState MetallicRoughnessSampler : register(s1);

Texture2D<float4> NormalTexture : register(t3);
SamplerState NormalSampler : register(s2);

Texture2D<float4> AOTexture : register(t4);
SamplerState AOSampler : register(s3);

Texture2D<float4> EmissiveTexture : register(t5);
SamplerState EmissiveSampler : register(s4);

Texture2D<float4> HeightTexture : register(t6);
SamplerState HeightSampler : register(s5);

// Environment textures (IBL)
TextureCube<float4> EnviroTexture : register(t7);
SamplerState EnviroSampler : register(s6);

TextureCube<float4> IrradianceTexture : register(t8);
SamplerState IrradianceSampler : register(s7);

Texture2D<float4> BrdfLutTexture : register(t9);
SamplerState BrdfLutSampler : register(s8);

// Shadow map
Texture2D<float> Light1ShadowMap : register(t10);
SamplerState Light1ShadowMapSampler : register(s9);
SamplerComparisonState ShadowComparisonSampler : register(s10);

// ============================================================================
// MATERIAL PARAMETERS
// ============================================================================

// Albedo
float4 AlbedoColor = float4(1.0, 1.0, 1.0, 1.0);
bool UseAlbedoTexture = true;

// Metallic & Roughness
float UniformMetallic = 0.0;
float UniformRoughness = 0.5;
bool UseMetallicTexture = false;
bool UseRoughnessTexture = false;

// Normal mapping
float NormalScale = 1.0;
bool UseNormalTexture = true;

// Ambient Occlusion
float AOValue = 1.0;
bool UseAOTexture = false;

// Emissive
float3 EmissiveColor = float3(0.0, 0.0, 0.0);
float EmissiveIntensity = 1.0;
bool UseEmissiveTexture = false;

// Height mapping
float HeightScale = 0.05;
bool UseHeightTexture = false;

// Environment
float3 EnvRotation = float3(0.0, 0.0, 0.0);

// ============================================================================
// LIGHTING PARAMETERS
// ============================================================================

#define MAX_LIGHTS 4

int LightsCount = 0;
float LightTypes[MAX_LIGHTS];
float3 LightPositions[MAX_LIGHTS];
float4 LightColors[MAX_LIGHTS]; // RGB = color, A = intensity
matrix LightViewProjs[MAX_LIGHTS];
float2 LightShadowMapSizes[MAX_LIGHTS];

float DepthBias = 0.05;

// ============================================================================
// CAMERA AND MATRIX PARAMETERS
// ============================================================================

matrix ViewMatrix;
matrix ProjMatrix;
matrix InvViewMatrix;
matrix InvProjMatrix;
float3 CameraPosition;
float GameTime;

// ============================================================================
// SCREEN AND POST-PROCESSING PARAMETERS
// ============================================================================

float2 ScreenResolution;
float Exposure = 1.0;
float Gamma = 2.2;

// ============================================================================
// VERTEX SHADER INPUT/OUTPUT STRUCTURES
// ============================================================================

struct VertexShaderInputPNTTB
{
    float3 Position : POSITION;
    float3 Normal : NORMAL;
    float2 TextureCoordinates0 : TEXCOORD0;
    float3 Tangent : TANGENT;
    float3 Binormal : BINORMAL;
};

struct VertexShaderInputPN
{
    float3 Position : POSITION;
    float3 Normal : NORMAL;
};

struct VertexShaderInputPT
{
    float3 Position : POSITION;
    float2 TextureCoordinates0 : TEXCOORD0;
};

struct VertexShaderOutputPBR
{
    float4 Position : SV_POSITION;
    float3 Normal : TEXCOORD0;
    float3 WorldPosition : TEXCOORD1;
    float2 TextureCoordinates0 : TEXCOORD2;
    float3x3 TBNMatrix : TEXCOORD3;
    float3 ViewDirection : TEXCOORD4;
};

// ============================================================================
// HELPER FUNCTIONS
// ============================================================================

// Constants
#define PI 3.14159265359

// Fresnel Schlick
float3 fresnelSchlick(float cosTheta, float3 F0)
{
    return F0 + (1.0 - F0) * pow(1.0 - cosTheta, 5.0);
}

// Fresnel Schlick with roughness
float3 fresnelSchlickRoughness(float cosTheta, float3 F0, float roughness)
{
    float r = 1.0 - roughness;
    return F0 + (max(float3(r, r, r), F0) - F0) * pow(1.0 - cosTheta, 5.0);
}

// Normal Distribution Function (Trowbridge-Reitz GGX)
float DistributionGGX(float3 N, float3 H, float roughness)
{
    float a = roughness * roughness;
    float a2 = a * a;
    float NdotH = max(dot(N, H), 0.0);
    float NdotH2 = NdotH * NdotH;

    float nom = a2;
    float denom = (NdotH2 * (a2 - 1.0) + 1.0);
    denom = PI * denom * denom;

    return nom / max(denom, 0.0001);
}

// Geometry Function (Schlick GGX)
float GeometrySchlickGGX(float NdotV, float roughness)
{
    float r = (roughness + 1.0);
    float k = (r * r) / 8.0;

    float nom = NdotV;
    float denom = NdotV * (1.0 - k) + k;

    return nom / max(denom, 0.0001);
}

// Smith's method for GGX
float GeometrySmith(float3 N, float3 V, float3 L, float roughness)
{
    float NdotV = max(dot(N, V), 0.0);
    float NdotL = max(dot(N, L), 0.0);
    float ggx2 = GeometrySchlickGGX(NdotV, roughness);
    float ggx1 = GeometrySchlickGGX(NdotL, roughness);

    return ggx1 * ggx2;
}

// Get F0 from albedo and metallic
float3 getF0(float3 albedo, float metallic)
{
    return lerp(float3(0.04, 0.04, 0.04), albedo, metallic);
}

// Sample normal from normal map
float3 getNormalFromMap(SamplerState samplerState, Texture2D normalMap, float3x3 TBN, float2 texCoords)
{
    float3 tangentNormal = normalMap.Sample(samplerState, texCoords).xyz;
    tangentNormal = normalize(tangentNormal * 2.0 - 1.0); // Unpack from [0,1] to [-1,1]
    return mul(tangentNormal, TBN);
}

// Sample normal from map with scale
float3 getScaledNormalFromMap(SamplerState samplerState, Texture2D normalMap, float3x3 TBN, float2 texCoords, float scale)
{
    float3 normal = getNormalFromMap(samplerState, normalMap, TBN, texCoords);
    return normalize(normal * scale + (1.0 - scale) * float3(0, 0, 1));
}

// Tonemapping - ACES Filmic
float3 ToneMapACES(float3 color)
{
    const float A = 2.51;
    const float B = 0.03;
    const float C = 2.43;
    const float D = 0.59;
    const float E = 0.14;
    
    color = (color * (A * color + B)) / (color * (C * color + D) + E);
    return saturate(color);
}

// Tonemapping - Reinhard
float3 ToneMapReinhard(float3 color)
{
    return saturate(color / (1.0 + color));
}

// Tonemapping - Uncharted 2
float3 ToneMapUncharted2(float3 color)
{
    const float A = 0.15;
    const float B = 0.50;
    const float C = 0.10;
    const float D = 0.20;
    const float E = 0.02;
    const float F = 0.30;
    
    return ((color * (A * color + C * B) + D * E) / (color * (A * color + B) + D * F)) - E / F;
}

// Sample environment map with mip levels for specular IBL
float3 SamplePrefilteredEnv(SamplerState samplerState, TextureCube envMap, float3 R, float roughness)
{
    float mipLevel = roughness * 4.0; // Assuming 5 mip levels (0-4)
    return envMap.SampleLevel(samplerState, R, mipLevel).rgb;
}

// Sample split-sum approximation for IBL
float3 SampleSplitSumApprox(SamplerState irradianceSampler, TextureCube irradianceMap,
                              SamplerState prefilterSampler, TextureCube prefilterMap,
                              SamplerState brdfSampler, Texture2D brdfLUT,
                              float3 N, float3 V, float3 R, float roughness)
{
    // Diffuse part (irradiance)
    float3 diffuse = irradianceMap.Sample(irradianceSampler, N).rgb;
    
    // Specular part (prefilter + BRDF)
    float3 specular = SamplePrefilteredEnv(prefilterSampler, prefilterMap, R, roughness);
    float2 brdf = brdfLUT.Sample(brdfSampler, float2(max(dot(N, V), 0.0), roughness)).rg;
    
    float3 kS = fresnelSchlick(max(dot(N, V), 0.0), float3(0.04, 0.04, 0.04));
    
    return diffuse * (1.0 - kS) + specular * (kS * brdf.x + brdf.y);
}

// Calculate shadow term using PCF
float CalcShadowTermPCF(float lightSpaceDepth, float2 shadowCoord, float2 lightShadowMapSize, 
                         SamplerComparisonState shadowSampler, Texture2D shadowTexture)
{
    float variableBias = 0.0005;
    float size = 1.0 / lightShadowMapSize.x;
    
    float samples = 0.0;
    
    if ((lightSpaceDepth - variableBias) < shadowTexture.SampleCmp(shadowSampler, shadowCoord, lightSpaceDepth))
        samples += 1.0;
    
    if ((lightSpaceDepth - variableBias) < shadowTexture.SampleCmp(shadowSampler, shadowCoord + float2(size, 0), lightSpaceDepth))
        samples += 1.0;
    
    if ((lightSpaceDepth - variableBias) < shadowTexture.SampleCmp(shadowSampler, shadowCoord + float2(0, size), lightSpaceDepth))
        samples += 1.0;
    
    if ((lightSpaceDepth - variableBias) < shadowTexture.SampleCmp(shadowSampler, shadowCoord - float2(size, 0), lightSpaceDepth))
        samples += 1.0;
    
    if ((lightSpaceDepth - variableBias) < shadowTexture.SampleCmp(shadowSampler, shadowCoord - float2(0, size), lightSpaceDepth))
        samples += 1.0;
    
    return samples / 5.0;
}

// Get shadow contribution for a light
float GetShadowContribution(float4 worldPosition, float3 normal, matrix lightViewProj, 
                             float3 lightDirection, float2 lightShadowMapSize,
                             SamplerComparisonState shadowSampler, Texture2D shadowTexture)
{
    float4 lightingPosition = mul(worldPosition, lightViewProj);
    
    // Perspective divide
    float2 shadowTexCoord = lightingPosition.xy / lightingPosition.w;
    
    // Transform to [0,1] UV coordinates
    shadowTexCoord = shadowTexCoord * 0.5 + 0.5;
    shadowTexCoord.y = 1.0 - shadowTexCoord.y; // Flip Y
    
    // Get depth in light space
    float lightSpaceDepth = lightingPosition.z / lightingPosition.w;
    
    return CalcShadowTermPCF(lightSpaceDepth, shadowTexCoord, lightShadowMapSize, shadowSampler, shadowTexture);
}

// Calculate PBR lighting for a single light
float3 CalculatePBRLighting(float3 albedo, float metallic, float roughness,
                             float3 normal, float3 viewDir, float3 lightDir,
                             float3 lightColor, float lightIntensity, float3 F0)
{
    float3 halfWay = normalize(viewDir + lightDir);
    float NdotL = max(dot(normal, lightDir), 0.0);
    float NdotV = max(dot(normal, viewDir), 0.0);
    
    if (NdotL <= 0.0)
        return float3(0.0, 0.0, 0.0);
    
    // Cook-Torrance BRDF
    float3 F = fresnelSchlick(max(dot(halfWay, viewDir), 0.0), F0);
    float D = DistributionGGX(normal, halfWay, roughness);
    float G = GeometrySmith(normal, viewDir, lightDir, roughness);
    
    float3 numerator = D * F * G;
    float denominator = 4.0 * max(NdotV, 0.0001) * max(NdotL, 0.0001) + 0.0001;
    float3 specular = numerator / denominator;
    
    // Diffuse (Lambertian with energy conservation)
    float3 kD = (1.0 - F) * (1.0 - metallic);
    float3 diffuse = kD * albedo / PI;
    
    // Combine and scale by light properties
    return (diffuse + specular) * lightColor * lightIntensity * NdotL;
}

// ============================================================================
// VERTEX SHADERS
// ============================================================================

VertexShaderOutputPBR MainVSPBR(VertexShaderInputPNTTB input, matrix transform : COLOR0)
{
    VertexShaderOutputPBR output;
    
    matrix transformTrans = transpose(transform);
    
    // World space position
    float4 worldPosition = mul(float4(input.Position, 1.0), transformTrans);
    output.WorldPosition = worldPosition.xyz;
    
    // View space position
    float4 viewPosition = mul(worldPosition, ViewMatrix);
    output.Position = mul(viewPosition, ProjMatrix);
    
    // Normal in world space
    output.Normal = mul(input.Normal, (float3x3)transformTrans);
    output.Normal = normalize(output.Normal);
    
    // Texture coordinates
    output.TextureCoordinates0 = input.TextureCoordinates0;
    
    // TBN matrix for normal mapping
    float3 tangent = mul(input.Tangent, (float3x3)transformTrans);
    float3 bitangent = mul(input.Binormal, (float3x3)transformTrans);
    output.TBNMatrix = float3x3(normalize(tangent), normalize(bitangent), output.Normal);
    
    // View direction
    output.ViewDirection = normalize(CameraPosition - output.WorldPosition);
    
    return output;
}

VertexShaderOutputPBR MainVSPN(VertexShaderInputPN input, matrix transform : COLOR0)
{
    VertexShaderOutputPBR output;
    
    matrix transformTrans = transpose(transform);
    
    float4 worldPosition = mul(float4(input.Position, 1.0), transformTrans);
    output.WorldPosition = worldPosition.xyz;
    
    float4 viewPosition = mul(worldPosition, ViewMatrix);
    output.Position = mul(viewPosition, ProjMatrix);
    
    output.Normal = mul(input.Normal, (float3x3)transformTrans);
    output.Normal = normalize(output.Normal);
    
    output.TextureCoordinates0 = float2(0, 0);
    
    // Default TBN (no tangent space)
    output.TBNMatrix = float3x3(float3(1,0,0), float3(0,1,0), output.Normal);
    
    output.ViewDirection = normalize(CameraPosition - output.WorldPosition);
    
    return output;
}

VertexShaderOutputPBR MainVSPT(VertexShaderInputPT input, matrix transform : COLOR0)
{
    VertexShaderOutputPBR output;
    
    matrix transformTrans = transpose(transform);
    
    float4 worldPosition = mul(float4(input.Position, 1.0), transformTrans);
    output.WorldPosition = worldPosition.xyz;
    
    float4 viewPosition = mul(worldPosition, ViewMatrix);
    output.Position = mul(viewPosition, ProjMatrix);
    
    // Default normal
    output.Normal = float3(0, 0, 1);
    
    output.TextureCoordinates0 = input.TextureCoordinates0;
    
    // Default TBN
    output.TBNMatrix = float3x3(float3(1,0,0), float3(0,1,0), float3(0,0,1));
    
    output.ViewDirection = normalize(CameraPosition - output.WorldPosition);
    
    return output;
}

// ============================================================================
// PIXEL SHADERS
// ============================================================================

float4 MainPSPBR(VertexShaderOutputPBR input) : SV_Target0
{
    // Sample material properties from textures or use uniform values
    float4 albedoTex = UseAlbedoTexture ? 
        AlbedoTexture.Sample(AlbedoSampler, input.TextureCoordinates0) : AlbedoColor;
    
    float metallic = UseMetallicTexture ? 
        MetallicTexture.Sample(MetallicRoughnessSampler, input.TextureCoordinates0).r : UniformMetallic;
    
    float roughness = UseRoughnessTexture ? 
        RoughnessTexture.Sample(MetallicRoughnessSampler, input.TextureCoordinates0).g : UniformRoughness;
    
    float ao = UseAOTexture ? 
        AOTexture.Sample(AOSampler, input.TextureCoordinates0).r : AOValue;
    
    // Get normal (with normal mapping if enabled)
    float3 normal = input.Normal;
    if (UseNormalTexture)
    {
        normal = getScaledNormalFromMap(NormalSampler, NormalTexture, 
                                         (float3x3)input.TBNMatrix, 
                                         input.TextureCoordinates0, NormalScale);
    }
    
    // Normalize normal in case of scaling
    normal = normalize(normal);
    
    // Calculate view direction
    float3 viewDir = normalize(CameraPosition - input.WorldPosition);
    
    // Get F0 from albedo and metallic
    float3 F0 = getF0(albedoTex.rgb, metallic);
    
    // ========================================================================
    // IMAGE-BASED LIGHTING (IBL)
    // ========================================================================
    
    // Irradiance (diffuse IBL)
    float3 irradiance = IrradianceTexture.Sample(IrradianceSampler, normal).rgb;
    float3 diffuseIBL = irradiance * albedoTex.rgb * (1.0 - metallic) * ao;
    
    // Specular IBL using split-sum approximation
    float3 reflection = reflect(-viewDir, normal);
    
    // Rotate reflection vector by environment rotation
    float3x3 envRotationMatrix = mul(float3x3(cos(EnvRotation.y), 0, -sin(EnvRotation.y)),
                                       float3(0, 1, 0),
                                       float3(sin(EnvRotation.y), 0, cos(EnvRotation.y)));
    reflection = mul(reflection, envRotationMatrix);
    
    // Sample prefiltered environment
    float3 prefilteredColor = SamplePrefilteredEnv(EnviroSampler, EnviroTexture, reflection, roughness);
    
    // Sample BRDF LUT
    float2 brdf = BrdfLutTexture.Sample(BrdfLutSampler, float2(max(dot(normal, viewDir), 0.0), roughness)).rg;
    
    // Fresnel
    float3 kS = fresnelSchlick(max(dot(normal, viewDir), 0.0), F0);
    float3 kD = 1.0 - kS;
    kD *= 1.0 - metallic;
    
    float3 specularIBL = prefilteredColor * (kS * brdf.x + brdf.y);
    
    float3 color = (kD * diffuseIBL + specularIBL) * ao;
    
    // ========================================================================
    // DIRECT LIGHTING (from light sources)
    // ========================================================================
    
    float3 directLighting = float3(0.0, 0.0, 0.0);
    
    for (int i = 0; i < LightsCount; i++)
    {
        int lightType = (int)LightTypes[i];
        float3 lightColor = LightColors[i].rgb;
        float lightIntensity = LightColors[i].a;
        float3 lightPos = LightPositions[i];
        
        float3 lightDir;
        
        if (lightType == 0) // Point light
        {
            lightDir = normalize(lightPos - input.WorldPosition);
        }
        else if (lightType == 1) // Directional light
        {
            lightDir = normalize(lightPos);
        }
        else // Spot light (simplified)
        {
            lightDir = normalize(lightPos - input.WorldPosition);
        }
        
        // Calculate shadow contribution
        float shadow = 1.0;
        if (lightType == 1 && LightsCount > 0) // Directional light with shadows
        {
            shadow = GetShadowContribution(
                float4(input.WorldPosition, 1.0),
                normal,
                LightViewProjs[i],
                lightDir,
                LightShadowMapSizes[i],
                ShadowComparisonSampler,
                Light1ShadowMap);
        }
        
        // Calculate PBR lighting
        directLighting += CalculatePBRLighting(
            albedoTex.rgb,
            metallic,
            roughness,
            normal,
            viewDir,
            -lightDir,
            lightColor,
            lightIntensity,
            F0) * shadow;
    }
    
    color += directLighting;
    
    // ========================================================================
    // EMISIVE
    // ========================================================================
    
    float3 emissive = EmissiveColor * EmissiveIntensity;
    if (UseEmissiveTexture)
    {
        emissive *= EmissiveTexture.Sample(EmissiveSampler, input.TextureCoordinates0).rgb;
    }
    color += emissive;
    
    // ========================================================================
    // AMBIENT LIGHT
    // ========================================================================
    
    // Add ambient light (can be removed if using IBL only)
    // float3 ambient = AmbientLight * albedoTex.rgb * ao;
    // color += ambient;
    
    // ========================================================================
    // POST-PROCESSING
    // ========================================================================
    
    // Apply exposure
    color *= Exposure;
    
    // Tonemapping
    color = ToneMapACES(color);
    
    // Gamma correction
    color = pow(color, float3(1.0 / Gamma, 1.0 / Gamma, 1.0 / Gamma));
    
    return float4(color, albedoTex.a);
}

// Simplified pixel shader for forward rendering without IBL
float4 MainPSPBRForwardOnly(VertexShaderOutputPBR input) : SV_Target0
{
    // Sample material properties
    float4 albedoTex = UseAlbedoTexture ? 
        AlbedoTexture.Sample(AlbedoSampler, input.TextureCoordinates0) : AlbedoColor;
    
    float metallic = UseMetallicTexture ? 
        MetallicTexture.Sample(MetallicRoughnessSampler, input.TextureCoordinates0).r : UniformMetallic;
    
    float roughness = UseRoughnessTexture ? 
        RoughnessTexture.Sample(MetallicRoughnessSampler, input.TextureCoordinates0).g : UniformRoughness;
    
    float ao = UseAOTexture ? 
        AOTexture.Sample(AOSampler, input.TextureCoordinates0).r : AOValue;
    
    // Get normal
    float3 normal = input.Normal;
    if (UseNormalTexture)
    {
        normal = getScaledNormalFromMap(NormalSampler, NormalTexture, 
                                         (float3x3)input.TBNMatrix, 
                                         input.TextureCoordinates0, NormalScale);
    }
    normal = normalize(normal);
    
    float3 viewDir = normalize(CameraPosition - input.WorldPosition);
    float3 F0 = getF0(albedoTex.rgb, metallic);
    
    // Direct lighting only
    float3 color = float3(0.0, 0.0, 0.0);
    
    for (int i = 0; i < LightsCount; i++)
    {
        int lightType = (int)LightTypes[i];
        float3 lightColor = LightColors[i].rgb;
        float lightIntensity = LightColors[i].a;
        float3 lightPos = LightPositions[i];
        
        float3 lightDir;
        if (lightType == 0)
            lightDir = normalize(lightPos - input.WorldPosition);
        else if (lightType == 1)
            lightDir = normalize(lightPos);
        else
            lightDir = normalize(lightPos - input.WorldPosition);
        
        float shadow = 1.0;
        if (lightType == 1 && LightsCount > 0)
        {
            shadow = GetShadowContribution(
                float4(input.WorldPosition, 1.0),
                normal,
                LightViewProjs[i],
                lightDir,
                LightShadowMapSizes[i],
                ShadowComparisonSampler,
                Light1ShadowMap);
        }
        
        color += CalculatePBRLighting(
            albedoTex.rgb,
            metallic,
            roughness,
            normal,
            viewDir,
            -lightDir,
            lightColor,
            lightIntensity,
            F0) * shadow * ao;
    }
    
    // Emissive
    float3 emissive = EmissiveColor * EmissiveIntensity;
    if (UseEmissiveTexture)
        emissive *= EmissiveTexture.Sample(EmissiveSampler, input.TextureCoordinates0).rgb;
    color += emissive;
    
    // Post-processing
    color *= Exposure;
    color = ToneMapACES(color);
    color = pow(color, float3(1.0 / Gamma, 1.0 / Gamma, 1.0 / Gamma));
    
    return float4(color, albedoTex.a);
}

// Shadow pass pixel shader
float4 MainPSShadow(VertexShaderOutputPBR input) : SV_Target0
{
    // Simple depth-only rendering for shadow map
    return float4(0.0, 0.0, 0.0, 1.0);
}

// ============================================================================
// TECHNIQUES
// ============================================================================

technique Default
{
    pass PNTTB_PBR
    {
        VertexShader = compile vs_5_0 MainVSPBR();
        PixelShader = compile ps_5_0 MainPSPBR();
    }

    pass PN_PBR
    {
        VertexShader = compile vs_5_0 MainVSPN();
        PixelShader = compile ps_5_0 MainPSPBR();
    }

    pass PT_PBR
    {
        VertexShader = compile vs_5_0 MainVSPT();
        PixelShader = compile ps_5_0 MainPSPBR();
    }

    pass PNTTB_ForwardOnly
    {
        VertexShader = compile vs_5_0 MainVSPBR();
        PixelShader = compile ps_5_0 MainPSPBRForwardOnly();
    }

    pass PNTTB_Shadow
    {
        VertexShader = compile vs_5_0 MainVSPBR();
        PixelShader = compile ps_5_0 MainPSShadow();
    }
};

technique Shadows
{
    pass PNTTB_Shadow
    {
        VertexShader = compile vs_5_0 MainVSPBR();
        PixelShader = compile ps_5_0 MainPSShadow();
    }
};
