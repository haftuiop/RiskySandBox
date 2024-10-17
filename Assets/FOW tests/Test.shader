Shader "Custom/FogOfWar" {
    Properties {
        _MainTex ("Fog Texture", 2D) = "white" {}
        _FogColor ("Fog Color", Color) = (.5,.5,.5,0.5)
        _FogSpeed ("Fog Speed", Float) = 1.0
        _FogOffset ("Fog Offset", Float) = 0.0
    }
    SubShader {
        Tags { "RenderType" = "Opaque" }
        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata_t {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _FogColor;
            float _FogSpeed;
            float _FogOffset;

            v2f vert (appdata_t v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag (v2f i) : SV_Target {
                float2 uv = i.uv + _FogOffset;
                half4 texColor = tex2D(_MainTex, uv);
                return texColor * _FogColor;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}
