Shader "Cel Shading/PonyShaderV3Body6C"
{
	Properties
	{
		_BaseTex("BaseTex", 2D) = "black" {}
		_ShadowValue("Shadow Value", Range( 0 , 1)) = 0.15
		_OutlineColor("OutlineColor", Color) = (1,1,1,1)
		_OutlineBrightness("OutlineBrightness", Range( 0 , 2)) = 0.7
		_OutlineWidth("OutlineWidth", Range( 0 , 0.4)) = 0
		[Toggle]_OutlineSwitch1("Skin Color Based Outline Switch", Float) = 1
		[NoScaleOffset]_FlankmarkTex("FlankmarkTex", 2D) = "white" {}
		[NoScaleOffset]_FlankmarkZoneMask("FlankmarkZoneMask", 2D) = "black" {}
		_FlankmarkOpacity("FlankmarkOpacity", Range( 0 , 1)) = 1
		[Toggle]_FlankmarkMirror("Flankmark Mirror", Float) = 1
		_FlankmarkRotation("Flankmark Rotation", Range( 0 , 360)) = 0
		_FlankmarksBasicUVs("Flankmarks Basic UVs", Vector) = (12,12,-0.1,2.07)
		_FlankmarkColor("Flankmark Color", Color) = (1,1,1,0)
		[Toggle]_EnableEmissiveFlankMark("EnableEmissiveFlankMark", Float) = 0
		[IntRange]_EmissionFM("EmissionFM", Range( 1 , 10)) = 1
		_XVar("XVar", Range( -3 , 3)) = 0
		_YVar("YVar", Range( -3 , 3)) = 0
		_ColorMaskTex("ColorMaskTex", 2D) = "black" {}
		[Toggle(_USEADDITIONALTEXTURE_ON)] _UseAdditionalTexture("Use Additional Texture", Float) = 0
		_ColorMaskAdditionalTex("ColorMaskAdditionalTex", 2D) = "black" {}
		_Color1("Color 1", Color) = (1,1,1,1)
		_Color2("Color 2", Color) = (1,1,1,1)
		_Color3("Color 3", Color) = (1,1,1,1)
		_Color4("Color 4", Color) = (1,1,1,1)
		_Color5("Color 5", Color) = (1,1,1,1)
		_Color6("Color 6", Color) = (1,1,1,1)
		[Toggle]_EnableEmissiveColor1("EnableEmissiveColor1", Float) = 0
		[Toggle]_EnableEmissiveColor2("EnableEmissiveColor2", Float) = 0
		[Toggle]_EnableEmissiveColor3("EnableEmissiveColor3", Float) = 0
		[Toggle]_EnableEmissiveColor4("EnableEmissiveColor4", Float) = 0
		[Toggle]_EnableEmissiveColor5("EnableEmissiveColor5", Float) = 0
		[Toggle]_EnableEmissiveColor6("EnableEmissiveColor6", Float) = 0
		[IntRange]_EmissionC1("EmissionC1", Range( 1 , 10)) = 1
		[IntRange]_EmissionC2("EmissionC2", Range( 1 , 10)) = 1
		[IntRange]_EmissionC3("EmissionC3", Range( 1 , 10)) = 1
		[IntRange]_EmissionC4("EmissionC4", Range( 1 , 10)) = 1
		[IntRange]_EmissionC5("EmissionC5", Range( 1 , 10)) = 1
		[IntRange]_EmissionC6("EmissionC6", Range( 1 , 10)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
		ZWrite On
		ZTest LEqual
		Cull Front
		CGPROGRAM
		#pragma target 3.0
		#pragma surface outlineSurf Outline nofog  keepalpha noshadow noambient novertexlights nolightmap nodynlightmap nodirlightmap nometa noforwardadd vertex:outlineVertexDataFunc 
		void outlineVertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float outlineVar = _OutlineWidth;
			v.vertex.xyz += ( v.normal * outlineVar );
		}
		inline half4 LightingOutline( SurfaceOutput s, half3 lightDir, half atten ) { return half4 ( 0,0,0, s.Alpha); }
		void outlineSurf( Input i, inout SurfaceOutput o )
		{
			float3 IndirDiffLight136 = float3(0,0,0);
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			half4 ase_lightColor = 0;
			#else //aselc
			half4 ase_lightColor = _LightColor0;
			#endif //aselc
			half temp_output_137_0 = ( 1.0 - ( ( 1.0 - 1 ) * _WorldSpaceLightPos0.w ) );
			half3 ase_worldNormal = WorldNormalVector( i, half3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			half3 ase_worldlightDir = 0;
			#else //aseld
			half3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			half dotResult98 = dot( ase_worldNormal , ase_worldlightDir );
			float NdotL107 = dotResult98;
			half lerpResult140 = lerp( temp_output_137_0 , ( saturate( ( ( NdotL107 + 0.0 ) / 0.001 ) ) * 1 ) , _ShadowValue);
			half3 temp_output_147_0 = ( ( IndirDiffLight136 * ase_lightColor.a * temp_output_137_0 ) + ( ase_lightColor.rgb * lerpResult140 ) );
			half3 Shadows405 = ( temp_output_147_0 * float3( 0.8,0.8,0.8 ) );
			half4 BodyColorVar190 = _Color1;
			o.Emission = (( _OutlineSwitch1 )?( ( CalculateContrast(_OutlineBrightness,BodyColorVar190) * half4( Shadows405 , 0.0 ) ) ):( ( _OutlineColor * half4( Shadows405 , 0.0 ) ) )).rgb;
			o.Normal = float3(0,0,-1);
		}
		ENDCG
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		LOD 200
		Cull Off
		ZWrite On
		ZTest LEqual
		Blend One Zero , SrcAlpha OneMinusSrcAlpha
		
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature _USEADDITIONALTEXTURE_ON
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			half3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
			float2 uv_texcoord;
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
		uniform float4 _Color1;
		uniform sampler2D _ColorMaskTex;
		uniform half4 _ColorMaskTex_ST;
		uniform float4 _Color2;
		uniform float4 _Color3;
		uniform float4 _Color4;
		uniform sampler2D _ColorMaskAdditionalTex;
		uniform half4 _ColorMaskAdditionalTex_ST;
		uniform float4 _Color5;
		uniform float4 _Color6;
		uniform sampler2D _BaseTex;
		uniform half4 _BaseTex_ST;
		uniform float4 _FlankmarkColor;
		uniform sampler2D _FlankmarkTex;
		uniform half _FlankmarkMirror;
		uniform half4 _FlankmarksBasicUVs;
		uniform half _XVar;
		uniform half _YVar;
		uniform half _FlankmarkRotation;
		uniform half _FlankmarkOpacity;
		uniform sampler2D _FlankmarkZoneMask;
		uniform half _EnableEmissiveFlankMark;
		uniform half _EnableEmissiveColor1;
		uniform half _EmissionC1;
		uniform half _EnableEmissiveColor2;
		uniform half _EmissionC2;
		uniform half _EnableEmissiveColor3;
		uniform half _EmissionC3;
		uniform half _EnableEmissiveColor4;
		uniform half _EmissionC4;
		uniform half _EnableEmissiveColor5;
		uniform half _EmissionC5;
		uniform half _EnableEmissiveColor6;
		uniform half _EmissionC6;
		uniform half _EmissionFM;
		uniform float _OutlineWidth;
		uniform half _OutlineSwitch1;
		uniform float4 _OutlineColor;
		uniform float _OutlineBrightness;


		inline half2 RotateUV46( half2 UV , half Angle )
		{
			return mul( UV - half2( 0.5,0.5 ) , half2x2( cos(Angle) , sin(Angle), -sin(Angle) , cos(Angle) )) + half2( 0.5,0.5 );;
		}


		inline half2 RotateUV223( half2 UV , half Angle )
		{
			return mul( UV - half2( 0.5,0.5 ) , half2x2( cos(Angle) , sin(Angle), -sin(Angle) , cos(Angle) )) + half2( 0.5,0.5 );;
		}


		float4 CalculateContrast( float contrastValue, float4 colorTarget )
		{
			float t = 0.5 * ( 1.0 - contrastValue );
			return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
		}

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 OutlineVar166 = 0;
			v.vertex.xyz += OutlineVar166;
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
			half4 ase_lightColor = 0;
			#else //aselc
			half4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 LightColorData116 = ( ase_lightColor.rgb * ase_lightAtten );
			UnityGI gi134 = gi;
			float3 diffNorm134 = LightColorData116;
			gi134 = UnityGI_Base( data, 1, diffNorm134 );
			half3 indirectDiffuse134 = gi134.indirect.diffuse + diffNorm134 * 0.0001;
			float3 IndirDiffLight136 = indirectDiffuse134;
			half temp_output_137_0 = ( 1.0 - ( ( 1.0 - ase_lightAtten ) * _WorldSpaceLightPos0.w ) );
			half3 ase_worldNormal = WorldNormalVector( i, half3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			half3 ase_worldlightDir = 0;
			#else //aseld
			half3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			half dotResult98 = dot( ase_worldNormal , ase_worldlightDir );
			float NdotL107 = dotResult98;
			half lerpResult140 = lerp( temp_output_137_0 , ( saturate( ( ( NdotL107 + 0.0 ) / 0.001 ) ) * ase_lightAtten ) , _ShadowValue);
			half3 temp_output_147_0 = ( ( IndirDiffLight136 * ase_lightColor.a * temp_output_137_0 ) + ( ase_lightColor.rgb * lerpResult140 ) );
			float2 uv_ColorMaskTex = i.uv_texcoord * _ColorMaskTex_ST.xy + _ColorMaskTex_ST.zw;
			half4 break20 = tex2D( _ColorMaskTex, uv_ColorMaskTex );
			half4 temp_output_47_0 = ( _Color1 * break20.r );
			half4 temp_output_59_0 = ( _Color2 * break20.g );
			half4 temp_output_75_0 = ( _Color3 * break20.b );
			float2 uv_ColorMaskAdditionalTex = i.uv_texcoord * _ColorMaskAdditionalTex_ST.xy + _ColorMaskAdditionalTex_ST.zw;
			#ifdef _USEADDITIONALTEXTURE_ON
				float4 staticSwitch9 = tex2D( _ColorMaskAdditionalTex, uv_ColorMaskAdditionalTex );
			#else
				float4 staticSwitch9 = float4( float3(0,0,0) , 0.0 );
			#endif
			half4 break41 = staticSwitch9;
			half4 temp_output_74_0 = ( _Color4 * break41.r );
			half4 temp_output_70_0 = ( _Color5 * break41.g );
			half4 temp_output_71_0 = ( _Color6 * break41.b );
			float2 uv_BaseTex = i.uv_texcoord * _BaseTex_ST.xy + _BaseTex_ST.zw;
			half4 tex2DNode66 = tex2D( _BaseTex, uv_BaseTex );
			float4 AlphaTexVar72 = tex2DNode66;
			half4 ColorMaskTextureOutput88 = ( ( temp_output_47_0 + temp_output_59_0 + temp_output_75_0 + temp_output_74_0 + temp_output_70_0 + temp_output_71_0 ) + AlphaTexVar72 );
			half4 temp_output_128_0 = ( ColorMaskTextureOutput88 * float4( 0.7,0.7,0.7,1 ) );
			half2 temp_cast_5 = (0.5).xx;
			half2 appendResult13 = (half2(_FlankmarksBasicUVs.x , _FlankmarksBasicUVs.y));
			half2 appendResult22 = (half2(( _FlankmarksBasicUVs.z + _XVar ) , ( _FlankmarksBasicUVs.w + _YVar )));
			half2 temp_output_30_0 = ( ( ( i.uv_texcoord - temp_cast_5 ) * appendResult13 ) + appendResult22 );
			half2 UV46 = temp_output_30_0;
			half Angle46 = radians( _FlankmarkRotation );
			half2 localRotateUV46 = RotateUV46( UV46 , Angle46 );
			half2 temp_cast_6 = (0.5).xx;
			half2 UV223 = temp_output_30_0;
			half Angle223 = radians( ( _FlankmarkRotation + 270.0 ) );
			half2 localRotateUV223 = RotateUV223( UV223 , Angle223 );
			half2 FlankmarksUVs51 = (( _FlankmarkMirror )?( (localRotateUV223).yx ):( localRotateUV46 ));
			half4 FlankmarkTexVar79 = ( tex2D( _FlankmarkTex, FlankmarksUVs51 ) * _FlankmarkOpacity );
			half4 temp_output_122_0 = ( ( _FlankmarkColor * float4( 0.7,0.7,0.7,0 ) ) * FlankmarkTexVar79 );
			half temp_output_91_0 = (FlankmarkTexVar79).a;
			half FMZoneAlpha80 = tex2D( _FlankmarkZoneMask, FlankmarksUVs51 ).r;
			half ifLocalVar114 = 0;
			if( temp_output_91_0 > FMZoneAlpha80 )
				ifLocalVar114 = temp_output_91_0;
			else if( temp_output_91_0 == FMZoneAlpha80 )
				ifLocalVar114 = ( temp_output_91_0 - FMZoneAlpha80 );
			half4 lerpResult130 = lerp( temp_output_128_0 , ( temp_output_122_0 * saturate( ( ColorMaskTextureOutput88 + float4( 1,1,1,1 ) ) ) ) , ifLocalVar114);
			half4 CombinedTexture141 = lerpResult130;
			float3 BaseColor151 = ( temp_output_147_0 * (CombinedTexture141).rgb );
			half3 temp_output_171_0 = BaseColor151;
			c.rgb = temp_output_171_0;
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
			float3 IndirDiffLight136 = float3(0,0,0);
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			half4 ase_lightColor = 0;
			#else //aselc
			half4 ase_lightColor = _LightColor0;
			#endif //aselc
			half temp_output_137_0 = ( 1.0 - ( ( 1.0 - 1 ) * _WorldSpaceLightPos0.w ) );
			half3 ase_worldNormal = WorldNormalVector( i, half3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			half3 ase_worldlightDir = 0;
			#else //aseld
			half3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			half dotResult98 = dot( ase_worldNormal , ase_worldlightDir );
			float NdotL107 = dotResult98;
			half lerpResult140 = lerp( temp_output_137_0 , ( saturate( ( ( NdotL107 + 0.0 ) / 0.001 ) ) * 1 ) , _ShadowValue);
			half3 temp_output_147_0 = ( ( IndirDiffLight136 * ase_lightColor.a * temp_output_137_0 ) + ( ase_lightColor.rgb * lerpResult140 ) );
			float2 uv_ColorMaskTex = i.uv_texcoord * _ColorMaskTex_ST.xy + _ColorMaskTex_ST.zw;
			half4 break20 = tex2D( _ColorMaskTex, uv_ColorMaskTex );
			half4 temp_output_47_0 = ( _Color1 * break20.r );
			half4 temp_output_59_0 = ( _Color2 * break20.g );
			half4 temp_output_75_0 = ( _Color3 * break20.b );
			float2 uv_ColorMaskAdditionalTex = i.uv_texcoord * _ColorMaskAdditionalTex_ST.xy + _ColorMaskAdditionalTex_ST.zw;
			#ifdef _USEADDITIONALTEXTURE_ON
				float4 staticSwitch9 = tex2D( _ColorMaskAdditionalTex, uv_ColorMaskAdditionalTex );
			#else
				float4 staticSwitch9 = float4( float3(0,0,0) , 0.0 );
			#endif
			half4 break41 = staticSwitch9;
			half4 temp_output_74_0 = ( _Color4 * break41.r );
			half4 temp_output_70_0 = ( _Color5 * break41.g );
			half4 temp_output_71_0 = ( _Color6 * break41.b );
			float2 uv_BaseTex = i.uv_texcoord * _BaseTex_ST.xy + _BaseTex_ST.zw;
			half4 tex2DNode66 = tex2D( _BaseTex, uv_BaseTex );
			float4 AlphaTexVar72 = tex2DNode66;
			half4 ColorMaskTextureOutput88 = ( ( temp_output_47_0 + temp_output_59_0 + temp_output_75_0 + temp_output_74_0 + temp_output_70_0 + temp_output_71_0 ) + AlphaTexVar72 );
			half4 temp_output_128_0 = ( ColorMaskTextureOutput88 * float4( 0.7,0.7,0.7,1 ) );
			half2 temp_cast_1 = (0.5).xx;
			half2 appendResult13 = (half2(_FlankmarksBasicUVs.x , _FlankmarksBasicUVs.y));
			half2 appendResult22 = (half2(( _FlankmarksBasicUVs.z + _XVar ) , ( _FlankmarksBasicUVs.w + _YVar )));
			half2 temp_output_30_0 = ( ( ( i.uv_texcoord - temp_cast_1 ) * appendResult13 ) + appendResult22 );
			half2 UV46 = temp_output_30_0;
			half Angle46 = radians( _FlankmarkRotation );
			half2 localRotateUV46 = RotateUV46( UV46 , Angle46 );
			half2 temp_cast_2 = (0.5).xx;
			half2 UV223 = temp_output_30_0;
			half Angle223 = radians( ( _FlankmarkRotation + 270.0 ) );
			half2 localRotateUV223 = RotateUV223( UV223 , Angle223 );
			half2 FlankmarksUVs51 = (( _FlankmarkMirror )?( (localRotateUV223).yx ):( localRotateUV46 ));
			half4 FlankmarkTexVar79 = ( tex2D( _FlankmarkTex, FlankmarksUVs51 ) * _FlankmarkOpacity );
			half4 temp_output_122_0 = ( ( _FlankmarkColor * float4( 0.7,0.7,0.7,0 ) ) * FlankmarkTexVar79 );
			half temp_output_91_0 = (FlankmarkTexVar79).a;
			half FMZoneAlpha80 = tex2D( _FlankmarkZoneMask, FlankmarksUVs51 ).r;
			half ifLocalVar114 = 0;
			if( temp_output_91_0 > FMZoneAlpha80 )
				ifLocalVar114 = temp_output_91_0;
			else if( temp_output_91_0 == FMZoneAlpha80 )
				ifLocalVar114 = ( temp_output_91_0 - FMZoneAlpha80 );
			half4 lerpResult130 = lerp( temp_output_128_0 , ( temp_output_122_0 * saturate( ( ColorMaskTextureOutput88 + float4( 1,1,1,1 ) ) ) ) , ifLocalVar114);
			half4 CombinedTexture141 = lerpResult130;
			float3 BaseColor151 = ( temp_output_147_0 * (CombinedTexture141).rgb );
			half3 temp_output_171_0 = BaseColor151;
			o.Albedo = temp_output_171_0;
			half4 ColorMaskEmissiveBase318 = ( (( _EnableEmissiveColor1 )?( ( temp_output_47_0 * _EmissionC1 ) ):( float4( 0,0,0,0 ) )) + (( _EnableEmissiveColor2 )?( ( temp_output_59_0 * _EmissionC2 ) ):( float4( 0,0,0,0 ) )) + (( _EnableEmissiveColor3 )?( ( temp_output_75_0 * _EmissionC3 ) ):( float4( 0,0,0,0 ) )) + (( _EnableEmissiveColor4 )?( ( temp_output_74_0 * _EmissionC4 ) ):( float4( 0,0,0,0 ) )) + (( _EnableEmissiveColor5 )?( ( temp_output_70_0 * _EmissionC5 ) ):( float4( 0,0,0,0 ) )) + (( _EnableEmissiveColor6 )?( ( temp_output_71_0 * _EmissionC6 ) ):( float4( 0,0,0,0 ) )) );
			half4 FlankmarkTexOutput400 = temp_output_122_0;
			half4 lerpResult386 = lerp( ColorMaskEmissiveBase318 , ( ( saturate( ( ColorMaskEmissiveBase318 + float4( 1,1,1,1 ) ) ) * FlankmarkTexOutput400 ) * _EmissionFM ) , ifLocalVar114);
			half4 ColorMaskEmissiveTextureCombined374 = (( _EnableEmissiveFlankMark )?( lerpResult386 ):( ColorMaskEmissiveBase318 ));
			o.Emission = ColorMaskEmissiveTextureCombined374.rgb;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows vertex:vertexDataFunc 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO( o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				vertexDataFunc( v, customInputData );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}