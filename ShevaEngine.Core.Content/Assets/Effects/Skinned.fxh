#define SKINNED_EFFECT_MAX_BONES   72
float4x3 Bones[SKINNED_EFFECT_MAX_BONES];

void Skin(inout VertexShaderInputPBBNT vin, uniform int boneCount)
{
    float4x3 skinning = 0;

    [unroll]
    for (int i = 0; i < boneCount; i++)
    {
        skinning += Bones[vin.BlendIndices[i]] * vin.BlendWeight[i];
    }

    vin.Position.xyz = mul(vin.Position, skinning);
    vin.Normal = mul(vin.Normal, (float3x3)skinning);
}

void Skin(inout VertexShaderInputPBBNTTT vin, uniform int boneCount)
{
    float4x3 skinning = 0;

    [unroll]
    for (int i = 0; i < boneCount; i++)
    {
        skinning += Bones[vin.BlendIndices[i]] * vin.BlendWeight[i];
    }

    vin.Position.xyz = mul(vin.Position, skinning);
    vin.Normal = mul(vin.Normal, (float3x3)skinning);
}

void Skin(inout VertexShaderInputPBBNTTB vin, uniform int boneCount)
{
    float4x3 skinning = 0;

    [unroll]
    for (int i = 0; i < boneCount; i++)
    {
        skinning += Bones[vin.BlendIndices[i]] * vin.BlendWeight[i];
    }

    vin.Position.xyz = mul(vin.Position, skinning);
    vin.Normal = mul(vin.Normal, (float3x3)skinning);
    vin.Tangent = mul(vin.Tangent, (float3x3)skinning);
    vin.Binormal = mul(vin.Binormal, (float3x3)skinning);
}