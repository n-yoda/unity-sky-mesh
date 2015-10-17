Shader "Custom/SkyMesh"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		[Enum(CullMode)] _CullMode ("Cull Mode", float) = 1
        [Toggle] _Finite ("Finite Radius", float) = 0
	}
	SubShader
	{
		Tags { "Queue"="Geometry+500" }
		Cull [_CullMode]
		ZWrite Off
		Pass
		{
			CGPROGRAM
            #pragma multi_compile _FINITE_OFF _FINITE_ON
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			
			v2f vert (appdata v)
			{
				v2f o;
                #if _FINITE_ON
                    o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
                #else
                    o.vertex = mul(UNITY_MATRIX_MVP, float4(v.vertex.xyz, 0));
                #endif
				o.vertex.z = o.vertex.w;
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target
			{
				return tex2D(_MainTex, i.uv);
			}
			ENDCG
		}
	}
}
