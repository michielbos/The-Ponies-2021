Shader "Cel Shading/WallsMarkers"
{
	Properties
	{
		_Color("Color", Color) = (1,1,1,1)
		_ShadowValue("Shadow Value", Range( 0 , 1)) = 0.15
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		LOD 200
		Cull Back
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 2.0
		#pragma surface surf StandardCustomLighting alpha:fade keepalpha noshadow 
		struct Input
		{
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform half _ShadowValue;
		uniform half4 _Color;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			half AlphaValue59 = _Color.a;
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			half3 LightColorData17 = ( ase_lightColor.rgb * ase_lightAtten );
			UnityGI gi30 = gi;
			float3 diffNorm30 = LightColorData17;
			gi30 = UnityGI_Base( data, 1, diffNorm30 );
			float3 indirectDiffuse30 = gi30.indirect.diffuse + diffNorm30 * 0.0001;
			half3 IndirDiffLight34 = indirectDiffuse30;
			float temp_output_35_0 = ( 1.0 - ( ( 1.0 - ase_lightAtten ) * _WorldSpaceLightPos0.w ) );
			half3 ase_worldNormal = WorldNormalVector( i, half3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult8 = dot( ase_worldNormal , ase_worldlightDir );
			half NdotL10 = dotResult8;
			float lerpResult38 = lerp( temp_output_35_0 , ( saturate( ( ( NdotL10 + 0.0 ) / 0.001 ) ) * ase_lightAtten ) , _ShadowValue);
			half3 InputColor48 = (( _Color * float4( 0.7,0.7,0.7,1 ) )).rgb;
			half3 BaseColorOutput55 = ( ( ( IndirDiffLight34 * ase_lightColor.a * temp_output_35_0 ) + ( ase_lightColor.rgb * lerpResult38 ) ) * InputColor48 );
			float3 temp_output_57_0 = BaseColorOutput55;
			c.rgb = temp_output_57_0;
			c.a = AlphaValue59;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
			half3 IndirDiffLight34 = float3(0,0,0);
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float temp_output_35_0 = ( 1.0 - ( ( 1.0 - 1 ) * _WorldSpaceLightPos0.w ) );
			half3 ase_worldNormal = WorldNormalVector( i, half3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult8 = dot( ase_worldNormal , ase_worldlightDir );
			half NdotL10 = dotResult8;
			float lerpResult38 = lerp( temp_output_35_0 , ( saturate( ( ( NdotL10 + 0.0 ) / 0.001 ) ) * 1 ) , _ShadowValue);
			half3 InputColor48 = (( _Color * float4( 0.7,0.7,0.7,1 ) )).rgb;
			half3 BaseColorOutput55 = ( ( ( IndirDiffLight34 * ase_lightColor.a * temp_output_35_0 ) + ( ase_lightColor.rgb * lerpResult38 ) ) * InputColor48 );
			float3 temp_output_57_0 = BaseColorOutput55;
			o.Albedo = temp_output_57_0;
		}

		ENDCG
	}
	Fallback "Legacy Shaders/Diffuse"
}
