Shader "Custom/TestShaderA"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _Color("Color", Color) = (1,1,1,1)
        _Direction ("Direction",Vector) = (0,-1,0,0)// 初识偏移向量
        _NormalDir ("NormalDir",Vector) = (0,-1,0,0)// 最终偏移向量
        _Interval("_Interval", Float) = 1// 时间间隔
        _CreateDuration ("CreateDuration",Float) = 2// 创建动画时间
        _WaitDuration("WaitDuration",Float) = 0.5// 中间等待时间
        _CloseDuration("CloseDuration",Float) = 1.5// 拉紧动画时间
        _StartTime("StartTime", Float) = 0
        [Toggle(ENABLE_ANGLE_Y)] _EnableAngleY ("Enable Angle Y", Float) = 0
        [Toggle(ENABLE_SCALE_Z)] _EnableScaleZ ("Enable Scale Z", Float) = 0
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
            #pragma shader_feature ENABLE_ANGLE_Y
            #pragma shader_feature ENABLE_SCALE_Z

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION; // 顶点位置
                float4 color : COLOR; // 顶点色
                float3 normal : NORMAL; // 法线方向
                float2 uv : TEXCOORD0; // UV坐标
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float idx : TEXCOORD1;
            };

            sampler2D _MainTex;
            float4 _Color;
            float4 _MainTex_ST;
            float4 _Direction;
            float4 _NormalDir;
            float _Interval;
            float _StartTime;
            float _CreateDuration;
            float _WaitDuration;
            float _CloseDuration;

            float GetTime()
            {
                float t = _Time.y - _StartTime;
                // test
                // float totalTime = (_CreateDuration + _WaitDuration + _CloseDuration + 1) * 256;
                // t %= totalTime;
                // endTest
                return t;
            }

            float GetIndex(float4 color)
            {
                const float _byte = 256;
                // const float r = _byte * _byte * _byte * _byte;
                // const float g = _byte * _byte * _byte;
                const float b = _byte * _byte;
                const float a = _byte;
                // return color.r * r + color.g * g + color.b * b + color.a * a;
                return color.b * b + color.a * a;
                // return color.a * a;
            }

            float4x4 GetRotateY(float delta)
            {
                return float4x4(
                    cos(delta), 0, sin(delta), 0,
                    0, 1, 0, 0,
                    -sin(delta), 0, cos(delta), 0,
                    0, 0, 0, 1);
            }

            float4x4 GetRotateY(float4 color)
            {
                float delta = color.x * UNITY_TWO_PI;
                return GetRotateY(delta);
            }

            // 拉紧动画插值
            float4 GetVert(appdata v)
            {
                float t = GetTime();
                float idx = GetIndex(v.color);
                t -= idx * _Interval;

                t -= _CreateDuration + _WaitDuration;
                t = clamp(t / _CloseDuration, 0, 1);

                float4 direction = _Direction;
                float4 normal = _NormalDir;

                #if ENABLE_ANGLE_Y
                float4x4 rotate = GetRotateY(v.color);
                direction = mul(rotate, direction);
                normal = mul(rotate, normal);
                #endif
                #if ENABLE_SCALE_Z
                const float baseScale = 255 / 100; 
                float scale = v.color.y * baseScale;
                direction *= scale;
                normal *= scale;
                #endif

                float4 ret = lerp(direction, normal, t);
                // float4 ret = smoothstep(_Direction, _NormalDir, t);

                float uvx = v.uv.x;
                uvx = clamp(uvx, 0, 1);
                float yMax = -4 * pow(uvx - 0.5, 2) + 1;
                float yMin = -0.8 * pow(uvx - 0.5, 2) + 0.2;

                float y = lerp(yMax, yMin, t);
                // float y = smoothstep(yMax, yMin, t);

                return ret * y;
            }

            v2f vert(appdata v)
            {
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                v.vertex += GetVert(v);
                o.idx = GetIndex(v.color) + v.uv.x;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            // 创建动画插值
            float GetAlpha(v2f v)
            {
                float uvx = v.uv.x;
                
                float t = GetTime();
                t -= (v.idx - uvx) * _Interval;
                
                uvx *= _CreateDuration;
                // step ::: a < b ? 0 : 1 
                return step(uvx, t);
            }

            float4 frag(v2f i) : SV_Target
            {
                float4 col = tex2D(_MainTex, i.uv);
                col *= _Color;
                col.a *= GetAlpha(i);

                // 丢弃透明的片元
                clip(col.a - 0.01);
                // clip(1 - i.uv.x);
                // clip(1 - i.uv.y);
                // if (col.a <= 0 || i.uv.x > 1 || i.uv.y > 1) { discard ; }
                return col;
            }
            ENDCG
        }
    }
}