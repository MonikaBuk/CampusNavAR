Shader "Unlit/TransparentArrow"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" { }
    }

    SubShader
    {
        Tags { "Queue" = "Geometry+1" "RenderType" = "Transparent" }

        Pass
        {
            // Ensure writing to the depth buffer and perform depth testing
            ZWrite On            // Write to the depth buffer
            ZTest LEqual         // Use depth testing: render if in front of something else

            Blend SrcAlpha OneMinusSrcAlpha  // Standard transparency blend mode
            ColorMask RGB        // Write to RGB channels, but not alpha
       
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float4 pos : POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            v2f vert(appdata v)
            {
                v2f o;
                o.pos = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv;
                return o;
            }

            half4 frag(v2f i) : SV_Target
            {
                // Sample the texture
                half4 texColor = tex2D(_MainTex, i.uv);
                
                // If the texture is fully transparent (alpha < 0.1), discard it
                if (texColor.a < 0.1)
                    discard;  // Skip rendering fully transparent pixels

                // Return the color with transparency
                return texColor;
            }

            ENDCG
        }
    }

    Fallback "Diffuse"
}
