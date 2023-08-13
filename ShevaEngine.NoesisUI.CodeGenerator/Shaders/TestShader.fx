#include "Defines.fxh"

#include "ShaderVS.hlsl"
#include "ShaderPS.hlsl"


technique BasicColorDrawing
{
	pass P0
	{
		VertexShader = compile VS_SHADERMODEL MainVS();
		PixelShader = compile PS_SHADERMODEL MainPS();
	}
};