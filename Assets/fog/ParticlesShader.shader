
// Shader "Instanced/GridTestParticleShader" {
	
// 	Properties{
// 		_MainTex("Albedo (RGB)", 2D) = "white" {}
// 		_Glossiness("Smoothness", Range(0,1)) = 0.5
// 		_Metallic("Metallic", Range(0,1)) = 0.0
//         _Color("Color", Color) = (0.25, 0.5, 0.5, 1)
// 		// _DensityRange ("Density Range", Range(0,500000)) = 1.0
// 	}
// 		SubShader{
// 			Tags { "RenderType" = "Opaque" }
// 			LOD 200

// 			CGPROGRAM
// 			// Physically based Standard lighting model
// 			#pragma surface surf Standard addshadow fullforwardshadows
// 			#pragma multi_compile_instancing
// 			#pragma instancing_options procedural:setup
// 			// #pragma UNITY_PROCEDURAL_INSTANCING_ENABLED
			 
// 			sampler2D _MainTex;
// 			float _size;
//             float3 Color;
// 			float _DensityRange;

// 			struct Input {
// 				float2 uv_MainTex;
//             	float3 Color;
// 			};

// 			struct Particle
// 			{
//                 float pressure;
//                 float density;
//                 float3 currentForce;
//                 float3 velocity;
// 				float3 position;
				
// 			};

// 		// #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
// 			StructuredBuffer<Particle> _particles;
// 		// #endif

// 			void setup()
// 			{
// 			// #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
// 				float3 pos = _particles[unity_InstanceID].position;
// 				float size = _size;

// 				unity_ObjectToWorld._11_21_31_41 = float4(size, 0, 0, 0);
// 				unity_ObjectToWorld._12_22_32_42 = float4(0, size, 0, 0);
// 				unity_ObjectToWorld._13_23_33_43 = float4(0, 0, size, 0);
// 				unity_ObjectToWorld._14_24_34_44 = float4(pos.xyz, 1);
// 				unity_WorldToObject = unity_ObjectToWorld;
// 				unity_WorldToObject._14_24_34 *= -1;
// 				unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;
// 			// #endif
// 			}
			
// 			half _Glossiness;
// 			half _Metallic;

// 			void surf(Input IN, inout SurfaceOutputStandard o) {

// 				// #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
// 					// float dens = _particles[unity_InstanceID].pressure;
// 					// float4 col = float4(dens/_DensityRange, 0,0, 1);
				
// 					o.Albedo = IN.Color;
// 					o.Metallic = 0;
// 					o.Smoothness = 0;
// 					o.Alpha = 1;
// 				// #endif

				
// 			}
// 			ENDCG
// 		}
// 			FallBack "Diffuse"
// }


// Shader "Instanced/GridTestParticleShader"
// {
//     Properties
//     {
//         _MainTex("Albedo (RGB)", 2D) = "white" {}
//         _Glossiness("Smoothness", Range(0,1)) = 0.5
//         _Metallic("Metallic", Range(0,1)) = 0.0
//         _Color("Color", Color) = (0.25, 0.5, 0.5, 1)
//         _DensityRange("Density Range", Range(0,500000)) = 1.0
//     }
//     SubShader
//     {
//         Tags { "RenderType" = "Opaque" }
//         LOD 200

//         Pass
//         {
//             CGPROGRAM
//             #include "UnityCG.cginc"

//             #pragma vertex vert
//             #pragma fragment frag
//             #pragma multi_compile_instancing
//             #pragma instancing_options procedural:setup

//             struct Particle
//             {
//                 float3 position;
//                 float3 velocity;
//                 float3 currentForce;
//                 float density;
//                 float pressure;
//             };

//             StructuredBuffer<Particle> _particles;

//             sampler2D _MainTex;
//             float _Glossiness;
//             float _Metallic;
//             float _DensityRange;

//             struct appdata
//             {
//                 float4 vertex : POSITION;
//                 float3 normal : NORMAL;
//                 float4 texcoord : TEXCOORD0;
//                 UNITY_VERTEX_INPUT_INSTANCE_ID
//             };

