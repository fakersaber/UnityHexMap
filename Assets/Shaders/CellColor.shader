// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Unlit/NewUnlitShader"
{
    Properties
    {
		_GridTex("Grid Texture", 2D) = "white" {}
		//_Color("Color",Color) = (1,1,1,1)
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

		

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work

			sampler2D _GridTex;
			float4 _GridTex_ST;
            #include "UnityCG.cginc"

            struct appdata
            {
				//顶点颜色
				fixed4 color : COLOR;
                float4 vertex : POSITION;
				//float2 uv : TEXCOORD0;
            };

            struct v2f
            {
				fixed4 color : COLOR;
                float4 vertex : SV_POSITION;
				//float2 uv : TEXCOORD0;
				float4 worldPos : TEXCOORD1;
            };


            v2f vert (appdata v)
            {
                v2f o;
				
				//o.uv = TRANSFORM_TEX(v.uv, _GridTex);
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.color = v.color;
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float2 gridUV = i.worldPos.xz;
				//坐标映射
				gridUV.x *= 1 / (4 * 8.66025404);
				gridUV.y *= 1 / (3 * 10.0);
				//这里由于距离太远造成纹理放大缩小时表现不同，采用多级渐进纹理解决
				fixed3 final_color = tex2D(_GridTex, gridUV).rgb * i.color.rgb;// * i.color.rgb;

				return fixed4(final_color,1.0);
            }
            ENDCG
        }
    }
}
