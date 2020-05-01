Shader "Cel Shading/RegularV3"
{
	Properties
	{
		_MainTex("MainTex", 2D) = "white" {}
		_OcclusionMap("OcclusionMap", 2D) = "black" {}
		[Toggle(_EMISSIONTOGGLE_ON)] _EmissionToggle("EmissionToggle", Float) = 0
		_EmissionMap("EmissionMap", 2D) = "black" {}
		_EmissionFactor("EmissionFactor", Color) = (0,0,0,0)
		_DirtynessMap("DirtynessMap", 2D) = "black" {}
		_ShadowValue("Shadow Value", Range( 0 , 1)) = 0.15
		_DirtynessVal("DirtynessVal", Range( 0 , 1)) = 0
		_Ref("Ref", Range( 0 , 255)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		LOD 200
		Cull Off
		Stencil
		{
			Ref [_Ref]
			CompFront NotEqual
			PassFront Zero
		}
		Blend One Zero , SrcAlpha OneMinusSrcAlpha
		BlendOp Add , Add
		AlphaToMask On
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature _EMISSIONTOGGLE_ON
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

		uniform half _Ref;
		uniform float _ShadowValue;
		uniform sampler2D _MainTex;
		uniform half4 _MainTex_ST;
		uniform sampler2D _DirtynessMap;
		uniform half4 _DirtynessMap_ST;
		uniform float _DirtynessVal;
		uniform sampler2D _OcclusionMap;
		uniform half4 _OcclusionMap_ST;
		uniform sampler2D _EmissionMap;
		uniform half4 _EmissionMap_ST;
		uniform half4 _EmissionFactor;

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
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			half4 tex2DNode16 = tex2D( _MainTex, uv_MainTex );
			half AlphaValue98 = tex2DNode16.a;
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			half4 ase_lightColor = 0;
			#else //aselc
			half4 ase_lightColor = _LightColor0;
			#endif //aselc
			half3 LightColorData52 = ( ase_lightColor.rgb * ase_lightAtten );
			UnityGI gi153 = gi;
			float3 diffNorm153 = LightColorData52;
			gi153 = UnityGI_Base( data, 1, diffNorm153 );
			half3 indirectDiffuse153 = gi153.indirect.diffuse + diffNorm153 * 0.0001;
			half3 IndirDiffLight74 = indirectDiffuse153;
			half temp_output_73_0 = ( 1.0 - ( ( 1.0 - ase_lightAtten ) * _WorldSpaceLightPos0.w ) );
			half3 ase_worldNormal = WorldNormalVector( i, half3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			half3 ase_worldlightDir = 0;
			#else //aseld
			half3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			half dotResult39 = dot( ase_worldNormal , ase_worldlightDir );
			half NdotL41 = dotResult39;
			half lerpResult79 = lerp( temp_output_73_0 , ( saturate( ( NdotL41 / 0.001 ) ) * ase_lightAtten ) , _ShadowValue);
			half3 temp_output_86_0 = ( ( IndirDiffLight74 * ase_lightColor.a * temp_output_73_0 ) + ( ase_lightColor.rgb * lerpResult79 ) );
			half3 InputColor24 = (( tex2DNode16 * half4(0.7,0.7,0.7,1) )).rgb;
			float2 uv_DirtynessMap = i.uv_texcoord * _DirtynessMap_ST.xy + _DirtynessMap_ST.zw;
			half4 tex2DNode26 = tex2D( _DirtynessMap, uv_DirtynessMap );
			half ifLocalVar43 = 0;
			if( tex2DNode26.a > ( 1.0 - _DirtynessVal ) )
				ifLocalVar43 = tex2DNode26.a;
			half3 lerpResult67 = lerp( ( InputColor24 * float3( 0.7,0.7,0.7 ) ) , ( (( tex2DNode26 * float4( 0.7,0.7,0.7,1 ) )).rgb * saturate( ( InputColor24 + float3( 1,1,1 ) ) ) ) , ifLocalVar43);
			half3 switchResult68 = (((i.ASEVFace>0)?(lerpResult67):(InputColor24)));
			half3 CombinedTexture110 = switchResult68;
			float2 uv_OcclusionMap = i.uv_texcoord * _OcclusionMap_ST.xy + _OcclusionMap_ST.zw;
			half3 temp_output_83_0 = (tex2D( _OcclusionMap, uv_OcclusionMap )).rgb;
			half temp_output_7_0_g24 = ( ( i.uv_texcoord.y + i.uv_texcoord.x ) - ( _Time.y * 0.2 ) );
			half ShineVar84 = ( ( ase_lightColor.a * 0.3 ) * step( frac( pow( ( 1.0 + 2.18 ) , sin( temp_output_7_0_g24 ) ) ) , 0.08 ) );
			half3 BaseColorOutput99 = ( ( temp_output_86_0 * ( CombinedTexture110 * ( 1.0 - temp_output_83_0 ) ) ) + ( ( temp_output_86_0 * ( CombinedTexture110 * temp_output_83_0 ) ) + ( temp_output_83_0 * ShineVar84 ) ) );
			half3 temp_output_102_0 = BaseColorOutput99;
			c.rgb = temp_output_102_0;
			c.a = AlphaValue98;
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
			half3 IndirDiffLight74 = float3(0,0,0);
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			half4 ase_lightColor = 0;
			#else //aselc
			half4 ase_lightColor = _LightColor0;
			#endif //aselc
			half temp_output_73_0 = ( 1.0 - ( ( 1.0 - 1 ) * _WorldSpaceLightPos0.w ) );
			half3 ase_worldNormal = WorldNormalVector( i, half3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			half3 ase_worldlightDir = 0;
			#else //aseld
			half3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			half dotResult39 = dot( ase_worldNormal , ase_worldlightDir );
			half NdotL41 = dotResult39;
			half lerpResult79 = lerp( temp_output_73_0 , ( saturate( ( NdotL41 / 0.001 ) ) * 1 ) , _ShadowValue);
			half3 temp_output_86_0 = ( ( IndirDiffLight74 * ase_lightColor.a * temp_output_73_0 ) + ( ase_lightColor.rgb * lerpResult79 ) );
			float2 uv_MainTex = i.uv_texcoord * _MainTex_ST.xy + _MainTex_ST.zw;
			half4 tex2DNode16 = tex2D( _MainTex, uv_MainTex );
			half3 InputColor24 = (( tex2DNode16 * half4(0.7,0.7,0.7,1) )).rgb;
			float2 uv_DirtynessMap = i.uv_texcoord * _DirtynessMap_ST.xy + _DirtynessMap_ST.zw;
			half4 tex2DNode26 = tex2D( _DirtynessMap, uv_DirtynessMap );
			half ifLocalVar43 = 0;
			if( tex2DNode26.a > ( 1.0 - _DirtynessVal ) )
				ifLocalVar43 = tex2DNode26.a;
			half3 lerpResult67 = lerp( ( InputColor24 * float3( 0.7,0.7,0.7 ) ) , ( (( tex2DNode26 * float4( 0.7,0.7,0.7,1 ) )).rgb * saturate( ( InputColor24 + float3( 1,1,1 ) ) ) ) , ifLocalVar43);
			half3 switchResult68 = (((i.ASEVFace>0)?(lerpResult67):(InputColor24)));
			half3 CombinedTexture110 = switchResult68;
			float2 uv_OcclusionMap = i.uv_texcoord * _OcclusionMap_ST.xy + _OcclusionMap_ST.zw;
			half3 temp_output_83_0 = (tex2D( _OcclusionMap, uv_OcclusionMap )).rgb;
			half temp_output_7_0_g24 = ( ( i.uv_texcoord.y + i.uv_texcoord.x ) - ( _Time.y * 0.2 ) );
			half ShineVar84 = ( ( ase_lightColor.a * 0.3 ) * step( frac( pow( ( 1.0 + 2.18 ) , sin( temp_output_7_0_g24 ) ) ) , 0.08 ) );
			half3 BaseColorOutput99 = ( ( temp_output_86_0 * ( CombinedTexture110 * ( 1.0 - temp_output_83_0 ) ) ) + ( ( temp_output_86_0 * ( CombinedTexture110 * temp_output_83_0 ) ) + ( temp_output_83_0 * ShineVar84 ) ) );
			half3 temp_output_102_0 = BaseColorOutput99;
			o.Albedo = temp_output_102_0;
			float2 uv_EmissionMap = i.uv_texcoord * _EmissionMap_ST.xy + _EmissionMap_ST.zw;
			#ifdef _EMISSIONTOGGLE_ON
				float3 staticSwitch105 = (tex2D( _EmissionMap, uv_EmissionMap )).rgb;
			#else
				float3 staticSwitch105 = half3(0,0,0);
			#endif
			o.Emission = ( staticSwitch105 * ( (_EmissionFactor).rgb * float3( 10,10,10 ) ) );
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			AlphaToMask Off
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
			sampler3D _DitherMaskLOD;
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
				half alphaRef = tex3D( _DitherMaskLOD, float3( vpos.xy * 0.25, o.Alpha * 0.9375 ) ).a;
				clip( alphaRef - 0.01 );
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
}
