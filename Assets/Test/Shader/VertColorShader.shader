Shader "Custom/VertColorShader"
{
    SubShader
    {
        Tags
        {
            "Queue"="AlphaTest"
        }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                half4 vertex : POSITION; // 顶点位置
                half4 color : COLOR; // 顶点
            };

            struct v2f
            {
                half4 vertex : SV_POSITION;
                half4 color : COLOR;
            };
            

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.color = v.color;
                return o;
            }
            
            half4 frag(v2f i) : SV_Target
            {
                return  i.color;
            }
            ENDCG
        }
    }
}