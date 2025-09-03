Shader "UI/SquareRevealShader"
{
    Properties
    {
        _Center ("Center", Vector) = (0.5, 0.5, 0, 0)
        _HalfSize ("Half Size", Float) = 0.0
    }
    SubShader
    {
        Tags { "Queue"="Overlay" "RenderType"="Transparent" }
        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Off

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            float2 _Center;
            float _HalfSize;

            fixed4 frag (v2f i) : SV_Target
            {
                float2 d = abs(i.uv - _Center);

                float inside = max(d.x, d.y);

                float alpha = smoothstep(_HalfSize, _HalfSize, inside);

                return float4(0,0,0, alpha);
            }
            ENDCG
        }
    }
}
