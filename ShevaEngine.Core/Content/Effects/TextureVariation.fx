//#define USEHASH

float4 hash4( float2 p ) { return frac(sin(float4( 1.0+dot(p,float2(37.0,17.0)), 
                                                  2.0+dot(p,float2(11.0,47.0)),
                                                  3.0+dot(p,float2(41.0,29.0)),
                                                  4.0+dot(p,float2(23.0,31.0))))*103.0); }

float4 textureNoTile(Texture2D iChannel0, SamplerState iChannel0Sampler, in float2 uv)
{
    float2 iuv = floor( uv );
    float2 fuv = frac( uv );

#ifdef USEHASH    
    // generate per-tile transform (needs GL_NEAREST_MIPMAP_LINEARto work right)
    float4 ofa = Texture2DSample( iChannel1, iChannel1Sampler, (iuv + float2(0.5,0.5))/256.0 );
    float4 ofb = Texture2DSample( iChannel1, iChannel1Sampler, (iuv + float2(1.5,0.5))/256.0 );
    float4 ofc = Texture2DSample( iChannel1, iChannel1Sampler, (iuv + float2(0.5,1.5))/256.0 );
    float4 ofd = Texture2DSample( iChannel1, iChannel1Sampler, (iuv + float2(1.5,1.5))/256.0 );
#else
    // generate per-tile transform
    float4 ofa = hash4( iuv + float2(0.0,0.0) );
    float4 ofb = hash4( iuv + float2(1.0,0.0) );
    float4 ofc = hash4( iuv + float2(0.0,1.0) );
    float4 ofd = hash4( iuv + float2(1.0,1.0) );
#endif
        
    float2 ddxTemp = ddx( uv );
    float2 ddyTemp = ddy( uv );

    // transform per-tile uvs
    ofa.zw = sign(ofa.zw-0.5);
    ofb.zw = sign(ofb.zw-0.5);
    ofc.zw = sign(ofc.zw-0.5);
    ofd.zw = sign(ofd.zw-0.5);
        
    // uv's, and derivarives (for correct mipmapping)
    float2 uva = uv*ofa.zw + ofa.xy; float2 ddxa = ddxTemp*ofa.zw; float2 ddya = ddyTemp*ofa.zw;
    float2 uvb = uv*ofb.zw + ofb.xy; float2 ddxb = ddxTemp*ofb.zw; float2 ddyb = ddyTemp*ofb.zw;
    float2 uvc = uv*ofc.zw + ofc.xy; float2 ddxc = ddxTemp*ofc.zw; float2 ddyc = ddyTemp*ofc.zw;
    float2 uvd = uv*ofd.zw + ofd.xy; float2 ddxd = ddxTemp*ofd.zw; float2 ddyd = ddyTemp*ofd.zw;
            
    // fetch and blend
    float2 b = smoothstep(0.25,0.75,fuv);
        
    return lerp(lerp( iChannel0.SampleGrad( iChannel0Sampler, uva, ddxa, ddya ), 
                    iChannel0.SampleGrad( iChannel0Sampler, uvb, ddxb, ddyb ), b.x ), 
                lerp( iChannel0.SampleGrad( iChannel0Sampler, uvc, ddxc, ddyc ),
                    iChannel0.SampleGrad( iChannel0Sampler, uvd, ddxd, ddyd ), b.x), b.y );
    }