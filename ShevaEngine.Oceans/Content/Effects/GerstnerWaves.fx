#define MAX_WAVES	12

int GerstnerWavesCount = 0;
float4 Waves_K_W_Dir[MAX_WAVES];
float4 Waves_Phase_Amp[MAX_WAVES];
float Pinch;

void GerstnerWaves(out float3 ret, out float3 N, in float2 pos, float time)	
{	
	ret = float3(0.0, 0.0, 0.0);
	N = float3(0.0, 0.0, 0.0);

	[unroll]
	for (int i = 0; i < GerstnerWavesCount; ++i) 
	{
		float K = Waves_K_W_Dir[i].x;
		float W = Waves_K_W_Dir[i].y;
		float2 dir = Waves_K_W_Dir[i].zw;
		float phase =  Waves_Phase_Amp[i].x;
		float amp =  Waves_Phase_Amp[i].y;

		float wl = (3.14159 / K);																
	//	float killer = pow(min(wl * invSpacing, 1.0), moireReduction);								
//		killed += 1.0-killer;																		
		//float amp = amp;// * killer;																
																								
		float p = K * dot(dir, pos) - time * W + phase;							
		float cosine = cos(p);																		
		float2 d = dir * sin(p) * Pinch;
																								
		ret.xy -= d * amp;																			
		ret.z += amp * cosine; 
		float3 n = normalize(float3(d.x, d.y, cosine) * K) * amp;
		n.z = 1.0 - n.z;																			
		N += n;																						
	}

	N = normalize(N);
}
