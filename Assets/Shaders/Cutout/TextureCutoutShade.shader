Shader "Unphook/Cutout/Texture Cutout Shade" {
Properties {
	_MainTex("Texture (RGBA)",2D) = "white" {}
	_Cutoff("Alpha cutout",Range(0,1)) = 0.5
	_Color("Shade",Color) = (0,0,0,1)
}

SubShader {
	Tags {"Queue"="AlphaTest" "IgnoreProjector"="True" "RenderType"="TransparentCutout"}
	LOD 100
	
	Lighting Off
	
	Pass {
		CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 2.0
			#pragma multi_compile_fog
			
			#include "UnityCG.cginc"
			
			struct appdata_t {
				float4 vertex:POSITION;
				float2 texcoord:TEXCOORD0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			
			struct v2f {
				float4 vertex:SV_POSITION;
				float2 texcoord:TEXCOORD0;
				UNITY_FOG_COORDS(1)
				UNITY_VERTEX_OUTPUT_STEREO
			};
			
			sampler2D _MainTex;
			float4 _MainTex_ST;
			fixed _Cutoff;
			fixed4 _Color;
			
			v2f vert(appdata_t v) {
				v2f o;
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
			}
			
			fixed4 frag(v2f i):SV_Target {
				fixed4 tex = tex2D(_MainTex,i.texcoord);
				fixed4 col = tex;
				clip(col.a - _Cutoff);
				col = 1-(1-col)*(1-_Color);
				col.a = tex.a*_Color.a;
				UNITY_APPLY_FOG(i.fogCoord,col);
				return col;
			}
		ENDCG
	}
}
}