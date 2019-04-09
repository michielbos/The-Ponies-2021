Shader "Cel Shading/SurfaceDoubleSided"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1)
		_HColor ("Highlight Color", Color) = (0.785,0.785,0.785,1.0)
		_SColor ("Shadow Color", Color) = (0.5,0.5,0.5,1.0)
		
		_MainTex ("Main Texture", 2D) = "white" {}
		
		_RampThreshold ("Ramp Threshold", Range(0,1)) = 0.1
		_RampSmooth ("Ramp Smoothing", Range(0.01,1)) = 0.01
		
		_Cutoff("AlphaCutoff", Range( 0 , 0.75)) = 0.75
		
	}
	
	SubShader
	{
        Tags {
           // "Queue"="AlphaTest"
            "RenderType"="Opaque"
        }
		LOD 200
		Cull Off		
		CGPROGRAM
		
		#include "CelShading_Include.cginc"
		
		#pragma surface surf Celshading

		// mobile optimized
		//#pragma surface surf Celshading noforwardadd interpolateview halfasview

		#pragma target 3.0
		
        uniform float _Cutoff;		
		fixed4 _Color;
		sampler2D _MainTex;
		
		
		struct Input
		{
			half2 uv_MainTex : TEXCOORD0;
		};
		
		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb * _Color.rgb;
			o.Alpha = c.a * _Color.a;
			clip(o.Alpha-_Cutoff);
		}
		
		ENDCG
		
	}
	
	Fallback "Transparent/Cutout/Diffuse"
}