#ifndef CUSTOM_LIGHTING_INCLUDED
#define CUSTOM_LIGHTING_INCLUDED

void MainLightData_float(float3 WorldPos, float3 WorldNormal, out float3 Diffuse)
{
    float3 diffuseColor = 0;
    float3 direction = 1;
    float3 color = 1;
    float distanceAtten = 0;
    float shadowAtten = 0;

    #ifdef SHADERGRAPH_PREVIEW
    direction = float3(0.5, 0.5, 0);
    color = 1;
    distanceAtten = 1;
    shadowAtten = 1;
    #else
    float4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
    Light mainLight = GetMainLight(shadowCoord);
    direction = mainLight.direction;
    color = mainLight.color;
    distanceAtten = mainLight.distanceAttenuation;

    // #if !defined(_MAIN_LIGHT_SHADOWS) || defined(_RECEIVE_SHADOWS_OFF)
    // shadowAtten = 1.0;
    // #endif
    //
    // #if SHADOWS_SCREEN
    // shadowAtten = SampleScreenSpaceShadowmap(shadowCoord);
    // #else
    // ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
    // float shadowStrength = GetMainLightShadowStrength();
    // shadowAtten = SampleShadowmap(shadowCoord,
    //                               TEXTURE2D_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture),
    //                               shadowSamplingData, shadowStrength, false);
    // #endif
    
    #if !defined(_MAIN_LIGHT_SHADOWS) || defined(_RECEIVE_SHADOWS_OFF)
    shadowAtten = 1.0f;
    #else
    ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
    shadowAtten = SampleShadowmap(shadowCoord,
                                  TEXTURE2D_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture),
                                  shadowSamplingData, GetMainLightShadowStrength(), false);
    #endif

    #endif

    float nDotLSaturated = saturate(dot(WorldNormal, direction));
    float3 atten = distanceAtten * shadowAtten;
    diffuseColor = nDotLSaturated * (color * atten);

    Diffuse = diffuseColor;
}

void MainLightData_half(half3 WorldPos, half3 WorldNormal, out half3 Diffuse)
{
    half3 diffuseColor = 0;
    half3 direction = 1;
    half3 color = 1;
    half distanceAtten = 0;
    half shadowAtten = 0;

    #ifdef SHADERGRAPH_PREVIEW
    direction = half3(0.5, 0.5, 0);
    color = 1;
    distanceAtten = 1;
    shadowAtten = 1;
    #else
    #if SHADOWS_SCREEN
    half4 clipPos = TransformWorldToHClip(WorldPos);
    half4 shadowCoord = ComputeScreenPos(clipPos);
    #else
    half4 shadowCoord = TransformWorldToShadowCoord(WorldPos);
    #endif
    Light mainLight = GetMainLight(shadowCoord);
    direction = mainLight.direction;
    color = mainLight.color;
    distanceAtten = mainLight.distanceAttenuation;

    #if !defined(_MAIN_LIGHT_SHADOWS) || defined(_RECEIVE_SHADOWS_OFF)
    shadowAtten = 1.0;
    #endif

    #if SHADOWS_SCREEN
    shadowAtten = SampleScreenSpaceShadowmap(shadowCoord);
    #else
    ShadowSamplingData shadowSamplingData = GetMainLightShadowSamplingData();
    half shadowStrength = GetMainLightShadowStrength();
    shadowAtten = SampleShadowmap(shadowCoord,
                                  TEXTURE2D_ARGS(_MainLightShadowmapTexture, sampler_MainLightShadowmapTexture),
                                  shadowSamplingData, shadowStrength, false);
    #endif
    #endif

    half nDotLSaturated = saturate(dot(WorldNormal, direction));
    half3 atten = distanceAtten * shadowAtten;
    diffuseColor = nDotLSaturated * (color * atten);

    Diffuse = diffuseColor;
}

void AdditionalLights_float(float3 WorldPosition, float3 WorldNormal, float3 WorldView, out float3 Diffuse)
{
    float3 diffuseColor = 0;

    #ifndef SHADERGRAPH_PREVIEW
    WorldNormal = normalize(WorldNormal);
    WorldView = SafeNormalize(WorldView);
    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; ++i)
    {
        Light light = GetAdditionalLight(i, WorldPosition);
        half3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
        diffuseColor += LightingLambert(attenuatedLightColor, light.direction, WorldNormal);
    }
    #endif

    Diffuse = diffuseColor;
}

void AdditionalLights_half(half3 WorldPosition, half3 WorldNormal, half3 WorldView, out half3 Diffuse)
{
    half3 diffuseColor = 0;

    #ifndef SHADERGRAPH_PREVIEW
    WorldNormal = normalize(WorldNormal);
    WorldView = SafeNormalize(WorldView);
    int pixelLightCount = GetAdditionalLightsCount();
    for (int i = 0; i < pixelLightCount; ++i)
    {
        Light light = GetAdditionalLight(i, WorldPosition);
        half3 attenuatedLightColor = light.color * (light.distanceAttenuation * light.shadowAttenuation);
        diffuseColor += LightingLambert(attenuatedLightColor, light.direction, WorldNormal);
    }
    #endif

    Diffuse = diffuseColor;
}

#endif
