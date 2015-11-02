Shader "Hidden/TV"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_Scale("Scale", int) = 1
		_MainColor("MainColor", float) = 1
		_AddColor("AdditionalColors", float) = 0
		_Scanlines("Scanlines", float) = 0
		_Distort("Distort", float) = 1


	}
	SubShader
	{
		// No culling or depth
		Cull Off ZWrite Off ZTest Always

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
				fixed4 color : COLOR;
			};

			struct v2f
			{
				float2 uv : TEXCOORD0;
				float4 vert : SV_POSITION;


			};

			v2f vert (appdata v)
			{
				v2f o;
				o.vert = mul(UNITY_MATRIX_MVP, v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;
			int _Scale;
			float _MainColor;
			float _AddColor;
			float _Scanlines;
			float _Distort;
			const float PI = 3.14159;

			float2 radialDistortion(float2 coord)
			{
				coord *= 2;
				float2 cc = coord - 1.0;
				float dist = dot(cc, cc) * (_Distort / 100.0);
				return (coord + cc * (1.0 + dist) * dist) / 2;
			}

			fixed4 frag (v2f i) : SV_Target
			{
				
				i.uv = radialDistortion(i.uv);
				
				
				float mul = 1;

				
				float2 lns = abs(i.uv - float2(0.5,0.5));

		
				if (lns.x > 0.49) {
					mul = 0.0017 / (lns.x-0.489);
				}

				if (lns.y > 0.49)
				{
					mul *= 0.0017 / (lns.y - 0.489);
				}

				



				i.uv.x = abs(0.98 - abs(i.uv.x - 0.99)) + 0.01;
				i.uv.y = abs(0.98 - abs(i.uv.y - 0.99)) + 0.01;



				float4 color = tex2D(_MainTex, i.uv);
				
				//i.vertex.xy = (int2)((i.vertex.xy + float2(1, 1))/2 * _ScreenParams);

				//i.vertex = abs(i.vertex * _ScreenParams);
				//if (i.vertex.x < 0) i.vertex.x = i.vertex.x + 1;

				float2 vertex = i.uv * _ScreenParams;
				//i.vertex.x = ((i.vertex.x + 1.0) / 2.0 * _ScreenParams);


				int column = abs(vertex.x) % (3 * _Scale);
				column = abs(column);

				float4 o = float4(0, 0, 0, 1);

				if (column < _Scale) {
					color = color * float4(_MainColor, _AddColor, _AddColor, 1);
				}				
				else if (column < _Scale * 2) {
					color = color * float4(_AddColor, _MainColor, _AddColor, 1);
				}
				else {
					color = color * float4(_AddColor, _AddColor, _MainColor, 1);
				}


				int row = (int)vertex.y % (3 * _Scale);
				if (row < _Scale) color *= _Scanlines;

				return color * mul;

			}
			ENDCG
		}
	}
}
