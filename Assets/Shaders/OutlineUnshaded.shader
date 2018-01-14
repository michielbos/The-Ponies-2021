Shader "Cel Shading/(WIP) Outline Unshaded" {
	Properties {
		_Color ("Color", Color) = (1,1,1,1)
		_OutlineColor ("Outline Color", Color) = (1,1,1,1)
		_MainTex ("Albedo (RGB)", 2D) = "white" {}
		_Expand ("Expand", range(0, 0.1)) = 0.05
	}


	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 200

		ZWrite Off

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform float4 _Color;
			uniform float4 _OutlineColor;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float _Expand;

			struct appdata {
				float2 uv : TEXCOORD0;
				float4 vertex : POSITION;
				float4 normal : NORMAL;
			};

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			v2f vert (appdata v) {
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex + v.normal * _Expand);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}

			float4 frag (v2f i) : COLOR {
				float4 tex = tex2D(_MainTex, i.uv);
				return _OutlineColor;
			}

			ENDCG
		}

		ZWrite On

		Pass {
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			uniform float4 _Color;
			uniform sampler2D _MainTex;
			uniform float4 _MainTex_ST;
			uniform float _Expand;

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

			float4 frag (v2f i) : COLOR {
				float4 tex = tex2D(_MainTex, i.uv);
				return _Color * tex;
			}

			ENDCG
		}
	}
	FallBack "Diffuse"
}
