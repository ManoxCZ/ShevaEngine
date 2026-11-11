// PBR Defines Header File
// Shared definitions for PBR effects

#ifndef PBR_DEFINES_FXH
#define PBR_DEFINES_FXH

// Shader model
#define VS_SHADERMODEL vs_5_0
#define PS_SHADERMODEL ps_5_0

// Maximum number of lights
#define MAX_LIGHTS 4

// PI constant
#define PI 3.14159265359

// Light types
#define LIGHT_TYPE_POINT 0
#define LIGHT_TYPE_DIRECTIONAL 1
#define LIGHT_TYPE_SPOT 2

// Default color for dielectric materials (F0 = 0.04)
#define DEFAULT_F0 float3(0.04, 0.04, 0.04)

// Minimum roughness to prevent artifacts
#define MIN_ROUGHNESS 0.0001

#endif // PBR_DEFINES_FXH
