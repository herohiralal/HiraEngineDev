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

    shadowAtten = MainLightRealtimeShadow(shadowCoord);

    #endif

    float nDotLSaturated = saturate(dot(WorldNormal, direction));
    float3 atten = distanceAtten * shadowAtten;
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

#endif
