#define MESH
#include "Common\DataStructs.hlsl"
#include "Common\CommonBuffers.hlsl"

float4 main(PSInput input) : SV_Target
{
    float4 c = input.cDiffuse;
    if (bHasDiffuseMap)
    {
        c *= texDiffuseMap.Sample(samplerSurface, input.t);
    }
    c.rgb = saturate(c.rgb * input.c.rgb * 2.0);
    return c;
}
