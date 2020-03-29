Shader "Cel Shading/TVShaderV1"
{
	Properties
	{
		[IntRange]_Pixels("Pixels", Range( 16 , 512)) = 96
		_Color("Color", Color) = (0.3018868,0.3018868,0.3018868,1)
		_MainTexture("MainTexture", 2D) = "black" {}
		_ShadowValue("Shadow Value", Range( 0 , 1)) = 0.15
		[Toggle(_TVON_ON)] _TVOn("TVOn", Float) = 0
		[Toggle(_TVBROKEN_ON)] _TVBroken("TVBroken", Float) = 0
		_LCDPixels("LCD Pixels", 2D) = "white" {}
		[Toggle(_CHANNELON_ON)] _ChannelOn("ChannelOn", Float) = 0
		_BrokenTVSideEffect("BrokenTVSideEffect", 2D) = "white" {}
		_MinDistance("Min Distance", Float) = 0.25
		_MaxDistance("Max Distance", Float) = 5
		_DimSpeed("Dim Speed", Float) = 0.25
		_FarBrightness("Far Brightness", Float) = 1.2
		_DimFreq("Dim Freq", Float) = 1
		_CloseBrightness("Close Brightness", Float) = 1
		_BrightnessStatic("BrightnessStatic", Float) = 1.8
		_BrightnessOnChannel("BrightnessOnChannel", Float) = 1.8
		_DimWidth("Dim Width", Float) = 0.01
		_BrokenTvCrack("BrokenTvCrack", 2D) = "black" {}
		_DimPower("Dim Power", Float) = 2
		_DimStrength("Dim Strength", Float) = 0.8
		_rgbOffset("rgbOffset", Float) = 0.2
		_Noise("Noise", Range( 0 , 0.01)) = 0.001
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#pragma shader_feature_local _TVBROKEN_ON
		#pragma shader_feature_local _TVON_ON
		#pragma shader_feature_local _CHANNELON_ON
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
			float3 worldPos;
			float2 uv_texcoord;
			float3 worldNormal;
			INTERNAL_DATA
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

		uniform sampler2D _MainTexture;
		uniform float _CloseBrightness;
		uniform float _FarBrightness;
		uniform float _MinDistance;
		uniform float _MaxDistance;
		uniform sampler2D _LCDPixels;
		uniform float _Pixels;
		uniform float4 _MainTexture_ST;
		uniform float _Noise;
		uniform float _rgbOffset;
		uniform float _DimFreq;
		uniform float _DimSpeed;
		uniform float _DimWidth;
		uniform float _DimPower;
		uniform float _DimStrength;
		uniform float _BrightnessStatic;
		uniform float _BrightnessOnChannel;
		uniform sampler2D _BrokenTVSideEffect;
		uniform sampler2D _BrokenTvCrack;
		uniform float4 _BrokenTvCrack_ST;
		uniform float _ShadowValue;
		uniform half4 _Color;


		float3 mod2D289( float3 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float2 mod2D289( float2 x ) { return x - floor( x * ( 1.0 / 289.0 ) ) * 289.0; }

		float3 permute( float3 x ) { return mod2D289( ( ( x * 34.0 ) + 1.0 ) * x ); }

		float snoise( float2 v )
		{
			const float4 C = float4( 0.211324865405187, 0.366025403784439, -0.577350269189626, 0.024390243902439 );
			float2 i = floor( v + dot( v, C.yy ) );
			float2 x0 = v - i + dot( i, C.xx );
			float2 i1;
			i1 = ( x0.x > x0.y ) ? float2( 1.0, 0.0 ) : float2( 0.0, 1.0 );
			float4 x12 = x0.xyxy + C.xxzz;
			x12.xy -= i1;
			i = mod2D289( i );
			float3 p = permute( permute( i.y + float3( 0.0, i1.y, 1.0 ) ) + i.x + float3( 0.0, i1.x, 1.0 ) );
			float3 m = max( 0.5 - float3( dot( x0, x0 ), dot( x12.xy, x12.xy ), dot( x12.zw, x12.zw ) ), 0.0 );
			m = m * m;
			m = m * m;
			float3 x = 2.0 * frac( p * C.www ) - 1.0;
			float3 h = abs( x ) - 0.5;
			float3 ox = floor( x + 0.5 );
			float3 a0 = x - ox;
			m *= 1.79284291400159 - 0.85373472095314 * ( a0 * a0 + h * h );
			float3 g;
			g.x = a0.x * x0.x + h.x * x0.y;
			g.yz = a0.yz * x12.xz + h.yz * x12.yw;
			return 130.0 * dot( m, g );
		}


		float3 RGBToHSV(float3 c)
		{
			float4 K = float4(0.0, -1.0 / 3.0, 2.0 / 3.0, -1.0);
			float4 p = lerp( float4( c.bg, K.wz ), float4( c.gb, K.xy ), step( c.b, c.g ) );
			float4 q = lerp( float4( p.xyw, c.r ), float4( c.r, p.yzx ), step( p.x, c.r ) );
			float d = q.x - min( q.w, q.y );
			float e = 1.0e-10;
			return float3( abs(q.z + (q.w - q.y) / (6.0 * d + e)), d / (q.x + e), q.x);
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
			half3 LightColorData42 = ( ase_lightColor.rgb * ase_lightAtten );
			UnityGI gi45 = gi;
			float3 diffNorm45 = LightColorData42;
			gi45 = UnityGI_Base( data, 1, diffNorm45 );
			float3 indirectDiffuse45 = gi45.indirect.diffuse + diffNorm45 * 0.0001;
			half3 IndirDiffLight46 = indirectDiffuse45;
			float temp_output_61_0 = ( 1.0 - ( ( 1.0 - ase_lightAtten ) * _WorldSpaceLightPos0.w ) );
			float3 ase_worldNormal = WorldNormalVector( i, float3( 0, 0, 1 ) );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = normalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float dotResult37 = dot( ase_worldNormal , ase_worldlightDir );
			half NdotL40 = dotResult37;
			float lerpResult65 = lerp( temp_output_61_0 , ( saturate( ( NdotL40 / 0.001 ) ) * ase_lightAtten ) , _ShadowValue);
			half3 InputColor88 = (_Color).rgb;
			half3 BaseColorOutput82 = ( ( ( IndirDiffLight46 * ase_lightColor.a * temp_output_61_0 ) + ( ase_lightColor.rgb * lerpResult65 ) ) * InputColor88 );
			float2 uv_BrokenTvCrack = i.uv_texcoord * _BrokenTvCrack_ST.xy + _BrokenTvCrack_ST.zw;
			#ifdef _TVBROKEN_ON
				float3 staticSwitch247 = ( BaseColorOutput82 * (tex2D( _BrokenTvCrack, uv_BrokenTvCrack )).rgb );
			#else
				float3 staticSwitch247 = BaseColorOutput82;
			#endif
			c.rgb = staticSwitch247;
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
			float3 ase_worldPos = i.worldPos;
			float clampResult163 = clamp( distance( _WorldSpaceCameraPos , ase_worldPos ) , _MinDistance , _MaxDistance );
			float lerpResult167 = lerp( _CloseBrightness , _FarBrightness , (0.0 + (clampResult163 - _MinDistance) * (1.0 - 0.0) / (_MaxDistance - _MinDistance)));
			float2 appendResult154 = (float2(_Pixels , _Pixels));
			float2 temp_output_150_0 = ( appendResult154 * i.uv_texcoord );
			float mulTime121 = _Time.y * 4.0;
			float clampResult117 = clamp( mulTime121 , float2( -3,-1 ).x , float2( 3,1 ).x );
			float2 temp_cast_2 = (clampResult117).xx;
			float pixelWidth115 =  1.0f / 128.0;
			float pixelHeight115 = 1.0f / 128.0;
			half2 pixelateduv115 = half2((int)(i.uv_texcoord.x / pixelWidth115) * pixelWidth115, (int)(i.uv_texcoord.y / pixelHeight115) * pixelHeight115);
			float2 panner97 = ( 2.0 * _Time.y * temp_cast_2 + pixelateduv115);
			float simplePerlin2D94 = snoise( panner97*40.0 );
			simplePerlin2D94 = simplePerlin2D94*0.5 + 0.5;
			float3 temp_cast_3 = (simplePerlin2D94).xxx;
			float2 uv0_MainTexture = i.uv_texcoord * _MainTexture_ST.xy + _MainTexture_ST.zw;
			float2 appendResult344 = (float2(( _Time.y * 15.0 ) , ( i.uv_texcoord.y * 80.0 )));
			float simplePerlin2D345 = snoise( appendResult344 );
			float2 appendResult350 = (float2(( i.uv_texcoord.y * 25.0 ) , ( _Time.z * 1.0 )));
			float simplePerlin2D347 = snoise( appendResult350 );
			float temp_output_395_0 = ( uv0_MainTexture.x + ( ( simplePerlin2D345 + simplePerlin2D347 ) * _Noise ) );
			float temp_output_396_0 = ( _rgbOffset * 0.01 );
			float2 appendResult398 = (float2(( temp_output_395_0 - temp_output_396_0 ) , uv0_MainTexture.y));
			float2 appendResult400 = (float2(temp_output_395_0 , uv0_MainTexture.y));
			float2 appendResult401 = (float2(( temp_output_395_0 + temp_output_396_0 ) , uv0_MainTexture.y));
			float3 appendResult399 = (float3(tex2D( _MainTexture, appendResult398 ).r , tex2D( _MainTexture, appendResult400 ).g , tex2D( _MainTexture, appendResult401 ).b));
			#ifdef _CHANNELON_ON
				float3 staticSwitch100 = appendResult399;
			#else
				float3 staticSwitch100 = temp_cast_3;
			#endif
			float3 temp_cast_4 = (( sin( ( i.uv_texcoord.y * 560.0 ) ) * 0.04 )).xxx;
			float3 TVOutputColor204 = ( staticSwitch100 - temp_cast_4 );
			float mulTime269 = _Time.y * _DimSpeed;
			float temp_output_273_0 = ( 1.0 - _DimWidth );
			float clampResult274 = clamp( sin( ( ( _DimFreq * ( ( floor( temp_output_150_0 ) / appendResult154 ) + ( float2( 0.5,0.5 ) / appendResult154 ) ).y ) + mulTime269 ) ) , temp_output_273_0 , 1.0 );
			float3 clampResult171 = clamp( ( lerpResult167 * ( (tex2D( _LCDPixels, temp_output_150_0 )).rgb * TVOutputColor204 ) * (1.0 + (pow( (0.0 + (clampResult274 - temp_output_273_0) * (1.0 - 0.0) / (1.0 - temp_output_273_0)) , _DimPower ) - 0.0) * (_DimStrength - 1.0) / (1.0 - 0.0)) ) , float3( 0,0,0 ) , float3( 1,1,1 ) );
			#ifdef _CHANNELON_ON
				float staticSwitch333 = _BrightnessOnChannel;
			#else
				float staticSwitch333 = _BrightnessStatic;
			#endif
			half3 LCDOutput177 = ( clampResult171 * staticSwitch333 );
			#ifdef _TVON_ON
				float3 staticSwitch91 = LCDOutput177;
			#else
				float3 staticSwitch91 = float3(0,0,0);
			#endif
			float3 hsvTorgb264 = RGBToHSV( staticSwitch91 );
			float2 uv_BrokenTvCrack = i.uv_texcoord * _BrokenTvCrack_ST.xy + _BrokenTvCrack_ST.zw;
			#ifdef _TVBROKEN_ON
				float4 staticSwitch242 = ( float4( hsvTorgb264 , 0.0 ) * ( saturate( tex2D( _BrokenTVSideEffect, ( ( abs( (i.uv_texcoord*2.0 + -1.0) ) * float2( 0.5,0.01 ) ) + _Time.x ) ) ) * tex2D( _BrokenTvCrack, uv_BrokenTvCrack ) ) );
			#else
				float4 staticSwitch242 = float4( staticSwitch91 , 0.0 );
			#endif
			o.Emission = staticSwitch242.rgb;
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