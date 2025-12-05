Shader "HaMiLeJa/SuperBoostCustomShader"
{
	Properties
	{
		[HDR]_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
		_MainTex ("Particle Texture", 2D) = "white" {}
		_InvFade ("Soft Particles Factor", Range(0.01,10.0)) = 1.0
		[Toggle(_USESOFTPARTICLES_ON)] _UseSoftParticles("Use Soft Particles", Float) = 1
		_NoiseTex("Noise Tex", 2D) = "white" {}
		_AlphaCutout("Alpha Cutout", Float) = 0
		_TexPower("Tex Power", Float) = 1
		_NoiseSpeed("Noise Speed", Vector) = (0,0,0,0)
		_MaskClipValue("Mask Clip Value", Float) = 0.5
		_NoiseMin("Noise Min", Float) = 0
		_NoiseMax("Noise Max", Float) = 1
		[KeywordEnum(X,Y)] _Alignment("Alignment", Float) = 0
		_Size("Size", Float) = 1
		_NoisePower("Noise Power", Float) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		_BendingAmount("BendingAmount", Float) = 0.00988  //Bending
	}
	Category 
	{
		SubShader
		{
		LOD 0

			Tags { "Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent" "PreviewType"="Plane" }
			Blend SrcAlpha OneMinusSrcAlpha
			ColorMask RGB
			Cull Off
			Lighting Off 
			ZWrite Off
			ZTest LEqual
			
			Pass {
			
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma target 2.0
				#pragma multi_compile_particles
				#pragma multi_compile_fog
				#pragma shader_feature_local _USESOFTPARTICLES_ON
				#include "UnityShaderVariables.cginc"
				#define ASE_NEEDS_FRAG_COLOR
				#pragma shader_feature_local _ALIGNMENT_X _ALIGNMENT_Y
				  #pragma multi_compile __ ENABLE_BENDING

				#include "UnityCG.cginc"

				struct appdata_t 
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_VERTEX_INPUT_INSTANCE_ID
					
				};
				struct v2f 
				{
					float4 vertex : SV_POSITION;
					fixed4 color : COLOR;
					float4 texcoord : TEXCOORD0;
					UNITY_FOG_COORDS(1)
					#ifdef _USESOFTPARTICLES_ON
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD2;
					#endif
					#endif
					UNITY_VERTEX_INPUT_INSTANCE_ID
					UNITY_VERTEX_OUTPUT_STEREO
					
				};
				#if UNITY_VERSION >= 560
				UNITY_DECLARE_DEPTH_TEXTURE( _CameraDepthTexture );
				#else
				uniform sampler2D_float _CameraDepthTexture;
				#endif
				uniform sampler2D _MainTex;
				uniform fixed4 _TintColor;
				uniform float4 _MainTex_ST;
				uniform float _InvFade;
				uniform float _TexPower;
				uniform sampler2D _NoiseTex;
				uniform float2 _NoiseSpeed;
				uniform float4 _NoiseTex_ST;
				uniform float _NoisePower;
				uniform float _NoiseMin;
				uniform float _NoiseMax;
				uniform float _AlphaCutout;
				uniform float _Size;
				uniform float _MaskClipValue;

            float _BendingAmount;

				v2f vert ( appdata_t v  )
				{
					       #ifdef ENABLE_BENDING
            float4 distance = mul(unity_ObjectToWorld, v.vertex);

            distance.xyz -= _WorldSpaceCameraPos.xyz;
            distance = float4(0.0f, ((distance.x*distance.x + distance.z*distance.z)*-_BendingAmount),0.0f, 0.0f);

            v.vertex += mul(unity_WorldToObject, distance);
                #endif
					v2f o;
					UNITY_SETUP_INSTANCE_ID(v);
					UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
					UNITY_TRANSFER_INSTANCE_ID(v, o);
					v.vertex.xyz +=  float3( 0, 0, 0 ) ;
					o.vertex = UnityObjectToClipPos(v.vertex);
					
					#ifdef _USESOFTPARTICLES_ON
					#ifdef SOFTPARTICLES_ON
						o.projPos = ComputeScreenPos (o.vertex);
						COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
					#endif
					
					o.color = v.color;
					o.texcoord = v.texcoord;
					UNITY_TRANSFER_FOG(o,o.vertex);
					return o;
				}

				fixed4 frag ( v2f i  ) : SV_Target
				{
					#ifdef _USESOFTPARTICLES_ON
					#ifdef SOFTPARTICLES_ON
						float sceneZ = LinearEyeDepth (SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos)));
						float partZ = i.projPos.z;
						float fade = saturate (_InvFade * (sceneZ-partZ));
						i.color.a *= fade;
					#endif
					#endif

					float2 uv_MainTex = i.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
					float4 temp_cast_0 = (_TexPower).xxxx;
					float2 uv0_NoiseTex = i.texcoord.xy * _NoiseTex_ST.xy + _NoiseTex_ST.zw;
					float2 panner = ( _Time.y * _NoiseSpeed + uv0_NoiseTex);
					float2 uv = i.texcoord.xy * float2( 1,1 ) + float2( 0,0 );
					#if defined(_ALIGNMENT_X)
					float staticSwitch = uv.x;
					#elif defined(_ALIGNMENT_Y)
					float staticSwitch = uv.y;
					#else
					float staticSwitch = uv.x;
					#endif
					float3 uv04 = i.texcoord.xyz;
					uv04.xy = i.texcoord.xyz.xy * float2( 1,1 ) + float2( 0,0 );
					float temp_output_128_0 = ( abs( ( staticSwitch + ( _AlphaCutout + uv04.z ) ) ) - _Size );
					float smoothstepResult = smoothstep( _NoiseMin , _NoiseMax , temp_output_128_0);
					float OpacityMask = ( ( ( tex2D( _NoiseTex, panner ).r * _NoisePower ) - smoothstepResult ) * saturate( ( 1.0 - temp_output_128_0 ) ) );
					clip( saturate( OpacityMask ) - _MaskClipValue);
					fixed4 col = ( pow( tex2D( _MainTex, uv_MainTex ) , temp_cast_0 ) * _TintColor * i.color );
					UNITY_APPLY_FOG(i.fogCoord, col);
					return col;
				}
				ENDCG 
			}
		}	
	}
}
