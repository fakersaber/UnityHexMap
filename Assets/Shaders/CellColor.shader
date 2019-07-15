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
			//float4 _GridTex_ST;
			uniform float _LinePos;

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

				float distoLine = abs(i.worldPos.x - _LinePos) - 8.66025404;

				//如果改成在线左右显示贴图然后渐变(r-1)个内圆，其他都是黑色
				if (distoLine <= 0) {
					return fixed4(tex2D(_GridTex, gridUV).rgb,1.0);
				}
				//外部渐变，距离越远ratio越小
				float ratio = 1.0 - saturate(distoLine / (8.66025404 * 3));
				//step(a, x) 如果a<x  return 1
				fixed3 final_color = ratio * tex2D(_GridTex, gridUV).rgb;

				return fixed4(final_color,1.0);


				//线周围显示黑色
				/***************************************************************************************
				//从轴线中间到左右两边一个内圆值都为0,超出为实际距离，然后除(r-1)个渐变内圆（因为值为0的距离为一个内圆长）表示在左右(r-1)个内圆渐变颜色，再截取到0到1
				//float ratio = saturate(max(0, abs(i.worldPos.x - _LinePos) - 8.66025404) / (8.66025404 * 3));	
				//fixed3 final_color = tex2D(_GridTex, gridUV).rgb  * ratio;// * i.color.rgb;
				//return fixed4(final_color,1.0);
				****************************************************************************************/
            }
            ENDCG
        }
    }
}
