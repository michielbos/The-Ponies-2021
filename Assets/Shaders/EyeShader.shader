Shader "Cel Shading/EyeShader"
{
	Properties
	{
		_EyePupilTex("EyePupilTex", 2D) = "white" {}
		_EyeGlossMask("Eye Gloss Mask", 2D) = "white" {}
		_Color("Color", Color) = (1,1,1,1)
		_ShadowValue("Shadow Value", Range( 0 , 1)) = 0.15
		_Tiling("Tiling", Range( -0.25 , 4)) = 1.5
		_YOffset("YOffset", Range( -4 , 4)) = 0
		_XOffset("XOffset", Range( -4 , 4)) = 0
		_EyeRotation("EyeRotation", Range( 0 , 360)) = 10
		[Toggle]_LtoR("LtoR", Float) = 1
		_BaseYOffset("BaseYOffset", Range( -4 , 4)) = 0
		_BaseXOffset("BaseXOffset", Range( -4 , 4)) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		LOD 200
		Cull Back
		CGPROGRAM
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf StandardCustomLighting keepalpha 
		struct Input
		{
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
			float2 uv_texcoord;
			half ASEVFace : VFACE;
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

		uniform float _ShadowValue;
		uniform float4 _EyeColor;
		uniform sampler2D _EyePupilTex;
		uniform float _Tiling;
		uniform float _XOffset;
		uniform float _BaseXOffset;
		uniform float _YOffset;
		uniform float _BaseYOffset;
		uniform float _EyeRotation;
		uniform sampler2D _EyeGlossMask;
		uniform float _LtoR;


		inline half2 RotateUV294( half2 UV , half Angle )
		{
			return mul( UV - half2( 0.5,0.5 ) , half2x2( cos(Angle) , sin(Angle), -sin(Angle) , cos(Angle) )) + half2( 0.5,0.5 );;
		}


		inline half2 RotateUV293( half2 UV , half Angle )
		{
			return mul( UV - half2( 0.5,0.5 ) , half2x2( cos(Angle) , sin(Angle), -sin(Angle) , cos(Angle) )) + half2( 0.5,0.5 );;
		}


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
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 LightColorData130 = ( ase_lightColor.rgb * ase_lightAtten );
			UnityGI gi133 = gi;
			float3 diffNorm133 = LightColorData130;
			gi133 = UnityGI_Base( data, 1, diffNorm133 );
			float3 indirectDiffuse133 = gi133.indirect.diffuse + diffNorm133 * 0.0001;
			float3 IndirDiffLight134 = indirectDiffuse133;
			float temp_output_147_0 = ( 1.0 - ( ( 1.0 - ase_lightAtten ) * _WorldSpaceLightPos0.w ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult124 = dot( ase_worldNormal , ase_worldlightDir );
			float NdotL127 = dotResult124;
			float lerpResult149 = lerp( temp_output_147_0 , ( saturate( ( ( NdotL127 + 0.0 ) / 0.001 ) ) * ase_lightAtten ) , _ShadowValue);
			float4 ColorVar12 = float4(0.7,0.7,0.7,1);
			float temp_output_185_0 = ( _Tiling + 1.0 );
			float2 appendResult166 = (float2(temp_output_185_0 , temp_output_185_0));
			float lerpResult175 = lerp( 0.0 , -2.0 , ( _Tiling / 9.0 ));
			float2 appendResult177 = (float2(( ( _XOffset + _BaseXOffset ) + lerpResult175 ) , ( ( ( _YOffset + _BaseYOffset ) * -1.0 ) + lerpResult175 )));
			float2 uv_TexCoord164 = i.uv_texcoord * appendResult166 + appendResult177;
			half2 UV294 = uv_TexCoord164;
			half Angle294 = radians( _EyeRotation );
			half2 localRotateUV294 = RotateUV294( UV294 , Angle294 );
			half2 EyeUVs88 = localRotateUV294;
			half4 EyeTexVar10 = tex2D( _EyePupilTex, EyeUVs88 );
			float3 _WhiteColor = float3(1,1,1);
			float2 uv_TexCoord262 = i.uv_texcoord * appendResult166 + appendResult177;
			half2 UV293 = uv_TexCoord262;
			half Angle293 = lerp(radians( ( 180.0 * ase_worldlightDir ) ),radians( ( ase_worldlightDir * -180.0 ) ),_LtoR).x;
			half2 localRotateUV293 = RotateUV293( UV293 , Angle293 );
			half2 GlossUV265 = localRotateUV293;
			half3 EyeGlossTextVar232 = (tex2D( _EyeGlossMask, GlossUV265 )).rgb;
			float3 break246 = EyeGlossTextVar232;
			float3 temp_output_242_0 = ( _WhiteColor * break246.y );
			float3 temp_output_247_0 = ( _WhiteColor * break246.z );
			float4 MaskedEyeVar75 = ( ( _EyeColor * EyeTexVar10 ) + float4( temp_output_242_0 , 0.0 ) + float4( temp_output_247_0 , 0.0 ) );
			float4 lerpResult115 = lerp( ColorVar12 , ( ( float4( float3(0.7,0.7,0.7) , 0.0 ) * MaskedEyeVar75 ) * saturate( ( ColorVar12 + 1.0 ) ) ) , (MaskedEyeVar75).a);
			float4 switchResult117 = (((i.ASEVFace>0)?(lerpResult115):(ColorVar12)));
			half4 CombinedTexture118 = switchResult117;
			float3 BaseColor158 = ( ( ( IndirDiffLight134 * ase_lightColor.a * temp_output_147_0 ) + ( ase_lightColor.rgb * lerpResult149 ) ) * (CombinedTexture118).rgb );
			float3 temp_output_159_0 = BaseColor158;
			c.rgb = temp_output_159_0;
			c.a = 1;
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
			float3 IndirDiffLight134 = float3(0,0,0);
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float temp_output_147_0 = ( 1.0 - ( ( 1.0 - 1 ) * _WorldSpaceLightPos0.w ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult124 = dot( ase_worldNormal , ase_worldlightDir );
			float NdotL127 = dotResult124;
			float lerpResult149 = lerp( temp_output_147_0 , ( saturate( ( ( NdotL127 + 0.0 ) / 0.001 ) ) * 1 ) , _ShadowValue);
			float4 ColorVar12 = float4(0.7,0.7,0.7,1);
			float temp_output_185_0 = ( _Tiling + 1.0 );
			float2 appendResult166 = (float2(temp_output_185_0 , temp_output_185_0));
			float lerpResult175 = lerp( 0.0 , -2.0 , ( _Tiling / 9.0 ));
			float2 appendResult177 = (float2(( ( _XOffset + _BaseXOffset ) + lerpResult175 ) , ( ( ( _YOffset + _BaseYOffset ) * -1.0 ) + lerpResult175 )));
			float2 uv_TexCoord164 = i.uv_texcoord * appendResult166 + appendResult177;
			half2 UV294 = uv_TexCoord164;
			half Angle294 = radians( _EyeRotation );
			half2 localRotateUV294 = RotateUV294( UV294 , Angle294 );
			half2 EyeUVs88 = localRotateUV294;
			half4 EyeTexVar10 = tex2D( _EyePupilTex, EyeUVs88 );
			float3 _WhiteColor = float3(1,1,1);
			float2 uv_TexCoord262 = i.uv_texcoord * appendResult166 + appendResult177;
			half2 UV293 = uv_TexCoord262;
			half Angle293 = lerp(radians( ( 180.0 * ase_worldlightDir ) ),radians( ( ase_worldlightDir * -180.0 ) ),_LtoR).x;
			half2 localRotateUV293 = RotateUV293( UV293 , Angle293 );
			half2 GlossUV265 = localRotateUV293;
			half3 EyeGlossTextVar232 = (tex2D( _EyeGlossMask, GlossUV265 )).rgb;
			float3 break246 = EyeGlossTextVar232;
			float3 temp_output_242_0 = ( _WhiteColor * break246.y );
			float3 temp_output_247_0 = ( _WhiteColor * break246.z );
			float4 MaskedEyeVar75 = ( ( _EyeColor * EyeTexVar10 ) + float4( temp_output_242_0 , 0.0 ) + float4( temp_output_247_0 , 0.0 ) );
			float4 lerpResult115 = lerp( ColorVar12 , ( ( float4( float3(0.7,0.7,0.7) , 0.0 ) * MaskedEyeVar75 ) * saturate( ( ColorVar12 + 1.0 ) ) ) , (MaskedEyeVar75).a);
			float4 switchResult117 = (((i.ASEVFace>0)?(lerpResult115):(ColorVar12)));
			half4 CombinedTexture118 = switchResult117;
			float3 BaseColor158 = ( ( ( IndirDiffLight134 * ase_lightColor.a * temp_output_147_0 ) + ( ase_lightColor.rgb * lerpResult149 ) ) * (CombinedTexture118).rgb );
			float3 temp_output_159_0 = BaseColor158;
			o.Albedo = temp_output_159_0;
			float3 EyeGlossVar355 = ( temp_output_242_0 + temp_output_247_0 );
			o.Emission = EyeGlossVar355;
		}

		ENDCG
	}
	Fallback "Diffuse"
}