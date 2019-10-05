Shader "Cel Shading/WaterV2"
{
	Properties
	{
		_Colorwater("Color water", Color) = (0,0.7803922,1,1)
		_ColorDepth("Color Depth", Color) = (0,0.1921569,1,1)
		_ColorFoam("Color Foam", Color) = (0,0.6323466,1,1)
		_Depth("Depth", Float) = 15
		_OpasityIntensity("OpasityIntensity", Range( 0 , 1)) = 0.3
		_SizeFoam("SizeFoam", Float) = 0.4
		[Toggle]_Worldspacetiling("Worldspace tiling", Float) = 1
		_Wavesspeed("Waves speed", Range( 0 , 10)) = 0.3
		_WaveDirectionX("WaveDirectionX", Range( -10 , 10)) = 2
		_WaveDirectionY("WaveDirectionY", Range( -10 , 10)) = 2
		_WaveHeight("Wave Height", Range( 0 , 1)) = 0.05
		_WaveSize("Wave Size", Range( 0 , 10)) = 10
		_Shadermap("Shadermap", 2D) = "black" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Transparent"  "Queue" = "Transparent+0" "IgnoreProjector" = "True" }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 2.0
		struct Input
		{
			float3 worldPos;
			half4 screenPosition13;
			float3 worldNormal;
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
		uniform half _WaveSize;
		uniform half _Wavesspeed;
		uniform half _WaveDirectionX;
		uniform half _WaveDirectionY;
		UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
		uniform float4 _CameraDepthTexture_TexelSize;
		uniform half _SizeFoam;
		uniform half4 _Colorwater;
		uniform half4 _ColorDepth;
		uniform half _Depth;
		uniform half4 _ColorFoam;
		uniform half _OpasityIntensity;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			float3 ase_vertexNormal = v.normal.xyz;
			float3 ase_worldPos = mul( unity_ObjectToWorld, v.vertex );
			half2 Tiling88 = lerp(( -20.0 * v.texcoord.xy ),( (ase_worldPos).xz * float2( 0.1,0.1 ) ),_Worldspacetiling);
			float2 appendResult65 = (half2(_WaveDirectionX , _WaveDirectionY));
			half2 WaveSpeed69 = ( ( _Wavesspeed * _Time.x ) * appendResult65 );
			float2 HeightmapUV98 = ( ( ( Tiling88 * _WaveSize ) * float2( 0.1,0.1 ) ) + ( WaveSpeed69 * float2( 0.5,0.5 ) ) );
			float3 Displacement105 = ( ase_vertexNormal * ( _WaveHeight * tex2Dlod( _Shadermap, half4( HeightmapUV98, 0, 1.0) ).g ) );
			v.vertex.xyz += Displacement105;
			float3 ase_vertex3Pos = v.vertex.xyz;
			float3 vertexPos13 = ase_vertex3Pos;
			float4 ase_screenPos13 = ComputeScreenPos( UnityObjectToClipPos( vertexPos13 ) );
			o.screenPosition13 = ase_screenPos13;
		}

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			float4 ase_screenPos13 = i.screenPosition13;
			float4 ase_screenPosNorm13 = ase_screenPos13 / ase_screenPos13.w;
			ase_screenPosNorm13.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm13.z : ase_screenPosNorm13.z * 0.5 + 0.5;
			float temp_output_59_0 = ( _SizeFoam * 0.001 );
			float screenDepth13 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos13 )));
			float distanceDepth13 = abs( ( screenDepth13 - LinearEyeDepth( ase_screenPosNorm13.z ) ) / ( temp_output_59_0 ) );
			half3 ase_worldNormal = i.worldNormal;
			float lerpResult17 = lerp( distanceDepth13 , ( distanceDepth13 * ( 1.0 - ase_worldNormal.y ) ) , 0.998);
			half4 ifLocalVar35 = 0;
			if( lerpResult17 <= temp_output_59_0 )
				ifLocalVar35 = _ColorFoam;
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth28 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos )));
			float distanceDepth28 = abs( ( screenDepth28 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Depth ) );
			float clampResult32 = clamp( distanceDepth28 , 0.0 , 1.0 );
			float lerpResult33 = lerp( 1.0 , clampResult32 , _OpasityIntensity);
			float4 clampResult39 = clamp( ( ifLocalVar35 + lerpResult33 ) , float4( 0,0,0,0 ) , float4( 1,0,0,0 ) );
			float4 lerpResult30 = lerp( _Colorwater , _ColorDepth , clampResult32);
			half4 ifLocalVar7 = 0;
			if( lerpResult17 <= temp_output_59_0 )
				ifLocalVar7 = _ColorFoam;
			else
				ifLocalVar7 = lerpResult30;
			c.rgb = ifLocalVar7.rgb;
			c.a = clampResult39.r;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			float4 ase_screenPos13 = i.screenPosition13;
			float4 ase_screenPosNorm13 = ase_screenPos13 / ase_screenPos13.w;
			ase_screenPosNorm13.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm13.z : ase_screenPosNorm13.z * 0.5 + 0.5;
			float temp_output_59_0 = ( _SizeFoam * 0.001 );
			float screenDepth13 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos13 )));
			float distanceDepth13 = abs( ( screenDepth13 - LinearEyeDepth( ase_screenPosNorm13.z ) ) / ( temp_output_59_0 ) );
			half3 ase_worldNormal = i.worldNormal;
			float lerpResult17 = lerp( distanceDepth13 , ( distanceDepth13 * ( 1.0 - ase_worldNormal.y ) ) , 0.998);
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth28 = LinearEyeDepth(SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos )));
			float distanceDepth28 = abs( ( screenDepth28 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _Depth ) );
			float clampResult32 = clamp( distanceDepth28 , 0.0 , 1.0 );
			float4 lerpResult30 = lerp( _Colorwater , _ColorDepth , clampResult32);
			half4 ifLocalVar7 = 0;
			if( lerpResult17 <= temp_output_59_0 )
				ifLocalVar7 = _ColorFoam;
			else
				ifLocalVar7 = lerpResult30;
			o.Albedo = ifLocalVar7.rgb;
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
			#pragma target 2.0
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
				float3 worldNormal : TEXCOORD4;
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
				o.worldNormal = worldNormal;
				o.customPack1.xyzw = customInputData.screenPosition13;
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
				surfIN.screenPosition13 = IN.customPack1.xyzw;
				float3 worldPos = IN.worldPos;
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
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
}