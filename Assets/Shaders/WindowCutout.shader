Shader "Cel Shading/Window Cutout" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_WindowMap("Window Map (R)", 2D) = "black" {}
		_WindowThresh("Window Threshold", range(0,1)) = 0.5
		_ShadowCutoff ("Shadow Cutoff", range(0, 1)) = 0.4
		_ShadowValue ("Shadow Value", Range(0,1)) = 0.65
		_ShadowColor ("Shadow Color", Color) = (1,1,1,1)
	}


	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform float4 _Color;
			uniform sampler2D _WindowMap;
			uniform float _WindowThresh;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float _ShadowCutoff;
			uniform float _ShadowValue;
			uniform float4 _ShadowColor;

			struct appdata {
				float2 uv : TEXCOORD0;
				float4 vertex : POSITION;
				float4 normal : NORMAL;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
				float3 viewDir : TEXCOORD1;
				float4 normal : NORMAL;
			};

			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.normal = v.normal;
				o.viewDir = ObjSpaceViewDir(v.vertex);
				return o;
			}

			float4 ViewDirOutline(float3 dir, float4 normal, float cutoff, float value, float4 color) {
				float val = 1 - max(0, (1 - dot(normalize(dir), normal)));
				float4 mul = 1;
				if (val < cutoff) {
					mul = value * color;
				}
				return mul;
			}

			float4 frag (v2f i) : COLOR {
				float4 map = tex2D(_WindowMap, i.uv);
				if (map.r < _WindowThresh){
					discard;
				}

				float4 tex = tex2D(_MainTex, i.uv);
				float4 shColor = ViewDirOutline(normalize(i.viewDir), i.normal, _ShadowCutoff, _ShadowValue, _ShadowColor);
				return _Color * tex * shColor;
			}

			ENDCG
		}
	}
	FallBack "Diffuse"
}
