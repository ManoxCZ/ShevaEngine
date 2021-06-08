#include "Defines.fx"
#include "VertexStructures.fx"
#include "Lightning.fx"
#include "Skinned.fx"

bool _useAOTexture = true;
bool _useRoughnessTexture = true;
bool _useMetallicTexture = true;

//Material setup
Texture2D Texture;
Texture2D NormalTexture;
Texture2D RoughnessTexture;
Texture2D MetallicTexture;
Texture2D AOTexture;

float _uniformRoughness;
float _uniformMetallic;

//IBL setup
TextureCube EnviroTexture;
TextureCube IrradianceTexture;
Texture2D BrdfLutTexture;

//Samplers
sampler2D TextureSampler = sampler_state
{
	Texture = <Texture>;
};

sampler2D NormalTextureSampler = sampler_state
{
	Texture = <NormalTexture>;
};

sampler2D RoughnessTextureSampler = sampler_state
{
	Texture = <RoughnessTexture>;
};

sampler2D MetallicTextureSampler = sampler_state 
{
	Texture = <MetallicTexture>;
};

sampler2D AOTextureSampler = sampler_state
{
	Texture = <AOTexture>;
};

samplerCUBE EnviroTextureSampler = sampler_state
{
	Texture = <EnviroTexture>;
};

samplerCUBE IrradianceTextureSampler = sampler_state
{
	Texture = <IrradianceTexture>;
};

sampler2D BrdfLutSampler = sampler_state
{
	Texture = <BrdfLutTexture>;
};

