Shader "Unlit/InvisibleMask"
{
     SubShader
    {
        Tags { "Queue" = "Geometry-1" }  // Render before the arrow
        Pass
        {
            ZWrite On     // Write depth to the depth buffer
            ZTest LEqual  // Compare the depth, objects in front will hide behind
            ColorMask 0   // Do not write color, just depth
        }
    }
            Fallback "Diffuse"

}