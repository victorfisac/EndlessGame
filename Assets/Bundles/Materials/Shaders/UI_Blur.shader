Shader "UI/Blur"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _BlurAmountX("Blur Amount X", Range(-0.2, 0.2)) = 0.01
        _BlurAmountY("Blur Amount Y", Range(-0.2, 0.2)) = 0.01
    }

    SubShader
    {
        Tags
        { 
            "Queue"="Transparent" 
            "IgnoreProjector"="True" 
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Lighting Off
        ZWrite Off

        CGINCLUDE     
        	#include "UnityCG.cginc"
        	       
            struct appdata_t
            {
                half4 vertex   : POSITION;
                half4 color    : COLOR;
                half2 texcoord : TEXCOORD0;
            };

            struct v2f
            {
                half4 vertex   : SV_POSITION;
                half4 color    : COLOR;
                half2 texcoord : TEXCOORD0;
                half4 grabPosition : TEXCOORD1;
            };

            v2f vert(appdata_t IN)
            {
                v2f OUT;
                OUT.vertex = UnityObjectToClipPos(IN.vertex);
                OUT.texcoord = IN.texcoord;                
                OUT.color = IN.color;
                return OUT;
            }

            sampler2D _MainTex;
            half _BlurAmountX;
            half _BlurAmountY;
            fixed4 frag(v2f IN) : SV_Target
            {
            	half4 col = (
            	( tex2D(_MainTex, half2(IN.texcoord.x - 1.0f * _BlurAmountX, IN.texcoord.y - 1.0f * _BlurAmountY)) ) +
				( tex2D(_MainTex, half2(IN.texcoord.x - 1.0f * _BlurAmountX, IN.texcoord.y - 0.0f * _BlurAmountY)) ) +
				( tex2D(_MainTex, half2(IN.texcoord.x - 1.0f * _BlurAmountX, IN.texcoord.y + 1.0f * _BlurAmountY)) ) +
				( tex2D(_MainTex, half2(IN.texcoord.x - 0.0f * _BlurAmountX, IN.texcoord.y + 1.0f * _BlurAmountY)) ) +
				( tex2D(_MainTex, IN.texcoord) ) +
				( tex2D(_MainTex, half2(IN.texcoord.x + 1.0f * _BlurAmountX, IN.texcoord.y + 1.0f * _BlurAmountY)) ) +
				( tex2D(_MainTex, half2(IN.texcoord.x + 1.0f * _BlurAmountX, IN.texcoord.y + 0.0f * _BlurAmountY)) ) +
				( tex2D(_MainTex, half2(IN.texcoord.x + 1.0f * _BlurAmountX, IN.texcoord.y - 1.0f * _BlurAmountY)) ) +
				( tex2D(_MainTex, half2(IN.texcoord.x + 0.0f * _BlurAmountX, IN.texcoord.y - 1.0f * _BlurAmountY)) ) +

				( tex2D(_MainTex, half2(IN.texcoord.x - 1.5f * _BlurAmountX, IN.texcoord.y - 0.5f * _BlurAmountY)) ) +
				( tex2D(_MainTex, half2(IN.texcoord.x - 1.5f * _BlurAmountX, IN.texcoord.y + 0.5f * _BlurAmountY)) ) +
				( tex2D(_MainTex, half2(IN.texcoord.x + 1.5f * _BlurAmountX, IN.texcoord.y - 0.5f * _BlurAmountY)) ) +
				( tex2D(_MainTex, half2(IN.texcoord.x + 1.5f * _BlurAmountX, IN.texcoord.y + 0.5f * _BlurAmountY)) ) +

				( tex2D(_MainTex, half2(IN.texcoord.x - 0.5f * _BlurAmountX, IN.texcoord.y - 1.5f * _BlurAmountY)) ) +
				( tex2D(_MainTex, half2(IN.texcoord.x - 0.5f * _BlurAmountX, IN.texcoord.y + 1.5f * _BlurAmountY)) ) +
				( tex2D(_MainTex, half2(IN.texcoord.x + 0.5f * _BlurAmountX, IN.texcoord.y - 1.5f * _BlurAmountY)) ) +
				( tex2D(_MainTex, half2(IN.texcoord.x + 0.5f * _BlurAmountX, IN.texcoord.y + 1.5f * _BlurAmountY)) )
				
				) / 17.0f;

				col.a = 1;
				return col * IN.color;
            }
        ENDCG

        Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag			
			ENDCG
		}
    }
}