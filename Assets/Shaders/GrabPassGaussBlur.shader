Shader "Custom/GrabPassGaussBlur" {
    Properties {
		//Multiplicative tint color
        _Color ("Tint Color", Color) = (1,1,1,1)
		//Blur control. >20 create unpleasant artifacts
        _Blur ("Blur Amount", Range(0, 20)) = 1
    }
   
    Category {
   
        // We must be transparent, so other objects are drawn before this one.
        Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Opaque" }
   
   
        SubShader {
       
            //grab screen region of object into _GrabTexture
            GrabPass {                     
                Tags { "LightMode" = "Always" }
            }
			// First Processing Pass - Horizontal Blur
            Pass {
                Tags { "LightMode" = "Always" }
               
                CGPROGRAM
                #pragma vertex vert //vertex shader
                #pragma fragment frag //fragment shader
                #include "UnityCG.cginc"
               
                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 texcoord: TEXCOORD0;
                };
               
                struct vertex_out {
                    float4 vertex : POSITION;
                    float4 uvgrab : TEXCOORD0;
                };
               
                vertex_out vert (appdata_t v) {
                    vertex_out o;
					//Unity utilites for grabbed textures
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uvgrab = ComputeGrabScreenPos(o.vertex);
                    return o;
                }
                //grabbed texture sampler
                sampler2D _GrabTexture;
				//grabbed texture texel size for scaling.
                float4 _GrabTexture_TexelSize;
				//control parameter
                float _Blur;
               
                half4 frag( vertex_out input ) : COLOR {
                    
					/*
					gaussian blur method:
					sample row of pixels from original texture, each with different offset (i'll call it offsetX)
					multiply each by gaussian bell curve weight coefficients
					and summarise the resulting images.
					sum of coefficients should be normalised (equals 1.0)!

					then repeat in columns with the same offset but different axis (offsetY).

					_Blur controls magnitude of offset
					_GrabTexture_TexelSize makes _Blur offset measured in texture texels

					*/

					//result
					half4 sum = half4(0,0,0,0);

					//define MACROS! Because too long
					#define GRABPIXEL(weight,offsetX) tex2Dproj( _GrabTexture, input.uvgrab + half4(_GrabTexture_TexelSize.x * offsetX * _Blur, 0, 0, 0) ) * weight;
					
					//9-point tap makes pretty smooth picture
                    sum += GRABPIXEL(0.05, -4.0);
                    sum += GRABPIXEL(0.09, -3.0);
                    sum += GRABPIXEL(0.12, -2.0);
                    sum += GRABPIXEL(0.15, -1.0);
                    sum += GRABPIXEL(0.18,  0.0);
                    sum += GRABPIXEL(0.15, +1.0);
                    sum += GRABPIXEL(0.12, +2.0);
                    sum += GRABPIXEL(0.09, +3.0);
                    sum += GRABPIXEL(0.05, +4.0);
                   
                    return sum;
                }
                ENDCG
            }

			//grab result again!
            GrabPass {                     
                Tags { "LightMode" = "Always" }
            }
			// Second Processing Pass - Vertica Blur
			//it is indeed the same as horizontal, but vertical instead
            Pass {
                Tags { "LightMode" = "Always" }
               
                CGPROGRAM
                #pragma vertex vert //vertex shader
                #pragma fragment frag //fragment shader
                #include "UnityCG.cginc"
               
                struct appdata_t {
                    float4 vertex : POSITION;
                    float2 texcoord: TEXCOORD0;
                };
               
                struct vertex_out {
                    float4 vertex : POSITION;
                    float4 uvgrab : TEXCOORD0;
                };
               
                vertex_out vert (appdata_t v) {
                    vertex_out o;
					//Unity utilites for grabbed textures
					o.vertex = UnityObjectToClipPos(v.vertex);
					o.uvgrab = ComputeGrabScreenPos(o.vertex);
                    return o;
                }
                //grabbed texture sampler
                sampler2D _GrabTexture;
				//grabbed texture texel size for scaling.
                float4 _GrabTexture_TexelSize;
				//control parameter
                float _Blur;
				//tint color
				half4 _Color;
               
                half4 frag( vertex_out input ) : COLOR {
                    
					//result
					half4 sum = half4(0,0,0,0);

					//define MACROS! Because too long
					#define GRABPIXEL(weight,offsetY) tex2Dproj( _GrabTexture, input.uvgrab + half4(0, _GrabTexture_TexelSize.x * offsetY * _Blur, 0, 0) ) * weight;
					
					//9-point tap makes pretty smooth picture
                    sum += GRABPIXEL(0.05, -4.0);
                    sum += GRABPIXEL(0.09, -3.0);
                    sum += GRABPIXEL(0.12, -2.0);
                    sum += GRABPIXEL(0.15, -1.0);
                    sum += GRABPIXEL(0.18,  0.0);
                    sum += GRABPIXEL(0.15, +1.0);
                    sum += GRABPIXEL(0.12, +2.0);
                    sum += GRABPIXEL(0.09, +3.0);
                    sum += GRABPIXEL(0.05, +4.0);
                   
				    //finishing touch: color tint controlled by _Color
					return sum * _Color;
                }
                ENDCG
            }
        }
    }
}