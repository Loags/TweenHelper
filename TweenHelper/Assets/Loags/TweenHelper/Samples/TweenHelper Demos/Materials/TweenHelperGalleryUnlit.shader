Shader "Tween Helper/Gallery Faceted"
{
    Properties
    {
        [MainColor] _Color ("Base Color", Color) = (0.08, 0.5, 0.92, 1)
        _ShadowColor ("Shadow Color", Color) = (0.018, 0.105, 0.28, 1)
        _HighlightColor ("Highlight Color", Color) = (0.3, 0.86, 1, 1)
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry" "RenderPipeline" = "UniversalPipeline" }

        Pass
        {
            CGPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "UnityCG.cginc"

            struct Attributes
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
            };

            fixed4 _Color;
            fixed4 _ShadowColor;
            fixed4 _HighlightColor;

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.position = UnityObjectToClipPos(input.vertex);
                output.worldNormal = UnityObjectToWorldNormal(input.normal);
                return output;
            }

            fixed4 Frag(Varyings input) : SV_Target
            {
                float3 normal = normalize(input.worldNormal);
                float3 lightDirection = normalize(float3(-0.45, 0.8, -0.55));
                float keyLight = saturate(dot(normal, lightDirection));
                float topLight = saturate(normal.y * 0.5 + 0.5);
                float3 shaded = lerp(_ShadowColor.rgb, _Color.rgb, 0.28 + keyLight * 0.72);
                shaded = lerp(shaded, _HighlightColor.rgb, topLight * 0.2);
                return fixed4(shaded, _Color.a);
            }
            ENDCG
        }
    }

    SubShader
    {
        Tags { "RenderType" = "Opaque" "Queue" = "Geometry" }

        Pass
        {
            CGPROGRAM
            #pragma vertex Vert
            #pragma fragment Frag
            #include "UnityCG.cginc"

            struct Attributes
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
            };

            struct Varyings
            {
                float4 position : SV_POSITION;
                float3 worldNormal : TEXCOORD0;
            };

            fixed4 _Color;
            fixed4 _ShadowColor;
            fixed4 _HighlightColor;

            Varyings Vert(Attributes input)
            {
                Varyings output;
                output.position = UnityObjectToClipPos(input.vertex);
                output.worldNormal = UnityObjectToWorldNormal(input.normal);
                return output;
            }

            fixed4 Frag(Varyings input) : SV_Target
            {
                float3 normal = normalize(input.worldNormal);
                float3 lightDirection = normalize(float3(-0.45, 0.8, -0.55));
                float keyLight = saturate(dot(normal, lightDirection));
                float topLight = saturate(normal.y * 0.5 + 0.5);
                float3 shaded = lerp(_ShadowColor.rgb, _Color.rgb, 0.28 + keyLight * 0.72);
                shaded = lerp(shaded, _HighlightColor.rgb, topLight * 0.2);
                return fixed4(shaded, _Color.a);
            }
            ENDCG
        }
    }
}
