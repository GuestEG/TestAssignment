Shader "Custom/UnlitTransparentShader"
{
	Properties
	{
		_Color ("Color", Color) = (1,1,1,1) //RGBA Color
		_Opacity("Opacity", Range(0.0,1.0)) = 0.5
	}
	
	SubShader {
      Tags { "Queue" = "Transparent" "RenderType" = "Transparent" }
      CGPROGRAM
      #pragma surface surf BlinnPhong alpha:auto

      struct Input {
          float4 color : COLOR;		  
      };
	  float4 _Color;
	  float _Opacity;
      void surf (Input IN, inout SurfaceOutput o) {
          o.Albedo = IN.color;
		  o.Emission = _Color;
		  o.Alpha = _Opacity;		  
      }
      ENDCG
    }
    
}
