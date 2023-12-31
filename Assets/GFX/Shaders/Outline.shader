//This version of the shader does not support shadows, but it does support transparent outlines

Shader "PUNKSOULS/Outline"
{
	Properties
	{
		_Color("Main Color", Color) = (0.5,0.5,0.5,1)
		_MainTex("Texture", 2D) = "white" {}

		_OutlineColor("Outline color", Color) = (1,0,0,0.5)
		_OutlineWidth("Outlines width", Range(0.0, 2.0)) = 0.15

		_Angle("Switch shader on angle", Range(0.0, 180.0)) = 89
	}

		CGINCLUDE
		#include "UnityCG.cginc"

		struct MeshData {
			float4 vertex : POSITION;
			float4 normal : NORMAL;
		};

		uniform float4 _OutlineColor;
		uniform float _OutlineWidth;

		uniform sampler2D _MainTex;
		uniform float4 _Color;
		uniform float _Angle;

		ENDCG

		SubShader{
			//First outline
			Pass
			{
				Tags
				{ 
				"Queue" = "Transparent" 
				"IgnoreProjector" = "True" 
				"RenderType" = "Transparent" 
				}
				// Blend SrcAlpha OneMinusSrcAlpha
				ZWrite Off
				Cull Back

				CGPROGRAM
				struct v2f {
					float4 pos : SV_POSITION;
				};

				#pragma vertex vert
				#pragma fragment frag

				v2f vert(MeshData v) {

					float3 scaleDir = normalize(v.vertex.xyz - float4(0,0,0,1));

					if (degrees(acos(dot(scaleDir.xyz, v.normal.xyz))) > _Angle) {
						v.vertex.xyz += normalize(v.normal.xyz) * _OutlineWidth;
					}
					else {
						v.vertex.xyz += scaleDir * _OutlineWidth;
					}

					v2f o;
					o.pos = UnityObjectToClipPos(v.vertex);
					return o;
				}

				float4 frag(v2f i) : COLOR{
					return _OutlineColor;
				}
				ENDCG

			}

			//Surface shader
			Tags{ "Queue" = "Transparent" }

			CGPROGRAM
			#pragma surface surf Lambert fullforwardshadows

			struct Input {
				float2 uv_MainTex;
				float4 color : COLOR;
			};

			void surf(Input IN, inout SurfaceOutput  o) {
				fixed4 c = tex2D(_MainTex, IN.uv_MainTex) * _Color;
				o.Albedo = c.rgb;
				o.Alpha = c.a;
			}
			ENDCG
	}
	
	Fallback "Diffuse"
}
