Shader "Unlit/ShellShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        Cull off
        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"
            #include "Leehbrary.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4  _FurBaseColor ;
            float4 _MainTex_ST;
            float _LayerIndex;
            float _LayerCount;
            float _Tickness,_Density;
            float _MinNoise,_MaxNoise,_FurDistanceAttenuation;
            float _DisplacementStrength;
            float _OcclusionAttenuation;
            float _OcclusionBias;
            float _Furlength;
            float _Curvature;
            float3 _DisplacementDirection;
            float hash(uint n) {
                // integer hash copied from Hugo Elias
                n = (n << 13U) ^ n;
                n = n * (n * n * 15731U + 0x789221U) + 0x1376312589U;
                return float(n & uint(0x7fffffffU)) / float(0x7fffffff);
            }
            float3 GetWorldScale()
            {
                return float3(
                length(float3(unity_ObjectToWorld[0].x, unity_ObjectToWorld[1].x, unity_ObjectToWorld[2].x)), // scale x axis
                length(float3(unity_ObjectToWorld[0].y, unity_ObjectToWorld[1].y, unity_ObjectToWorld[2].y)), // scale y axis
                length(float3(unity_ObjectToWorld[0].z, unity_ObjectToWorld[1].z, unity_ObjectToWorld[2].z))  // scale z axis
                );
            }
            v2f vert (appdata v)
            {
                v2f o;
                float shellHeight =_LayerIndex/_LayerCount;
                shellHeight = pow(shellHeight,_FurDistanceAttenuation);
                v.vertex.xyz += v.normal*_Furlength *shellHeight;
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.normal =normalize(mul(unity_ObjectToWorld,v.normal ));
                float k = pow(shellHeight, _Curvature);

                v.vertex.xyz += (_DisplacementDirection* k *_DisplacementStrength)/GetWorldScale();
                v.vertex.xyz += (float3(0,-.28,0)* k)/GetWorldScale();
                o.vertex = UnityObjectToClipPos(v.vertex);
                
                return o;
            }
            
            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                // return float4(i.uv,0,1);
                float density =_Density;
                float2 newUV = i.uv * density;
                uint2 tid = newUV;
                float seed = (tid.x + 100 * tid.y + 100) * 10;
                float rand= lerp(_MinNoise,_MaxNoise,hash(seed));
                // return rand;
                newUV=(frac((newUV)));
                float2 center = (newUV-.5) * 2;

                float circle = length(center );

                float h = _LayerIndex/_LayerCount;
                if(_LayerIndex <= 0)
                return 0; 
                
                if( circle > _Tickness*(rand-h)) discard;

                float ndotl = saturate(dot(i.normal, _WorldSpaceLightPos0)) * 0.5f + 0.5f;
                ndotl = ndotl * ndotl;
                float ambientOcclusion =pow(h,_OcclusionAttenuation);
                ambientOcclusion += _OcclusionBias;
                ambientOcclusion=saturate(ambientOcclusion);
                // return ndotl;
                // i.uv.x+= sin(_Time.y)*_FurLength*10;
                return  _FurBaseColor*ndotl *ambientOcclusion ;
            }
            ENDCG
        }
    }
}