VertexShaderOutputPNTWD MainVSPT(const VertexShaderInputPT input, matrix transform : COLOR0)
{
	VertexShaderOutputPNTWD output;

	matrix transformTrans = transpose(transform);

	output.WorldPosition = mul(float4(input.Position, 1.0), transformTrans);
	output.Position = mul(mul(mul(float4(input.Position, 1.0), transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = float3(0, 1, 0);
	output.TextureCoordinates0 = input.TextureCoordinates0;
	output.Depth = output.Position.zw;

	return output;
}

VertexShaderOutputPNTWD MainVSPNT(const VertexShaderInputPNT input, matrix transform : COLOR0)
{
	VertexShaderOutputPNTWD output;

	matrix transformTrans = transpose(transform);

	output.WorldPosition = mul(float4(input.Position, 1.0), transformTrans);
	output.Position = mul(mul(mul(float4(input.Position, 1.0), transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans);
	output.TextureCoordinates0 = input.TextureCoordinates0;
	output.Depth = output.Position.zw;

	return output;
}

VertexShaderOutputPNTWD MainVSPNTT(const VertexShaderInputPNTT input, matrix transform : COLOR0)
{
	VertexShaderOutputPNTWD output;

	matrix transformTrans = transpose(transform);

	output.WorldPosition = mul(float4(input.Position, 1.0), transformTrans);
	output.Position = mul(mul(mul(float4(input.Position, 1.0), transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans);
	output.TextureCoordinates0 = input.TextureCoordinates0;
	output.Depth = output.Position.zw;

	return output;
}

VertexShaderOutputPNTWD MainVSPNTT(const VertexShaderInputPNT input, matrix transform : COLOR0)
{
	VertexShaderOutputPNTWD output;

	matrix transformTrans = transpose(transform);

	output.WorldPosition = mul(float4(input.Position, 1.0), transformTrans);
	output.Position = mul(mul(mul(float4(input.Position, 1.0), transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans);
	output.TextureCoordinates0 = input.TextureCoordinates0;
	output.Depth = output.Position.zw;

	return output;
}

VertexShaderOutputPNTWTBD MainVSPNTTB(const VertexShaderInputPNTTB input, matrix transform : COLOR0)
{
	VertexShaderOutputPNTWTBD output;

	matrix transformTrans = transpose(transform);

	output.WorldPosition = mul(float4(input.Position, 1.0), transformTrans);
	output.Position = mul(mul(mul(float4(input.Position, 1.0), transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans);
	output.TextureCoordinates0 = input.TextureCoordinates0;
	output.Tangent = mul(input.Tangent, (float3x3)transformTrans);
	output.Binormal = mul(input.Binormal, (float3x3)transformTrans);
	output.Depth = output.Position.zw;

	return output;
}

VertexShaderOutputPNTWD MainVSPBBNT(const VertexShaderInputPBBNT input, matrix transform : COLOR0)
{
	VertexShaderOutputPNTWD output;

	matrix transformTrans = transpose(transform);

	output.WorldPosition = mul(input.Position, transformTrans);
	output.Position = mul(mul(mul(input.Position, transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans);
	output.TextureCoordinates0 = input.TextureCoordinates0;
	output.Depth = output.Position.zw;

	return output;
}

VertexShaderOutputPNTWD MainVSPBBNTAnimated(VertexShaderInputPBBNT input, matrix transform : COLOR0)
{
	VertexShaderOutputPNTWD output;

	matrix transformTrans = transpose(transform);

	Skin(input, 4);

	output.WorldPosition = mul(input.Position, transformTrans);
	output.Position = mul(mul(mul(input.Position, transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans);
	output.TextureCoordinates0 = input.TextureCoordinates0;
	output.Depth = output.Position.zw;

	return output;
}


VertexShaderOutputPNTWTBD MainVSPTNTB(const VertexShaderInputPTNTB input, matrix transform : COLOR0)
{
	VertexShaderOutputPNTWTBD output;

	matrix transformTrans = transpose(transform);

	output.WorldPosition = mul(float4(input.Position, 1.0), transformTrans);
	output.Position = mul(mul(mul(float4(input.Position, 1.0), transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans);
	output.TextureCoordinates0 = input.TextureCoordinates0;
	output.Tangent = mul(input.Tangent, (float3x3)transformTrans);
	output.Binormal = mul(input.Binormal, (float3x3)transformTrans);
	output.Depth = output.Position.zw;

	return output;
}

VertexShaderOutputPNTWTBD MainVSPNeco(const VertexShaderInputPTNTB input, matrix transform : COLOR0)
{
	VertexShaderOutputPNTWTBD output;

	matrix transformTrans = transpose(transform);

	output.WorldPosition = mul(float4(input.Position, 1.0), transformTrans);
	output.Position = mul(mul(mul(float4(input.Position, 1.0), transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans);
	output.TextureCoordinates0 = input.TextureCoordinates0;
	output.Tangent = mul(input.Tangent, (float3x3)transformTrans);
	output.Binormal = mul(input.Binormal, (float3x3)transformTrans);
	output.Depth = output.Position.zw;

	return output;
}

VertexShaderOutputPNTWTBD MainVSPBBNTTBAnimated(VertexShaderInputPBBNTTB input, matrix transform : COLOR0)
{
	VertexShaderOutputPNTWTBD output;

	matrix transformTrans = transpose(transform);

	Skin(input, 4);

	output.WorldPosition = mul(input.Position, transformTrans);
	output.Position = mul(mul(mul(input.Position, transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans);
	output.Depth = output.Position.zw;
	output.Tangent = mul(input.Tangent, (float3x3)transformTrans);
	output.Binormal = mul(input.Binormal, (float3x3)transformTrans);
	output.TextureCoordinates0 = input.TextureCoordinates0;

	return output;
}

VertexShaderOutputPNTWTBD MainVSPNecoAnimated(VertexShaderInputPBBNTTB input, matrix transform : COLOR0)
{
	VertexShaderOutputPNTWTBD output;

	matrix transformTrans = transpose(transform);

	Skin(input, 4);

	output.WorldPosition = mul(float4(input.Position, 1.0), transformTrans);
	output.Position = mul(mul(mul(float4(input.Position, 1.0), transformTrans), ViewMatrix), ProjMatrix);
	output.Normal = mul(input.Normal, (float3x3)transformTrans);
	output.TextureCoordinates0 = input.TextureCoordinates0;
	output.Tangent = mul(input.Tangent, (float3x3)transformTrans);
	output.Binormal = mul(input.Binormal, (float3x3)transformTrans);
	output.Depth = output.Position.zw;

	return output;
}


OutputWithDepth MainPSPNT(VertexShaderOutputPNTWD input)
{
	OutputWithDepth result;

	float3 finalColor;
	float4 texColor = tex2D(TextureSampler, input.TextureCoordinates0);

	clip(texColor.a - 0.5f);

	float ao = 1.0; 
	float metallic = _uniformMetallic;
	float roughness = _uniformRoughness;

	if (_useAOTexture)
	{
		ao = tex2D(AOTextureSampler, input.TextureCoordinates0).r;
	}

	if (_useRoughnessTexture)
	{
		roughness = tex2D(RoughnessTextureSampler, input.TextureCoordinates0).r;
	}

	if (_useMetallicTexture)
	{
		metallic = tex2D(MetallicTextureSampler, input.TextureCoordinates0).r;
	}

	float3 albedo = pow(texColor.rgb, float3(2.2, 2.2, 2.2));

	float3 ambientOcclusion = float3(1, 1, 1) * ao;

	float3 N = normalize(input.Normal);
	float3 V = normalize(CameraPosition - input.WorldPosition.rgb);
	float3 R = normalize(reflect(-V, N));
	float3 L = normalize(-LightPositions[0]);
	float3 H = normalize(V + L);

	float3 F0 = float3(0.04f, 0.04f, 0.04f);
	F0 = lerp(F0, albedo, metallic);

	// reflectance equation
	float3 Lo = float3(0.0, 0.0, 0.0);
	for (int i = 0; i < MAX_LIGHTS; ++i)
	{
		// calculate per-light radiance
		float3 radiance = LightColors[i];

		// Cook-Torrance BRDF
		float NDF = DistributionGGX(N, H, roughness);
		float G = GeometrySmith(N, V, L, roughness);
		float3 F = fresnelSchlick(saturate(dot(H, V)), F0);

		float3 nominator = NDF * G * F;
		// 0.001 to prevent divide by zero.
		float denominator = 4 * saturate(dot(N, V)) * saturate(dot(N, L)) + 0.001;
		float3 specular = nominator / denominator;

		// kS is equal to Fresnel
		float3 kS = F;
		// for energy conservation, the diffuse and specular light can't
		// be above 1.0 (unless the surface emits light); to preserve this
		// relationship the diffuse component (kD) should equal 1.0 - kS.
		float3 kD = float3(1.0, 1.0, 1.0) - kS;
		// multiply kD by the inverse metalness such that only non-metals 
		// have diffuse lighting, or a linear blend if partly metal (pure metals
		// have no diffuse light).
		kD *= 1.0 - metallic;

		// scale light by NdotL
		float NdotL = saturate(dot(N, L));

		// add to outgoing radiance Lo
		Lo += (kD * albedo / PI + specular) * radiance * NdotL; // note that we already multiplied the BRDF by the Fresnel (kS) so we won't multiply by kS again
	}

	// ambient lighting (we now use IBL as the ambient term)
	float3 F = fresnelSchlickRoughness(saturate(dot(N, V)), F0, roughness);

	float3 kS = F;
	float3 kD = 1.0 - kS;
	kD *= 1.0 - metallic;

	float3 irradiance = texCUBE(IrradianceTextureSampler, N).rgb;
	float3 diffuse = irradiance * albedo;

	// sample both the pre-filter map and the BRDF lut and combine them together as per the Split-Sum approximation to get the IBL specular part.
	const float MAX_REFLECTION_LOD = 6.0;
	float3 prefilteredColor = texCUBElod(EnviroTextureSampler, float4(R, roughness * MAX_REFLECTION_LOD)).rgb;
	float2 brdf = tex2D(BrdfLutSampler, float2(saturate(dot(N, V)), roughness)).rg;
	float3 specular = prefilteredColor * (F * brdf.x + brdf.y);

	float3 ambient = (kD * diffuse + specular) * ambientOcclusion;

	float3 color = ambient + Lo;

	// HDR tonemapping
	color = color / (color + float3(1.0, 1.0, 1.0));
	// gamma correct
	color = pow(color, float3(1.0 / 2.2, 1.0 / 2.2, 1.0 / 2.2));

	result.RenderTarget1 = float4(color, 1.0);

	result.RenderTarget2 = float4(input.Depth.x, 0.0f, 0.0f, 0.0f);

	return result;
}

OutputWithDepth MainPSPNTWTB(VertexShaderOutputPNTWTBD input)
{
	OutputWithDepth result;

	float3 finalColor;
	float4 texColor = tex2D(TextureSampler, input.TextureCoordinates0);
	float3 albedo = pow(texColor.rgb, float3(2.2, 2.2, 2.2));
	
	clip(texColor.a - 0.5f);

	float ao = 1.0, metallic, roughness;

	if (_useAOTexture)
	{
		ao = tex2D(AOTextureSampler, input.TextureCoordinates0).r;
	}

	if (_useRoughnessTexture)
	{
		roughness = tex2D(RoughnessTextureSampler, input.TextureCoordinates0).r;
	}

	if (_useMetallicTexture)
	{
		metallic = tex2D(MetallicTextureSampler, input.TextureCoordinates0).r;
	}

	float3x3 TBN = float3x3(normalize(input.Tangent), 
		normalize(input.Binormal), normalize(input.Normal));

	float3 N = getNormalFromMap(NormalTextureSampler, TBN, input.TextureCoordinates0);
	float3 V = normalize(CameraPosition - input.WorldPosition.rgb);
	float3 R = normalize(reflect(-V, N));
	float3 L = normalize(-LightPositions[0]);
	float3 H = normalize(V + L);

	float3 F0 = float3(0.04f, 0.04f, 0.04f);
	F0 = lerp(F0, albedo, metallic);

	float3 lightColor = float3(1.0, 1.0, 1.0);

	// reflectance equation
	float3 Lo = float3(0.0, 0.0, 0.0);
	for (int i = 0; i < MAX_LIGHTS; ++i)
	{
		// calculate per-light radiance
		
		//no need to calculate attenuation for outdoor directional light
		//float distance = length(LightPositions[i] - input.WorldPosition);
		//float attenuation = 1.0 / (distance * distance);
		float3 radiance = LightColors[i];

		// Cook-Torrance BRDF
		float NDF = DistributionGGX(N, H, roughness);
		float G = GeometrySmith(N, V, L, roughness);
		float3 F = fresnelSchlick(saturate(dot(H, V)), F0);

		float3 nominator = NDF * G * F;
		// 0.001 to prevent divide by zero.
		float denominator = 4 * saturate(dot(N, V)) * saturate(dot(N, L)) + 0.001;
		float3 specular = nominator / denominator;

		// kS is equal to Fresnel
		float3 kS = F;
		// for energy conservation, the diffuse and specular light can't
		// be above 1.0 (unless the surface emits light); to preserve this
		// relationship the diffuse component (kD) should equal 1.0 - kS.
		float3 kD = float3(1.0, 1.0, 1.0) - kS;
		// multiply kD by the inverse metalness such that only non-metals 
		// have diffuse lighting, or a linear blend if partly metal (pure metals
		// have no diffuse light).
		kD *= 1.0 - metallic;

		// scale light by NdotL
		float NdotL = saturate(dot(N, L));

		// add to outgoing radiance Lo
		Lo += (kD * albedo / PI + specular) * radiance * NdotL; // note that we already multiplied the BRDF by the Fresnel (kS) so we won't multiply by kS again
	}

	// ambient lighting (we now use IBL as the ambient term)
	float3 F = fresnelSchlickRoughness(saturate(dot(N, V)), F0, roughness);

	float3 kS = F;
	float3 kD = 1.0 - kS;
	kD *= 1.0 - metallic;

	float3 irradiance = texCUBE(IrradianceTextureSampler, N).rgb;
	float3 diffuse = irradiance * albedo;

	// sample both the pre-filter map and the BRDF lut and combine them together as per the Split-Sum approximation to get the IBL specular part.
	const float MAX_REFLECTION_LOD = 6.0;
	float3 prefilteredColor = texCUBElod(EnviroTextureSampler, float4(R, roughness * MAX_REFLECTION_LOD)).rgb;
	float2 brdf = tex2D(BrdfLutSampler, float2(saturate(dot(N, V)), roughness)).rg;
	float3 specular = prefilteredColor * (F * brdf.x + brdf.y);

	float3 ambient = (kD * diffuse + specular) * ao;

	float3 color = ambient + Lo;

	// HDR tonemapping
	color = color / (color + float3(1.0, 1.0, 1.0));
	// gamma correct
	color = pow(color, float3(1.0 / 2.2, 1.0 / 2.2, 1.0 / 2.2));

	result.RenderTarget1 = float4(color, texColor.w);

	result.RenderTarget2 = float4(input.Depth.x, 0.0f, 0.0f, 0.0f);

	return result;
}

OutputWithDepth MainPSPNeco(VertexShaderOutputPNTWTBD input)
{
	OutputWithDepth result;

	float3 finalColor;
	float4 texColor = tex2D(TextureSampler, input.TextureCoordinates0);
	float3 albedo = pow(texColor.rgb, float3(2.2, 2.2, 2.2));

	clip(texColor.a - 0.5f);

	float ao = 1.0, metallic, roughness;

	if (_useAOTexture)
	{
		ao = tex2D(AOTextureSampler, input.TextureCoordinates0).r;
	}

	if (_useRoughnessTexture)
	{
		roughness = tex2D(RoughnessTextureSampler, input.TextureCoordinates0).r;
	}

	if (_useMetallicTexture)
	{
		metallic = tex2D(MetallicTextureSampler, input.TextureCoordinates0).r;
	}

	float3x3 TBN = float3x3(normalize(input.Tangent),
		normalize(input.Binormal), normalize(input.Normal));

	float3 N = getNormalFromMap(NormalTextureSampler, TBN, input.TextureCoordinates0);
	float3 V = normalize(CameraPosition - input.WorldPosition.rgb);
	float3 R = normalize(reflect(-V, N));
	float3 L = normalize(-LightPositions[0]);
	float3 H = normalize(V + L);

	float3 F0 = float3(0.04f, 0.04f, 0.04f);
	F0 = lerp(F0, albedo, metallic);

	float3 lightColor = float3(1.0, 1.0, 1.0);

	// reflectance equation
	float3 Lo = float3(0.0, 0.0, 0.0);
	for (int i = 0; i < MAX_LIGHTS; ++i)
	{
		// calculate per-light radiance

		//no need to calculate attenuation for outdoor directional light
		//float distance = length(LightPositions[i] - input.WorldPosition);
		//float attenuation = 1.0 / (distance * distance);
		float3 radiance = LightColors[i];

		// Cook-Torrance BRDF
		float NDF = DistributionGGX(N, H, roughness);
		float G = GeometrySmith(N, V, L, roughness);
		float3 F = fresnelSchlick(saturate(dot(H, V)), F0);

		float3 nominator = NDF * G * F;
		// 0.001 to prevent divide by zero.
		float denominator = 4 * saturate(dot(N, V)) * saturate(dot(N, L)) + 0.001;
		float3 specular = nominator / denominator;

		// kS is equal to Fresnel
		float3 kS = F;
		// for energy conservation, the diffuse and specular light can't
		// be above 1.0 (unless the surface emits light); to preserve this
		// relationship the diffuse component (kD) should equal 1.0 - kS.
		float3 kD = float3(1.0, 1.0, 1.0) - kS;
		// multiply kD by the inverse metalness such that only non-metals 
		// have diffuse lighting, or a linear blend if partly metal (pure metals
		// have no diffuse light).
		kD *= 1.0 - metallic;

		// scale light by NdotL
		float NdotL = saturate(dot(N, L));

		// add to outgoing radiance Lo
		Lo += (kD * albedo / PI + specular) * radiance * NdotL; // note that we already multiplied the BRDF by the Fresnel (kS) so we won't multiply by kS again
	}

	// ambient lighting (we now use IBL as the ambient term)
	float3 F = fresnelSchlickRoughness(saturate(dot(N, V)), F0, roughness);

	float3 kS = F;
	float3 kD = 1.0 - kS;
	kD *= 1.0 - metallic;

	float3 irradiance = texCUBE(IrradianceTextureSampler, N).rgb;
	float3 diffuse = irradiance * albedo;

	// sample both the pre-filter map and the BRDF lut and combine them together as per the Split-Sum approximation to get the IBL specular part.
	const float MAX_REFLECTION_LOD = 6.0;
	float3 prefilteredColor = texCUBElod(EnviroTextureSampler, float4(R, roughness * MAX_REFLECTION_LOD)).rgb;
	float2 brdf = tex2D(BrdfLutSampler, float2(saturate(dot(N, V)), roughness)).rg;
	float3 specular = prefilteredColor * (F * brdf.x + brdf.y);

	float3 ambient = (kD * diffuse + specular) * ao;

	float3 color = ambient + Lo;

	// HDR tonemapping
	color = color / (color + float3(1.0, 1.0, 1.0));
	// gamma correct
	color = pow(color, float3(1.0 / 2.2, 1.0 / 2.2, 1.0 / 2.2));

	result.RenderTarget1 = float4(color, texColor.w);

	result.RenderTarget2 = float4(input.Depth.x, 0.0f, 0.0f, 0.0f);

	return result;
}

float4 MainPSPNTShadows(VertexShaderOutputPNTWD input) : COLOR
{
	return float4(input.Depth.x / input.Depth.y,0.0f,0.0f,0.0f);
//return float4(input.Depth,0.0f,0.0f,0.0f);	
}

technique Default
{
	pass PT20
	{
		VertexShader = compile VS_SHADERMODEL MainVSPT();
		PixelShader = compile PS_SHADERMODEL MainPSPNT();
	}


	pass PNT32
	{
		VertexShader = compile VS_SHADERMODEL MainVSPNT();
		PixelShader = compile PS_SHADERMODEL MainPSPNT();
	}

	pass PNTT40
	{
		VertexShader = compile VS_SHADERMODEL MainVSPNTT();
		PixelShader = compile PS_SHADERMODEL MainPSPNT();
	}

	pass PBBNT52
	{
		VertexShader = compile VS_SHADERMODEL MainVSPBBNT();
		PixelShader = compile PS_SHADERMODEL MainPSPNT();
	}

	pass PBBNT52Animated
	{
		VertexShader = compile VS_SHADERMODEL MainVSPBBNTAnimated();
		PixelShader = compile PS_SHADERMODEL MainPSPNT();
	}

	pass PNTTB56
	{
		VertexShader = compile VS_SHADERMODEL MainVSPNTTB();
		PixelShader = compile PS_SHADERMODEL MainPSPNTWTB();
	}

	pass PTNTB56
	{
		VertexShader = compile VS_SHADERMODEL MainVSPTNTB();
		PixelShader = compile PS_SHADERMODEL MainPSPNTWTB();
	}

	pass PBBNTTB76
	{
		VertexShader = compile VS_SHADERMODEL MainVSPNeco();
		PixelShader = compile PS_SHADERMODEL MainPSPNeco();
	}

	pass PBBNTTB76Animated
	{
		VertexShader = compile VS_SHADERMODEL MainVSPNecoAnimated();
		PixelShader = compile PS_SHADERMODEL MainPSPNeco();
	}
};

technique DefaultShadows
{
	pass PT20
	{
		VertexShader = compile VS_SHADERMODEL MainVSPT();
		PixelShader = compile PS_SHADERMODEL MainPSPNTShadows();
	}

	pass PNT32
	{
		VertexShader = compile VS_SHADERMODEL MainVSPNT();
		PixelShader = compile PS_SHADERMODEL MainPSPNTShadows();
	}

	pass PNTT40
	{
		VertexShader = compile VS_SHADERMODEL MainVSPNTT();
		PixelShader = compile PS_SHADERMODEL MainPSPNTShadows();
	}

	pass PBBNT52
	{
		VertexShader = compile VS_SHADERMODEL MainVSPBBNT();
		PixelShader = compile PS_SHADERMODEL MainPSPNTShadows();
	}

	pass PNTTB56
	{
		VertexShader = compile VS_SHADERMODEL MainVSPNTTB();
		PixelShader = compile PS_SHADERMODEL MainPSPNTShadows();
	}

	pass PTNTB56
	{
		VertexShader = compile VS_SHADERMODEL MainVSPTNTB();
		PixelShader = compile PS_SHADERMODEL MainPSPNTShadows();
	}

	pass PBBNT52Animated
	{
		VertexShader = compile VS_SHADERMODEL MainVSPBBNTAnimated();
		PixelShader = compile PS_SHADERMODEL MainPSPNTShadows();
	}

	pass PBBNTTB76Animated
	{
		VertexShader = compile VS_SHADERMODEL MainVSPBBNTTBAnimated();
		PixelShader = compile PS_SHADERMODEL MainPSPNTShadows();
	}
};