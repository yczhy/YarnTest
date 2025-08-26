Shader "Custom/TestShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Direction ("Direction",Vector) = (0,-1,0,0)// 初识偏移向量
        _NormalDir ("NormalDir",Vector) = (0,-1,0,0)// 最终偏移向量
        _Interval("_Interval", Range(0.0, 0.5)) = 0.1
        _CreateDuration ("CreateDuration",Float) = 2// 创建动画时间
        _WaitDuration("WaitDuration",Float) = 0.5// 中间等待时间
        _CloseDuration("CloseDuration",Float) = 1.5// 拉紧动画时间
        _StartTime("StartTime", Float) = 0
    }

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
                uint vid : SV_VertexID; // 顶点ID
                half3 normal : NORMAL; // 法线方向
                half2 uv : TEXCOORD0; // UV坐标
            };

            struct v2f
            {
                half2 uv : TEXCOORD0;
                half4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            half4 _MainTex_ST;
            half4 _Direction;
            half4 _NormalDir;
            half _Interval;
            half _StartTime;
            half _CreateDuration;
            half _WaitDuration;
            half _CloseDuration;

            half4 GetTime()
            {
                half t = _Time.y - _StartTime;
                // test
                half totalTime = _CreateDuration + _WaitDuration + _CloseDuration + 1;
                t %= totalTime;
                // endTest
                return t;
            }

            // 拉紧动画插值
            half4 GetVert(appdata v)
            {
                half t = GetTime();
                t -= _CreateDuration + _WaitDuration;
                t = clamp(t / _CloseDuration, 0, 1);

                half4 ret = lerp(_Direction, _NormalDir, t);

                half uvx = v.uv.x;
                // uvx -= _Interval * 0.5;
                // uvx /= 1 - _Interval;
                uvx = clamp(uvx, 0, 1);
                half yMax = sin(uvx * UNITY_PI);
                half yMin = sin(uvx * UNITY_PI) * 0.5;
                half y = lerp(yMax, yMin, t);

                return ret * y;
            }

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                v.vertex += GetVert(v);
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            // 创建动画插值
            half GetAlpha(v2f v)
            {
                half t = GetTime();

                half uvx = v.uv.x;
                uvx *= _CreateDuration;
                // step ::: a < b ? 0 : 1 
                return step(uvx, t);
            }

            half4 frag(v2f i) : SV_Target
            {
                half4 col = tex2D(_MainTex, i.uv);
                col.a = GetAlpha(i);

                // // clip ::: if (a < 0) discard 
                // // 丢弃位于接缝的片元
                // clip(0.5 - abs(i.uv.x - 0.5) - _Interval * 0.5);

                // 丢弃透明的片元
                if (col.a <= 0) { discard ; }
                return col;
            }
            ENDCG
        }
    }
}