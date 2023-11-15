
Shader"ShaderMan/MyShader"
	{
	Properties{
	
	}
	SubShader
	{
	Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }
	Pass
	{
ZWrite Off

Blend SrcAlpha

OneMinusSrcAlpha
	CGPROGRAM
	#pragma vertex vert
	#pragma fragment frag
#include "UnityCG.cginc"
			
    

float4 vec4(float x, float y, float z, float w)
{
    return float4(x, y, z, w);
}
float4 vec4(float x)
{
    return float4(x, x, x, x);
}
float4 vec4(float2 x, float2 y)
{
    return float4(float2(x.x, x.y), float2(y.x, y.y));
}
float4 vec4(float3 x, float y)
{
    return float4(float3(x.x, x.y, x.z), y);
}


float3 vec3(float x, float y, float z)
{
    return float3(x, y, z);
}
float3 vec3(float x)
{
    return float3(x, x, x);
}
float3 vec3(float2 x, float y)
{
    return float3(float2(x.x, x.y), y);
}

float2 vec2(float x, float y)
{
    return float2(x, y);
}
float2 vec2(float x)
{
    return float2(x, x);
}

float vec(float x)
{
    return float(x);
}
    
    

struct VertexInput
{
    float4 vertex : POSITION;
    float2 uv : TEXCOORD0;
    float4 tangent : TANGENT;
    float3 normal : NORMAL;
	//VertexInput
};
struct VertexOutput
{
    float4 pos : SV_POSITION;
    float2 uv : TEXCOORD0;
	//VertexOutput
};
	
	
VertexOutput vert(VertexInput v)
{
    VertexOutput o;
    o.pos = UnityObjectToClipPos(v.vertex);
    o.uv = v.uv;
	//VertexFactory
    return o;
}
    
    // Created by beautypi - beautypi/2012

const float2x2 m = float2x2(0.80, 0.60, -0.60, 0.80);

float hash(float n)
{
    return frac(sin(n) * 43758.5453);
}

float noise(in float2 x)
{
    float2 p = floor(x);
    float2 f = frac(x);

    f = f * f * (3.0 - 2.0 * f);

    float n = p.x + p.y * 57.0;

    return lerp(lerp(hash(n + 0.0), hash(n + 1.0), f.x),
               lerp(hash(n + 57.0), hash(n + 58.0), f.x), f.y);
}

float fbm(float2 p)
{
    float f = 0.0;

    f += 0.50000 * noise(p);
    p = mul(m, p) * 2.02;
    f += 0.25000 * noise(p);
    p = mul(m, p) * 2.03;
    f += 0.12500 * noise(p);
    p = mul(m, p) * 2.01;
    f += 0.06250 * noise(p);
    p = mul(m, p) * 2.04;
    f += 0.03125 * noise(p);

    return f / 0.984375;
}

float length2(float2 p)
{
    float2 q = p * p * p * p;
    return pow(q.x + q.y, 1.0 / 4.0);
}


    
    
fixed4 frag(VertexOutput vertex_output) : SV_Target
{
	
    float2 q = vertex_output.uv / 1;
    float2 p = -1.0 + 2.0 * q;
    p.x *= 1 / 1;

    float r = length(p);
    float a = atan2(p.x, p.y);

    float dd = 0.2 * sin(4.0 * _Time.y);
    float ss = 1.0 + clamp(1.0 - r, 0.0, 1.0) * dd;

    r *= ss;

    float3 col = vec3(0.0, 0.3, 0.4);

    float f = fbm(5.0 * p);
    col = lerp(col, vec3(0.2, 0.5, 0.4), f);

    col = lerp(col, vec3(0.9, 0.6, 0.2), 1.0 - smoothstep(0.2, 0.6, r));

    a += 0.05 * fbm(20.0 * p);

    f = smoothstep(0.3, 1.0, fbm(vec2(20.0 * a, 6.0 * r)));
    col = lerp(col, vec3(1.0, 1.0, 1.0), f);

    f = smoothstep(0.4, 0.9, fbm(vec2(15.0 * a, 10.0 * r)));
    col *= 1.0 - 0.5 * f;

    col *= 1.0 - 0.25 * smoothstep(0.6, 0.8, r);

    f = 1.0 - smoothstep(0.0, 0.6, length2(mul(float2x2(0.6, 0.8, -0.8, 0.6), (p - vec2(0.3, 0.5))) * vec2(1.0, 2.0)));

    col += vec3(1.0, 0.9, 0.9) * f * 0.985;

    col *= vec3(0.8 + 0.2 * cos(r * a));

    f = 1.0 - smoothstep(0.2, 0.25, r);
    col = lerp(col, vec3(0.0), f);

    f = smoothstep(0.79, 0.82, r);
    col = lerp(col, vec3(1.0), f);

    col *= 0.5 + 0.5 * pow(16.0 * q.x * q.y * (1.0 - q.x) * (1.0 - q.y), 0.1);
 
    return vec4(col, 1.0);

}
	ENDCG
	}
  }
  }