//             struct v2f
//             {
//                 float2 uv : TEXCOORD0;
//                 float4 vertex : SV_POSITION;
//                 UNITY_VERTEX_OUTPUT_STEREO
//             };

//             void setup()
//             {
//                 #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
//                     float3 pos = _particles[unity_InstanceID].position;
//                     float size = 1.0;

//                     unity_ObjectToWorld._11_21_31_41 = float4(size, 0, 0, 0);
//                     unity_ObjectToWorld._12_22_32_42 = float4(0, size, 0, 0);
//                     unity_ObjectToWorld._13_23_33_43 = float4(0, 0, size, 0);
//                     unity_ObjectToWorld._14_24_34_44 = float4(pos.xyz, 1);
//                     unity_WorldToObject = unity_ObjectToWorld;
//                     unity_WorldToObject._14_24_34 *= -1;
//                     unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;
//                 #endif
//             }

//             v2f vert(appdata v)
//             {
//                 UNITY_SETUP_INSTANCE_ID(v);
//                 v2f o;
//                 UNITY_INITIALIZE_OUTPUT(v2f, o);
//                 o.uv = v.texcoord.xy;
//                 o.vertex = UnityObjectToClipPos(v.vertex);
//                 return o;
//             }

//             fixed4 frag(v2f i) : SV_Target
//             {

//                 float density = _particles[unity_InstanceID].density;
//                 float4 col = float4(density / _DensityRange, 0, 0, 1);
//                 return col;
//             }
//             ENDCG
//         }
//     }
//     FallBack "Diffuse"
// }

Shader "Instanced/GridTestParticleShader"
{
    Properties
    {
        _MainTex("Albedo (RGB)", 2D) = "white" {}
        _Glossiness("Smoothness", Range(0,1)) = 0.5
        _Metallic("Metallic", Range(0,1)) = 0.0
        _Color("Color", Color) = (0.25, 0.5, 0.5, 1)
        _DensityRange("Density Range", Range(0,500000)) = 1.0
    }
    SubShader
    {
        Tags { "RenderType" = "Opaque" }
        LOD 200

        Pass
        {
            CGPROGRAM
            #include "UnityCG.cginc"

            #pragma vertex vert
            #pragma fragment frag
            #pragma multi_compile_instancing
            #pragma instancing_options procedural:setup

            struct Particle
            {
                float3 position;
                float3 velocity;
                float3 currentForce;
                float density;
                float pressure;
            };

            StructuredBuffer<Particle> _particles;

            sampler2D _MainTex;
            float _Glossiness;
            float _Metallic;
            float _DensityRange;

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float4 texcoord : TEXCOORD0;
                UNITY_VERTEX_INPUT_INSTANCE_ID
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                UNITY_VERTEX_OUTPUT_STEREO
            };

            void setup()
            {
                #ifdef UNITY_PROCEDURAL_INSTANCING_ENABLED
                    float3 pos = _particles[unity_InstanceID].position;
                    float size = 0.5;

                    unity_ObjectToWorld._11_21_31_41 = float4(size, 0, 0, 0);
                    unity_ObjectToWorld._12_22_32_42 = float4(0, size, 0, 0);
                    unity_ObjectToWorld._13_23_33_43 = float4(0, 0, size, 0);
                    unity_ObjectToWorld._14_24_34_44 = float4(pos.xyz, 1);
                    unity_WorldToObject = unity_ObjectToWorld;
                    unity_WorldToObject._14_24_34 *= -1;
                    unity_WorldToObject._11_22_33 = 1.0f / unity_WorldToObject._11_22_33;
                #endif 
            }

            v2f vert(appdata v)
            {
                UNITY_SETUP_INSTANCE_ID(v);
                v2f o;
                UNITY_INITIALIZE_OUTPUT(v2f, o);
                o.uv = v.texcoord.xy;
                o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag(v2f i) : SV_Target
            {
                float density = _particles[unity_InstanceID].density;
                float4 col = float4(density / _DensityRange, 0, 0, 1);
                return col;
            }
            ENDCG
        }
    }
    FallBack "Diffuse"
}