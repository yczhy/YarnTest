Shader "Custom/TestFillShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _SubTex ("Texture", 2D) = "black" {}
        _IsX ("IsX", Integer) = 1
        _Limit ("Limit", Range(0.0, 1)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            sampler2D _SubTex;
            float4 _MainTex_ST;
            half _Limit;
            int _IsX;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                half val = _IsX != 0 ? i.uv.x : i.uv.y;
                // sample the texture
                fixed4 col = val > _Limit ? tex2D(_MainTex, i.uv):tex2D(_SubTex,i.uv);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return col;
            }
            ENDCG
        }
    }
}
