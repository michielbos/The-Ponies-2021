Shader "Cel Shading/PonyShaderV2"
{
	Properties
	{
		_AlphaTex("AlphaTex", 2D) = "gray" {}
		_AlphaCutout("AlphaCutout", Range( 0 , 0.75)) = 0
		_ShadowValue("Shadow Value", Range( 0 , 1)) = 0.15
		_OutlineColor("OutlineColor", Color) = (1,1,1,1)
		_OutlineBrightness("OutlineBrightness", Range( 0 , 2)) = 1
		_OutlineWidth("OutlineWidth", Range( 0 , 0.05)) = 0
		[Toggle]_OutlineSwitch("Texture Based Outline Switch", Float) = 1
		[Enum(Overlay,0,Overlap,1)]_Cutiemarkmode("CM Render Mode", Float) = 1
		_CutieMarkTex("CutieMarkTex", 2D) = "white" {}
		_CutiemarkRotation("Cutiemark Rotation", Range( 0 , 360)) = 270
		_CutiemarksUVs("Cutiemarks UVs", Vector) = (12,12,-0.34,2.36)
		_ColorMaskTex("ColorMaskTex", 2D) = "white" {}
		[Toggle(_USEADDITIONALTEXTURE_ON)] _UseAdditionalTexture("Use Additional Texture", Float) = 0
		_ColorMaskAdditionalTex("ColorMaskAdditionalTex", 2D) = "white" {}
		_Color1("Color 1", Color) = (0.7,0.7,0.7,1)
		_Color2("Color 2", Color) = (0.7,0.7,0.7,1)
		_Color3("Color 3", Color) = (0.7,0.7,0.7,1)
		_Color4("Color 4", Color) = (0.7,0.7,0.7,1)
		_Color5("Color 5", Color) = (0.7,0.7,0.7,1)
		_Color6("Color 6", Color) = (0.7,0.7,0.7,1)
		_CutieMarkZoneMask("CutieMarkZoneMask", 2D) = "white" {}
		[Toggle(_DISABLEMASK_ON)] _DisableMask("DisableMask", Float) = 0
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ }
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
			float3 temp_output_155_0 = (_OutlineColor).rgb;
			float3 IndirDiffLight142 = float3(0,0,0);
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float temp_output_143_0 = ( 1.0 - ( ( 1.0 - 1 ) * _WorldSpaceLightPos0.w ) );
			half3 ase_worldNormal = WorldNormalVector( i, half3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult97 = dot( ase_worldNormal , ase_worldlightDir );
			float NdotL109 = dotResult97;
			float lerpResult148 = lerp( temp_output_143_0 , ( saturate( ( ( NdotL109 + 0.0 ) / 0.001 ) ) * 1 ) , _ShadowValue);
			float2 uv_ColorMaskTex = i.uv_texcoord * _ColorMaskTex_ST.xy + _ColorMaskTex_ST.zw;
			float3 break13 = (tex2D( _ColorMaskTex, uv_ColorMaskTex )).rgb;
			float2 uv_ColorMaskAdditionalTex = i.uv_texcoord * _ColorMaskAdditionalTex_ST.xy + _ColorMaskAdditionalTex_ST.zw;
			#ifdef _USEADDITIONALTEXTURE_ON
				float4 staticSwitch6 = tex2D( _ColorMaskAdditionalTex, uv_ColorMaskAdditionalTex );
			#else
				float4 staticSwitch6 = half4( half3(0,0,0) , 0.0 );
			#endif
			float3 break43 = (staticSwitch6).rgb;
			float2 uv_AlphaTex = i.uv_texcoord * _AlphaTex_ST.xy + _AlphaTex_ST.zw;
			half4 tex2DNode59 = tex2D( _AlphaTex, uv_AlphaTex );
			float4 AlphaTexVar72 = tex2DNode59;
			#ifdef _DISABLEMASK_ON
				float4 staticSwitch181 = AlphaTexVar72;
			#else
				float4 staticSwitch181 = ( ( ( _Color1 * break13.x ) + ( _Color2 * break13.y ) + ( _Color3 * break13.z ) + ( _Color4 * break43.x ) + ( _Color5 * break43.y ) + ( _Color6 * break43.z ) ) * AlphaTexVar72 );
			#endif
			float4 ColorMaskTextureOutput93 = staticSwitch181;
			half4 _Color = half4(0.7,0.7,0.7,1);
			float4 ColorVar116 = _Color;
			half2 temp_cast_4 = (0.5).xx;
			float2 appendResult22 = (half2(_CutiemarksUVs.x , _CutiemarksUVs.y));
			float2 appendResult36 = (half2(_CutiemarksUVs.z , _CutiemarksUVs.w));
			half2 UV60 = ( ( ( i.uv_texcoord - temp_cast_4 ) * appendResult22 ) + appendResult36 );
			half Angle60 = radians( _CutiemarkRotation );
			half2 localRotateUV60 = RotateUV60( UV60 , Angle60 );
			half2 CutiemarksUVs65 = localRotateUV60;
			half4 CutieMarkTexVar82 = tex2D( _CutieMarkTex, CutiemarksUVs65 );
			float temp_output_94_0 = (CutieMarkTexVar82).a;
			half CMZoneAlpha83 = tex2D( _CutieMarkZoneMask, CutiemarksUVs65 ).r;
			half ifLocalVar118 = 0;
			if( temp_output_94_0 == 1.0 )
				ifLocalVar118 = ( temp_output_94_0 - CMZoneAlpha83 );
			else if( temp_output_94_0 < 1.0 )
				ifLocalVar118 = temp_output_94_0;
			float4 lerpResult137 = lerp( ( ColorMaskTextureOutput93 * ColorVar116 ) , ( ( half4( half3(0.7,0.7,0.7) , 0.0 ) * CutieMarkTexVar82 ) * saturate( ( ColorMaskTextureOutput93 + _Cutiemarkmode ) ) ) , ifLocalVar118);
			float4 switchResult141 = (((i.ASEVFace>0)?(lerpResult137):(ColorMaskTextureOutput93)));
			half4 CombinedTexture149 = switchResult141;
			float3 BaseColor160 = ( ( ( IndirDiffLight142 * ase_lightColor.a * temp_output_143_0 ) + ( ase_lightColor.rgb * lerpResult148 ) ) * (CombinedTexture149).rgb );
			o.Emission = lerp(half4( temp_output_155_0 , 0.0 ),( CalculateContrast(_OutlineBrightness,half4( temp_output_155_0 , 0.0 )) * half4( BaseColor160 , 0.0 ) ),_OutlineSwitch).rgb;
			o.Normal = float3(0,0,-1);
		}
		ENDCG
		

		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "Geometry+0" }
		Cull Off
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature _DISABLEMASK_ON
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
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldPos;
			half2 uv_texcoord;
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

		uniform half _ShadowValue;
		uniform half4 _Color1;
		uniform sampler2D _ColorMaskTex;
		uniform half4 _ColorMaskTex_ST;
		uniform half4 _Color2;
		uniform half4 _Color3;
		uniform half4 _Color4;
		uniform sampler2D _ColorMaskAdditionalTex;
		uniform half4 _ColorMaskAdditionalTex_ST;
		uniform half4 _Color5;
		uniform half4 _Color6;
		uniform sampler2D _AlphaTex;
		uniform half4 _AlphaTex_ST;
		uniform sampler2D _CutieMarkTex;
		uniform half4 _CutiemarksUVs;
		uniform half _CutiemarkRotation;
		uniform half _Cutiemarkmode;
		uniform sampler2D _CutieMarkZoneMask;
		uniform half _AlphaCutout;
		uniform half _OutlineWidth;
		uniform half _OutlineSwitch;
		uniform half4 _OutlineColor;
		uniform half _OutlineBrightness;


		inline half2 RotateUV60( half2 UV , half Angle )
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
			float3 OutlineVar168 = 0;
			v.vertex.xyz += OutlineVar168;
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
			float2 uv_AlphaTex = i.uv_texcoord * _AlphaTex_ST.xy + _AlphaTex_ST.zw;
			half4 tex2DNode59 = tex2D( _AlphaTex, uv_AlphaTex );
			half4 _Color = half4(0.7,0.7,0.7,1);
			float AlphaValue175 = ( tex2DNode59.a * _Color.a );
			float AlphaClipVar173 = saturate( ( (0.0 + (_AlphaCutout - 1.0) * (1.0 - 0.0) / (0.0 - 1.0)) + AlphaValue175 ) );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float3 LightColorData120 = ( ase_lightColor.rgb * ase_lightAtten );
			UnityGI gi135 = gi;
			float3 diffNorm135 = LightColorData120;
			gi135 = UnityGI_Base( data, 1, diffNorm135 );
			float3 indirectDiffuse135 = gi135.indirect.diffuse + diffNorm135 * 0.0001;
			float3 IndirDiffLight142 = indirectDiffuse135;
			float temp_output_143_0 = ( 1.0 - ( ( 1.0 - ase_lightAtten ) * _WorldSpaceLightPos0.w ) );
			half3 ase_worldNormal = WorldNormalVector( i, half3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult97 = dot( ase_worldNormal , ase_worldlightDir );
			float NdotL109 = dotResult97;
			float lerpResult148 = lerp( temp_output_143_0 , ( saturate( ( ( NdotL109 + 0.0 ) / 0.001 ) ) * ase_lightAtten ) , _ShadowValue);
			float2 uv_ColorMaskTex = i.uv_texcoord * _ColorMaskTex_ST.xy + _ColorMaskTex_ST.zw;
			float3 break13 = (tex2D( _ColorMaskTex, uv_ColorMaskTex )).rgb;
			float2 uv_ColorMaskAdditionalTex = i.uv_texcoord * _ColorMaskAdditionalTex_ST.xy + _ColorMaskAdditionalTex_ST.zw;
			#ifdef _USEADDITIONALTEXTURE_ON
				float4 staticSwitch6 = tex2D( _ColorMaskAdditionalTex, uv_ColorMaskAdditionalTex );
			#else
				float4 staticSwitch6 = half4( half3(0,0,0) , 0.0 );
			#endif
			float3 break43 = (staticSwitch6).rgb;
			float4 AlphaTexVar72 = tex2DNode59;
			#ifdef _DISABLEMASK_ON
				float4 staticSwitch181 = AlphaTexVar72;
			#else
				float4 staticSwitch181 = ( ( ( _Color1 * break13.x ) + ( _Color2 * break13.y ) + ( _Color3 * break13.z ) + ( _Color4 * break43.x ) + ( _Color5 * break43.y ) + ( _Color6 * break43.z ) ) * AlphaTexVar72 );
			#endif
			float4 ColorMaskTextureOutput93 = staticSwitch181;
			float4 ColorVar116 = _Color;
			half2 temp_cast_5 = (0.5).xx;
			float2 appendResult22 = (half2(_CutiemarksUVs.x , _CutiemarksUVs.y));
			float2 appendResult36 = (half2(_CutiemarksUVs.z , _CutiemarksUVs.w));
			half2 UV60 = ( ( ( i.uv_texcoord - temp_cast_5 ) * appendResult22 ) + appendResult36 );
			half Angle60 = radians( _CutiemarkRotation );
			half2 localRotateUV60 = RotateUV60( UV60 , Angle60 );
			half2 CutiemarksUVs65 = localRotateUV60;
			half4 CutieMarkTexVar82 = tex2D( _CutieMarkTex, CutiemarksUVs65 );
			float temp_output_94_0 = (CutieMarkTexVar82).a;
			half CMZoneAlpha83 = tex2D( _CutieMarkZoneMask, CutiemarksUVs65 ).r;
			half ifLocalVar118 = 0;
			if( temp_output_94_0 == 1.0 )
				ifLocalVar118 = ( temp_output_94_0 - CMZoneAlpha83 );
			else if( temp_output_94_0 < 1.0 )
				ifLocalVar118 = temp_output_94_0;
			float4 lerpResult137 = lerp( ( ColorMaskTextureOutput93 * ColorVar116 ) , ( ( half4( half3(0.7,0.7,0.7) , 0.0 ) * CutieMarkTexVar82 ) * saturate( ( ColorMaskTextureOutput93 + _Cutiemarkmode ) ) ) , ifLocalVar118);
			float4 switchResult141 = (((i.ASEVFace>0)?(lerpResult137):(ColorMaskTextureOutput93)));
			half4 CombinedTexture149 = switchResult141;
			float3 BaseColor160 = ( ( ( IndirDiffLight142 * ase_lightColor.a * temp_output_143_0 ) + ( ase_lightColor.rgb * lerpResult148 ) ) * (CombinedTexture149).rgb );
			float3 temp_output_180_0 = BaseColor160;
			c.rgb = temp_output_180_0;
			c.a = 1;
			clip( AlphaClipVar173 - _AlphaCutout );
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
			float3 IndirDiffLight142 = float3(0,0,0);
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float temp_output_143_0 = ( 1.0 - ( ( 1.0 - 1 ) * _WorldSpaceLightPos0.w ) );
			half3 ase_worldNormal = WorldNormalVector( i, half3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult97 = dot( ase_worldNormal , ase_worldlightDir );
			float NdotL109 = dotResult97;
			float lerpResult148 = lerp( temp_output_143_0 , ( saturate( ( ( NdotL109 + 0.0 ) / 0.001 ) ) * 1 ) , _ShadowValue);
			float2 uv_ColorMaskTex = i.uv_texcoord * _ColorMaskTex_ST.xy + _ColorMaskTex_ST.zw;
			float3 break13 = (tex2D( _ColorMaskTex, uv_ColorMaskTex )).rgb;
			float2 uv_ColorMaskAdditionalTex = i.uv_texcoord * _ColorMaskAdditionalTex_ST.xy + _ColorMaskAdditionalTex_ST.zw;
			#ifdef _USEADDITIONALTEXTURE_ON
				float4 staticSwitch6 = tex2D( _ColorMaskAdditionalTex, uv_ColorMaskAdditionalTex );
			#else
				float4 staticSwitch6 = half4( half3(0,0,0) , 0.0 );
			#endif
			float3 break43 = (staticSwitch6).rgb;
			float2 uv_AlphaTex = i.uv_texcoord * _AlphaTex_ST.xy + _AlphaTex_ST.zw;
			half4 tex2DNode59 = tex2D( _AlphaTex, uv_AlphaTex );
			float4 AlphaTexVar72 = tex2DNode59;
			#ifdef _DISABLEMASK_ON
				float4 staticSwitch181 = AlphaTexVar72;
			#else
				float4 staticSwitch181 = ( ( ( _Color1 * break13.x ) + ( _Color2 * break13.y ) + ( _Color3 * break13.z ) + ( _Color4 * break43.x ) + ( _Color5 * break43.y ) + ( _Color6 * break43.z ) ) * AlphaTexVar72 );
			#endif
			float4 ColorMaskTextureOutput93 = staticSwitch181;
			half4 _Color = half4(0.7,0.7,0.7,1);
			float4 ColorVar116 = _Color;
			half2 temp_cast_2 = (0.5).xx;
			float2 appendResult22 = (half2(_CutiemarksUVs.x , _CutiemarksUVs.y));
			float2 appendResult36 = (half2(_CutiemarksUVs.z , _CutiemarksUVs.w));
			half2 UV60 = ( ( ( i.uv_texcoord - temp_cast_2 ) * appendResult22 ) + appendResult36 );
			half Angle60 = radians( _CutiemarkRotation );
			half2 localRotateUV60 = RotateUV60( UV60 , Angle60 );
			half2 CutiemarksUVs65 = localRotateUV60;
			half4 CutieMarkTexVar82 = tex2D( _CutieMarkTex, CutiemarksUVs65 );
			float temp_output_94_0 = (CutieMarkTexVar82).a;
			half CMZoneAlpha83 = tex2D( _CutieMarkZoneMask, CutiemarksUVs65 ).r;
			half ifLocalVar118 = 0;
			if( temp_output_94_0 == 1.0 )
				ifLocalVar118 = ( temp_output_94_0 - CMZoneAlpha83 );
			else if( temp_output_94_0 < 1.0 )
				ifLocalVar118 = temp_output_94_0;
			float4 lerpResult137 = lerp( ( ColorMaskTextureOutput93 * ColorVar116 ) , ( ( half4( half3(0.7,0.7,0.7) , 0.0 ) * CutieMarkTexVar82 ) * saturate( ( ColorMaskTextureOutput93 + _Cutiemarkmode ) ) ) , ifLocalVar118);
			float4 switchResult141 = (((i.ASEVFace>0)?(lerpResult137):(ColorMaskTextureOutput93)));
			half4 CombinedTexture149 = switchResult141;
			float3 BaseColor160 = ( ( ( IndirDiffLight142 * ase_lightColor.a * temp_output_143_0 ) + ( ase_lightColor.rgb * lerpResult148 ) ) * (CombinedTexture149).rgb );
			float3 temp_output_180_0 = BaseColor160;
			o.Albedo = temp_output_180_0;
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
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
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
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Cel Shading/OutlineV2"
}
