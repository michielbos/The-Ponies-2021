Shader "Cel Shading/WaterV2"
{
	Properties
	{
		_ColorFoam("Color Foam", Color) = (0,0.7803922,1,1)
		_ColorDepth("Color Depth", Color) = (0,0.6666667,1,1)
		_Depth("Depth", Range( 0.001 , 2)) = 0.2
		_OpacityIntensity("OpacityIntensity", Range( 0 , 1)) = 0.9
		[Toggle]_Worldspacetiling("Worldspace tiling", Float) = 1
		_WavesSpeed("Waves Speed", Range( 0 , 10)) = 0.3
		_WaveDirectionX("WaveDirectionX", Range( -10 , 10)) = 2
		_WaveDirectionY("WaveDirectionY", Range( -10 , 10)) = 2
		_WaveHeight("Wave Height", Range( 0 , 1)) = 0.05
		_WaveSize("Wave Size", Range( 0 , 10)) = 10
		_ReflectionStrength("Reflection Strength", Range( 0 , 1)) = 0
		_Shadermap("Shadermap", 2D) = "black" {}
		[Toggle(_TOGGLEORTHO_ON)] _ToggleOrtho("ToggleOrtho", Float) = 1
		_FoamSmoothstep("FoamSmoothstep", Range( 0 , 2)) = 1.5
		[PerRendererData]_ReflectionTex("_ReflectionTex", 2D) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		LOD 200
		Cull Off
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _TOGGLEORTHO_ON
		struct Input
		{
			float3 worldPos;
			float4 screenPosition186;
			float4 screenPos;
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

		uniform half _WaveHeight;
		uniform sampler2D _Shadermap;
		uniform half _Worldspacetiling;
		uniform float _WaveSize;
		uniform half _WavesSpeed;
		uniform half _WaveDirectionX;
		uniform half _WaveDirectionY;
		uniform float4 _ColorFoam;
		uniform float4 _ColorDepth;
		uniform half _FoamSmoothstep;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform float _Depth;
		uniform sampler2D _ReflectionTex;
		uniform half _OpacityIntensity;
		uniform float _ReflectionStrength;


		inline int ComparisonGreater169( half A , half B )
		{
			return A > B ? 1 : 0;;
		}


		inline float4 ASE_ComputeGrabScreenPos( float4 pos )
		{
			#if UNITY_UV_STARTS_AT_TOP
			float scale = -1.0;
			#else
			float scale = 1.0;
			#endif
			float4 o = pos;
			o.y = pos.w * 0.5f;
			o.y = ( pos.y - o.y ) * _ProjectionParams.x * scale + o.y;
			return o;
		}


		float2 SPSRUVAdjust268( float4 input )
		{
			return UnityStereoScreenSpaceUVAdjust(input, float4(1, 1, 0, 0));
		}


		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			half3 ase_vertexNormal = v.normal.xyz;
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			half2 Tiling88 = (( _Worldspacetiling )?( ( (ase_worldPos).xz * float2( 0.1,0.1 ) ) ):( ( -20.0 * v.texcoord.xy ) ));
			half2 appendResult65 = (half2(_WaveDirectionX , _WaveDirectionY));
			half2 WaveSpeed69 = ( ( _WavesSpeed * _Time.x ) * appendResult65 );
			float2 HeightmapUV98 = ( ( ( Tiling88 * _WaveSize ) * float2( 0.1,0.1 ) ) + ( WaveSpeed69 * float2( 0.5,0.5 ) ) );
			half4 tex2DNode115 = tex2Dlod( _Shadermap, float4( HeightmapUV98, 0, 1.0) );
			v.vertex.xyz += ( ase_vertexNormal * ( _WaveHeight * tex2DNode115.g ) );
			float3 ase_vertex3Pos = v.vertex.xyz;
			half3 vertexPos186 = ase_vertex3Pos;
			float4 ase_screenPos186 = ComputeScreenPos( UnityObjectToClipPos( vertexPos186 ) );
			o.screenPosition186 = ase_screenPos186;
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
			float4 ase_screenPos186 = i.screenPosition186;
			half4 ase_screenPosNorm186 = ase_screenPos186 / ase_screenPos186.w;
			ase_screenPosNorm186.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm186.z : ase_screenPosNorm186.z * 0.5 + 0.5;
			float screenDepth186 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm186.xy ));
			half distanceDepth186 = abs( ( screenDepth186 - LinearEyeDepth( ase_screenPosNorm186.z ) ) / ( _Depth ) );
			half A169 = _ProjectionParams.x;
			half B169 = 0.0;
			int localComparisonGreater169 = ComparisonGreater169( A169 , B169 );
			half lerpResult167 = lerp( distanceDepth186 , ( 1.0 - distanceDepth186 ) , (float)localComparisonGreater169);
			half lerpResult197 = lerp( _ProjectionParams.y , _ProjectionParams.z , lerpResult167);
			#ifdef _TOGGLEORTHO_ON
				half staticSwitch206 = lerpResult197;
			#else
				half staticSwitch206 = distanceDepth186;
			#endif
			half smoothstepResult252 = smoothstep( _FoamSmoothstep , 2.0 , staticSwitch206);
			half lerpResult255 = lerp( 1.0 , _OpacityIntensity , smoothstepResult252);
			half clampResult39 = clamp( lerpResult255 , 0.0 , 1.0 );
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			half4 ase_lightColor = 0;
			#else //aselc
			half4 ase_lightColor = _LightColor0;
			#endif //aselc
			half4 lerpResult30 = lerp( _ColorFoam , _ColorDepth , smoothstepResult252);
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			half4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float4 input268 = ase_grabScreenPosNorm;
			float2 localSPSRUVAdjust268 = SPSRUVAdjust268( input268 );
			float4 Reflection274 = tex2D( _ReflectionTex, localSPSRUVAdjust268 );
			half4 lerpResult297 = lerp( lerpResult30 , Reflection274 , ( 1.0 - lerpResult255 ));
			half4 lerpResult292 = lerp( lerpResult30 , Reflection274 , saturate( ( lerpResult255 * _ReflectionStrength ) ));
			half3 temp_output_152_0 = ( ( ase_lightColor.rgb * ( ase_lightColor.a * ( 1.0 - ( ( 1.0 - ase_lightAtten ) * _WorldSpaceLightPos0.w ) ) ) ) * ( (( lerpResult297 * lerpResult292 )).rgb * float3( 0.7,0.7,0.7 ) ) );
			c.rgb = temp_output_152_0;
			c.a = clampResult39;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			#if defined(LIGHTMAP_ON) && ( UNITY_VERSION < 560 || ( defined(LIGHTMAP_SHADOW_MIXING) && !defined(SHADOWS_SHADOWMASK) && defined(SHADOWS_SCREEN) ) )//aselc
			half4 ase_lightColor = 0;
			#else //aselc
			half4 ase_lightColor = _LightColor0;
			#endif //aselc
			float4 ase_screenPos186 = i.screenPosition186;
			half4 ase_screenPosNorm186 = ase_screenPos186 / ase_screenPos186.w;
			ase_screenPosNorm186.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm186.z : ase_screenPosNorm186.z * 0.5 + 0.5;
			float screenDepth186 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE( _CameraDepthTexture, ase_screenPosNorm186.xy ));
			half distanceDepth186 = abs( ( screenDepth186 - LinearEyeDepth( ase_screenPosNorm186.z ) ) / ( _Depth ) );
			half A169 = _ProjectionParams.x;
			half B169 = 0.0;
			int localComparisonGreater169 = ComparisonGreater169( A169 , B169 );
			half lerpResult167 = lerp( distanceDepth186 , ( 1.0 - distanceDepth186 ) , (float)localComparisonGreater169);
			half lerpResult197 = lerp( _ProjectionParams.y , _ProjectionParams.z , lerpResult167);
			#ifdef _TOGGLEORTHO_ON
				half staticSwitch206 = lerpResult197;
			#else
				half staticSwitch206 = distanceDepth186;
			#endif
			half smoothstepResult252 = smoothstep( _FoamSmoothstep , 2.0 , staticSwitch206);
			half4 lerpResult30 = lerp( _ColorFoam , _ColorDepth , smoothstepResult252);
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_grabScreenPos = ASE_ComputeGrabScreenPos( ase_screenPos );
			half4 ase_grabScreenPosNorm = ase_grabScreenPos / ase_grabScreenPos.w;
			float4 input268 = ase_grabScreenPosNorm;
			float2 localSPSRUVAdjust268 = SPSRUVAdjust268( input268 );
			float4 Reflection274 = tex2D( _ReflectionTex, localSPSRUVAdjust268 );
			half lerpResult255 = lerp( 1.0 , _OpacityIntensity , smoothstepResult252);
			half4 lerpResult297 = lerp( lerpResult30 , Reflection274 , ( 1.0 - lerpResult255 ));
			half4 lerpResult292 = lerp( lerpResult30 , Reflection274 , saturate( ( lerpResult255 * _ReflectionStrength ) ));
			half3 temp_output_152_0 = ( ( ase_lightColor.rgb * ( ase_lightColor.a * ( 1.0 - ( ( 1.0 - 1 ) * _WorldSpaceLightPos0.w ) ) ) ) * ( (( lerpResult297 * lerpResult292 )).rgb * float3( 0.7,0.7,0.7 ) ) );
			o.Albedo = temp_output_152_0;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting alpha:fade keepalpha fullforwardshadows exclude_path:deferred vertex:vertexDataFunc 

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
			sampler3D _DitherMaskLOD;
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float4 customPack1 : TEXCOORD1;
				float3 worldPos : TEXCOORD2;
				float4 screenPos : TEXCOORD3;
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
				o.customPack1.xyzw = customInputData.screenPosition186;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.screenPos = ComputeScreenPos( o.pos );
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
				surfIN.screenPosition186 = IN.customPack1.xyzw;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.screenPos = IN.screenPos;
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
	CustomEditor "ASEMaterialInspector"
}