#if OPENGL
	#define SV_POSITION POSITION
	#define VS_SHADERMODEL vs_3_0
	#define PS_SHADERMODEL ps_3_0
#else
	#define VS_SHADERMODEL vs_5_0  
	#define PS_SHADERMODEL ps_5_0  
#endif


#define SV_TARGET0 COLOR0
#define SV_TARGET1 COLOR1

