Shader "Cel Shading/PonyShaderV3Head"
{
	Properties
	{
		_AlphaTex("AlphaTex", 2D) = "gray" {}
		_ShadowValue("Shadow Value", Range( 0 , 1)) = 0.15
		_OutlineColor("OutlineColor", Color) = (1,1,1,1)
		_OutlineBrightness("OutlineBrightness", Range( 0 , 2)) = 0.7
		_OutlineWidth("OutlineWidth", Range( 0 , 0.4)) = 0
		[Toggle]_OutlineSwitch("Texture Based Outline Switch", Float) = 1
		_ColorMaskTex("ColorMaskTex", 2D) = "white" {}
		_Color1("Color 1", Color) = (0.7,0.7,0.7,1)
		_Color2("Color 2", Color) = (0.7,0.7,0.7,1)
		_Color3("Color 3", Color) = (0.7,0.7,0.7,1)
		_OutlineMask("OutlineMask", 2D) = "white" {}
		_MaskClipOutlineValue("MaskClipOutlineValue", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0"}
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
			float3 IndirDiffLight63 = float3(0,0,0);
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float temp_output_67_0 = ( 1.0 - ( ( 1.0 - 1 ) * _WorldSpaceLightPos0.w ) );
			half3 ase_worldNormal = WorldNormalVector( i, half3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult26 = dot( ase_worldNormal , ase_worldlightDir );
			float NdotL32 = dotResult26;
			float lerpResult71 = lerp( temp_output_67_0 , ( saturate( ( ( NdotL32 + 0.0 ) / 0.001 ) ) * 1 ) , _ShadowValue);
			float2 uv_ColorMaskTex = i.uv_texcoord * _ColorMaskTex_ST.xy + _ColorMaskTex_ST.zw;
			float3 break17 = (tex2D( _ColorMaskTex, uv_ColorMaskTex )).rgb;
			float2 uv_AlphaTex = i.uv_texcoord * _AlphaTex_ST.xy + _AlphaTex_ST.zw;
			half4 tex2DNode37 = tex2D( _AlphaTex, uv_AlphaTex );
			float4 AlphaTexVar47 = tex2DNode37;
			half4 ColorMaskTextureOutput64 = ( ( ( _Color1 * break17.x ) + ( _Color2 * break17.y ) + ( _Color3 * break17.z ) ) + AlphaTexVar47 );
			float3 BaseColor80 = ( ( ( IndirDiffLight63 * ase_lightColor.a * temp_output_67_0 ) + ( ase_lightColor.rgb * lerpResult71 ) ) * (( ColorMaskTextureOutput64 * float4( 0.7,0.7,0.7,1 ) )).rgb );
			float2 uv_OutlineMask = i.uv_texcoord * _OutlineMask_ST.xy + _OutlineMask_ST.zw;
			o.Emission = lerp(_OutlineColor,( CalculateContrast(_OutlineBrightness,_OutlineColor) * half4( BaseColor80 , 0.0 ) ),_OutlineSwitch).rgb;
			clip( ( 1.0 - tex2D( _OutlineMask, uv_OutlineMask ).r ) - _MaskClipOutlineValue );
			o.Normal = float3(0,0,-1);
		}
		ENDCG
		

		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" }
		LOD 200
		Cull Back
		ZWrite On
		ZTest LEqual
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 4.6
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
		uniform sampler2D _AlphaTex;
		uniform half4 _AlphaTex_ST;
		uniform half _OutlineWidth;
		uniform half _OutlineSwitch;
		uniform half4 _OutlineColor;
		uniform half _OutlineBrightness;
		uniform sampler2D _OutlineMask;
		uniform half4 _OutlineMask_ST;
		uniform half _MaskClipOutlineValue;


		float4 CalculateContrast( float contrastValue, float4 colorTarget )
		{
			float t = 0.5 * ( 1.0 - contrastValue );
			return mul( float4x4( contrastValue,0,0,t, 0,contrastValue,0,t, 0,0,contrastValue,t, 0,0,0,1 ), colorTarget );
		}

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 OutlineVar87 = 0;
			v.vertex.xyz += OutlineVar87;
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
			float3 LightColorData48 = ( ase_lightColor.rgb * ase_lightAtten );
			UnityGI gi62 = gi;
			float3 diffNorm62 = LightColorData48;
			gi62 = UnityGI_Base( data, 1, diffNorm62 );
			float3 indirectDiffuse62 = gi62.indirect.diffuse + diffNorm62 * 0.0001;
			float3 IndirDiffLight63 = indirectDiffuse62;
			float temp_output_67_0 = ( 1.0 - ( ( 1.0 - ase_lightAtten ) * _WorldSpaceLightPos0.w ) );
			half3 ase_worldNormal = WorldNormalVector( i, half3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult26 = dot( ase_worldNormal , ase_worldlightDir );
			float NdotL32 = dotResult26;
			float lerpResult71 = lerp( temp_output_67_0 , ( saturate( ( ( NdotL32 + 0.0 ) / 0.001 ) ) * ase_lightAtten ) , _ShadowValue);
			float2 uv_ColorMaskTex = i.uv_texcoord * _ColorMaskTex_ST.xy + _ColorMaskTex_ST.zw;
			float3 break17 = (tex2D( _ColorMaskTex, uv_ColorMaskTex )).rgb;
			float2 uv_AlphaTex = i.uv_texcoord * _AlphaTex_ST.xy + _AlphaTex_ST.zw;
			half4 tex2DNode37 = tex2D( _AlphaTex, uv_AlphaTex );
			float4 AlphaTexVar47 = tex2DNode37;
			half4 ColorMaskTextureOutput64 = ( ( ( _Color1 * break17.x ) + ( _Color2 * break17.y ) + ( _Color3 * break17.z ) ) + AlphaTexVar47 );
			float3 BaseColor80 = ( ( ( IndirDiffLight63 * ase_lightColor.a * temp_output_67_0 ) + ( ase_lightColor.rgb * lerpResult71 ) ) * (( ColorMaskTextureOutput64 * float4( 0.7,0.7,0.7,1 ) )).rgb );
			float3 temp_output_90_0 = BaseColor80;
			c.rgb = temp_output_90_0;
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
			float3 IndirDiffLight63 = float3(0,0,0);
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float temp_output_67_0 = ( 1.0 - ( ( 1.0 - 1 ) * _WorldSpaceLightPos0.w ) );
			half3 ase_worldNormal = WorldNormalVector( i, half3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult26 = dot( ase_worldNormal , ase_worldlightDir );
			float NdotL32 = dotResult26;
			float lerpResult71 = lerp( temp_output_67_0 , ( saturate( ( ( NdotL32 + 0.0 ) / 0.001 ) ) * 1 ) , _ShadowValue);
			float2 uv_ColorMaskTex = i.uv_texcoord * _ColorMaskTex_ST.xy + _ColorMaskTex_ST.zw;
			float3 break17 = (tex2D( _ColorMaskTex, uv_ColorMaskTex )).rgb;
			float2 uv_AlphaTex = i.uv_texcoord * _AlphaTex_ST.xy + _AlphaTex_ST.zw;
			half4 tex2DNode37 = tex2D( _AlphaTex, uv_AlphaTex );
			float4 AlphaTexVar47 = tex2DNode37;
			half4 ColorMaskTextureOutput64 = ( ( ( _Color1 * break17.x ) + ( _Color2 * break17.y ) + ( _Color3 * break17.z ) ) + AlphaTexVar47 );
			float3 BaseColor80 = ( ( ( IndirDiffLight63 * ase_lightColor.a * temp_output_67_0 ) + ( ase_lightColor.rgb * lerpResult71 ) ) * (( ColorMaskTextureOutput64 * float4( 0.7,0.7,0.7,1 ) )).rgb );
			float3 temp_output_90_0 = BaseColor80;
			o.Albedo = temp_output_90_0;
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
			#pragma target 4.6
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