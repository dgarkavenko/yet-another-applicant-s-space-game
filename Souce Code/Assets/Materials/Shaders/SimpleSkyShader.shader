// Shader created with Shader Forge Beta 0.36 
// Shader Forge (c) Joachim Holmer - http://www.acegikmo.com/shaderforge/
// Note: Manually altering this data may prevent you from opening it in Shader Forge
/*SF_DATA;ver:0.36;sub:START;pass:START;ps:flbk:,lico:1,lgpr:1,nrmq:1,limd:0,uamb:True,mssp:True,lmpd:False,lprd:False,enco:False,frtr:True,vitr:True,dbil:False,rmgx:True,rpth:0,hqsc:True,hqlp:False,tesm:0,blpr:0,bsrc:0,bdst:0,culm:0,dpts:2,wrdp:True,ufog:True,aust:True,igpj:False,qofs:0,qpre:1,rntp:1,fgom:False,fgoc:False,fgod:False,fgor:False,fgmd:0,fgcr:0.5,fgcg:0.5,fgcb:0.5,fgca:1,fgde:0.01,fgrn:0,fgrf:300,ofsf:0,ofsu:0,f2p0:False;n:type:ShaderForge.SFN_Final,id:1,x:32455,y:32739|emission-40-OUT;n:type:ShaderForge.SFN_Color,id:2,x:33293,y:32729,ptlb:Top,ptin:_Top,glob:False,c1:0.08235294,c2:0.1294118,c3:0.1607843,c4:1;n:type:ShaderForge.SFN_Color,id:3,x:33328,y:32872,ptlb:Mid,ptin:_Mid,glob:False,c1:0.08627451,c2:0.1333333,c3:0.1686275,c4:1;n:type:ShaderForge.SFN_Color,id:4,x:33290,y:33084,ptlb:Bot,ptin:_Bot,glob:False,c1:0.1568628,c2:0.1372549,c3:0.1686275,c4:1;n:type:ShaderForge.SFN_If,id:5,x:32860,y:32789|A-6-OUT,B-7-V,GT-9-OUT,EQ-8-OUT,LT-8-OUT;n:type:ShaderForge.SFN_Vector1,id:6,x:32929,y:32617,v1:0;n:type:ShaderForge.SFN_ScreenPos,id:7,x:33118,y:32635,sctp:0;n:type:ShaderForge.SFN_Lerp,id:8,x:33008,y:32827|A-3-RGB,B-2-RGB,T-7-V;n:type:ShaderForge.SFN_Lerp,id:9,x:32992,y:32983|A-3-RGB,B-4-RGB,T-10-OUT;n:type:ShaderForge.SFN_Abs,id:10,x:33042,y:33162|IN-7-V;n:type:ShaderForge.SFN_ValueProperty,id:12,x:32902,y:32314,ptlb:Overbrightness,ptin:_Overbrightness,glob:False,v1:1;n:type:ShaderForge.SFN_TexCoord,id:22,x:32544,y:32495,uv:0;n:type:ShaderForge.SFN_Tex2d,id:23,x:33228,y:32458,ptlb:Stars,ptin:_Stars,ntxv:2,isnm:False|UVIN-7-UVOUT;n:type:ShaderForge.SFN_Blend,id:40,x:32694,y:32844,blmd:6,clmp:True|SRC-69-OUT,DST-5-OUT;n:type:ShaderForge.SFN_Multiply,id:69,x:32902,y:32437|A-23-RGB,B-12-OUT;proporder:2-3-4-12-23;pass:END;sub:END;*/

Shader "Unlit/NewUnlitShader" {
    Properties {
        _Top ("Top", Color) = (0.08235294,0.1294118,0.1607843,1)
        _Mid ("Mid", Color) = (0.08627451,0.1333333,0.1686275,1)
        _Bot ("Bot", Color) = (0.1568628,0.1372549,0.1686275,1)
        _Overbrightness ("Overbrightness", Float ) = 1
        _Stars ("Stars", 2D) = "black" {}
		_StarsFar("StarsFar", 2D) = "black" {}


    }
    SubShader {
        Tags {
            "RenderType"="Opaque"
        }
        LOD 100
        Pass {
            Name "ForwardBase"
            Tags {
                "LightMode"="ForwardBase"
            }
            
            
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #define UNITY_PASS_FORWARDBASE
            #include "UnityCG.cginc"
            #pragma multi_compile_fwdbase_fullshadows
            #pragma exclude_renderers xbox360 ps3 flash d3d11_9x 
            #pragma target 3.0
            uniform float4 _Top;
            uniform float4 _Mid;
            uniform float4 _Bot;
            uniform float _Overbrightness;
            uniform sampler2D _Stars; uniform float4 _Stars_ST;
			uniform sampler2D _StarsFar; uniform float4 _StarsFar_ST;

            struct VertexInput {
                float4 vertex : POSITION;
            };
            struct VertexOutput {
                float4 pos : SV_POSITION;
                float4 screenPos : TEXCOORD0;
            };
            VertexOutput vert (VertexInput v) {
                VertexOutput o;
                o.pos = mul(UNITY_MATRIX_MVP, v.vertex);
                o.screenPos = o.pos;
                return o;
            }
            fixed4 frag(VertexOutput i) : COLOR {
                i.screenPos = float4( i.screenPos.xy / i.screenPos.w, 0, 0 );
                i.screenPos.y *= _ProjectionParams.x;
////// Lighting:
////// Emissive:
                float2 node_7 = i.screenPos;
                float node_5_if_leA = step(0.0,node_7.g);
                float node_5_if_leB = step(node_7.g,0.0);
                float3 node_8 = lerp(_Mid.rgb,_Top.rgb,node_7.g);
                float3 color = lerp((node_5_if_leA*node_8)+(node_5_if_leB*lerp(_Mid.rgb,_Bot.rgb,abs(node_7.g))),node_8,node_5_if_leA*node_5_if_leB);
                
				float3 emissive = saturate((1.0-(1.0-(tex2D(_Stars,TRANSFORM_TEX(node_7.rg, _Stars)).rgb*_Overbrightness))*(1.0- color)));
                

				float3 stars = tex2D(_StarsFar, TRANSFORM_TEX(node_7.rg, _StarsFar));

				float3 finalColor = emissive + stars;

                return fixed4(finalColor,1);
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
    CustomEditor "ShaderForgeMaterialInspector"
}
