Shader "Cubemap" {
    Properties {
        _CubeMap ("Cube Map", Cube) = "white" {}
    }
    SubShader {
        Pass {
            Tags { "DisableBatching"="True" }
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"
 
            samplerCUBE _CubeMap;
 
            struct v2f {
                float4 pos : SV_Position;
                half3 uv : TEXCOORD0;
            };
 
            v2f vert (appdata_img v) {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.vertex.xyz * half3(1,1,1); // orientation
                return o;
            }

            float3 computeRayDir(float3 tc)
            {
                return float3(tc.x, tc.y, tc.z);
            }
 
            fixed4 frag (v2f i) : SV_Target {
              float3 dir = computeRayDir(i.uv);
                return texCUBE(_CubeMap, i.uv);
            }
            ENDCG
        }
    }
}