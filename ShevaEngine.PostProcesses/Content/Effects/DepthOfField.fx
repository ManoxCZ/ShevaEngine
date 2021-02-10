#include "PPVertexShader.fxh"
// Shader for creating a Depth of field postprocess.
// Also includes a technique for rendering a depth
// values of the scene.

// Depth buffer resolve parameters
// x = focus distance
// y = focus range
// z = near clip
// w = far clip / ( far clip - near clip )
float4 DoFParams;

Texture2D SpriteTexture;

sampler2D SpriteTextureSampler = sampler_state
{
	Texture = <SpriteTexture>;
};


Texture2D DepthTexture;

sampler2D DepthTextureSampler = sampler_state
{
	Texture = <DepthTexture>;
};

// Textures
sampler2D SceneSampler = sampler_state
{
	Texture = <SceneTex>;
	MinFilter = LINEAR;
	MagFilter = LINEAR;
	MipFilter = LINEAR;
};
sampler2D SceneBlurSampler : register(s0);


///Drawing the depth of field
float4 DepthOfFieldPS_R32F(VertexShaderOutput input) : COLOR0
{
	float4 vSceneBlurSampler = tex2D(SceneBlurSampler, input.TexCoord);
    float4 vSceneSampler      = tex2D( SpriteTextureSampler, input.TexCoord);
	float vDepthTexel = 1 - tex2D(DepthTextureSampler, input.TexCoord);
    
    
    // This is only required if using a floating-point depth buffer format
	//fDepth = 1.0 - fDepth;
	float fDepth = vDepthTexel;

    // Back-transform depth into camera space
    float fSceneZ = ( -DoFParams.z * DoFParams.w ) / ( fDepth - DoFParams.w );
    
    // Compute blur factor
    float fBlurFactor = 0;

	if(fSceneZ <= DoFParams.x)
		fBlurFactor = saturate( ( DoFParams.x - fSceneZ ) / DoFParams.y );
	else
	fBlurFactor = saturate( ( fSceneZ - DoFParams.x ) / DoFParams.y );
    
	// Compute resultant pixel
    float4 color;
    color.rgb = lerp( vSceneSampler.rgb, vSceneBlurSampler.rgb, fBlurFactor );
    color.a	= 1.0;

    return color;
}

technique DepthOfField_R32F
{
    pass P0
    {
        PixelShader = compile PS_SHADERMODEL DepthOfFieldPS_R32F();
    }
}