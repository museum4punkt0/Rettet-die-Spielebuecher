// Upgrade NOTE: upgraded instancing buffer 'InstProperties' to new syntax.

//----------------------------------------------
//            Marvelous Techniques
// Copyright © 2017 - Arto Vaarala, Kirnu Interactive
// http://www.kirnuarp.com
//----------------------------------------------
Shader "Kirnu/Marvelous/CustomLightingMasterInstanced" {

	Properties {
		_MainTex ("Texture", 2D) = "white" {}
		_MainTexPower  ("Main Texture Power", Range(0,1)) = 1
		_LayoutTexture ("Layout Texture", 2D) = "white" {}
		_LayoutTexturePower ("Layout Texture Power", Range(0,1)) = 0.5
		_LightRampTexture ("Distance Light Texture", 2D) = "white" {}

 		_TopColor ("Top Color", Color) = (1,1,1,0)
 		_TopColorBottom ("Top Color Bottom", Color) = (1,1,1,0)

 		_RightColor ("Right Color", Color) = (0.9,0.9,0.9,0)
 		_RightColorBottom ("Right Color Bottom", Color) = (0.9,0.9,0.9,0)

 		_FrontColor ("Front Color", Color) = (0.7,0.7,0.7,0)
 		_FrontColorBottom ("Front Color Bottom", Color) = (0.7,0.7,0.7,0)

 		_MainColor ("Main Color Top", Color) = (0.7,0.7,0.7,0)
 		_MainColorBottom ("Main Color Bottom", Color) = (1,0.73,0.117,0)

 		_TopLight ("Top Light", Float) = 1
 		_RightLight ("Right Light", Float) = 0.9
 		_FrontLight ("Front Light", Float) = 0.7

 		_GradientStartY ("Gradient Start Y", Float) = 0
 		_GradientHeight ("Gradient Height", Float) = 1

 		_LightMaxDistance ("Light Max Distance", Float) = 10
		_LightPos ("Light position", Vector) = (0,0,0,1)
		_additiveBlend ("Additive blend", Float) = 0

 		_RimColor ("Rim Color", Color) = (0,0,0,0)
 		_RimColorBottom ("Rim Color Bottom", Color) = (0,0,0,0)
 		_RimPower ("Rim Power", Float) = 0.0
 		
 		_LightTint ("Light Tint", Color) = (1,1,1,0)
 		_AmbientColor ("Ambient Color", Color) = (0.5,0.1,0.2,0.0)
 		_AmbientPower ("Ambient Power", Float) = 0.0

 		_FogColor ("Fog color", Color) = (1,1,1,1)
		_FogYStartPos ("Fog Y-start pos", Float) = 0
		_FogHeight ("Fog Height", Float) = 0.1
		_FogAnimationHeight ("Fog Animation Height", Float) = 0.1
		_FogAnimationFreq ("Fog Animation Frequency", Float) = 0.1
		_UseFogDistance ("Distance Fog", Float) = 0

		_FogStart ("Distance Start", Float) = 0
 		_FogEnd ("Distance End", Float) = 50
 		_FogDensity ("Distance Density", Float) = 1

 		_FogStaticStartPos ("Fog static start", Vector) = (0,0,0)
 		[Toggle(FOG_STATIC_START_POS)] _UseFogStaticStart ("Fog static start", Float) = 0

		_LightmapColor ("Lightmap Tint", Color) = (0,0,0,0)
		_LightmapPower ("Lightmap Power", Float) = 1
		_ShadowPower ("Shadow Light", Float) = 0
		[Toggle(USE_LIGHTMAP)] _UseLightMap ("Lightmap Enabled", Float) = 0


		[HideInInspector]_LightDirF ("Front Light Direction", Vector) = (0,0,-1)
		[HideInInspector]_LightDirT ("Top Light Direction", Vector) = (0,1,0)
		[HideInInspector]_LightDirR ("Right Light Direction", Vector) = (1,0,0)
		[Toggle(USE_DIR_LIGHT)] _UseDirLight ("Directional Light", Float) = 0

		_Alpha ("Alpha", Range(0,1)) = 0
		_Cutoff("Cutoff", Range(0,1)) = 0
		_LightProbePower  ("Light Probe Power", Range(0,1)) = 0.5

		_SpecColorc ("Specular Color", Color) = (1,1,1,0)
		_Shininess ("Gloss", Range(0,8)) = 0
		_Specular ("Specular", Range(0,65)) = 0.01

		_LightMaxDistance ("Light Max Distance", Float) = 10
		_LightPos ("Light position", Vector) = (0,0,0,1)
		_additiveBlend ("Additive blend", Float) = 0

		// Blending state	
		[HideInInspector] _SrcBlend ("__src", Float) = 1.0
		[HideInInspector] _DstBlend ("__dst", Float) = 0.0
		[HideInInspector] _ZWrite ("__zw", Float) = 1.0

	}
	SubShader {
		Tags { "QUEUE"="Geometry" "RenderType"="Opaque" }
		LOD 200

		Pass {
			ZWrite [_ZWrite]
		Blend [_SrcBlend] [_DstBlend]
		Tags { "LIGHTMODE"="ForwardBase" "QUEUE"="Geometry" "RenderType"="Opaque" }
			CGPROGRAM
				#pragma shader_feature USE_LIGHTMAP
				#pragma shader_feature USE_DIR_LIGHT
				#pragma shader_feature USE_MAIN_TEX
				#pragma shader_feature USE_LAYOUT_TEXTURE
				#pragma shader_feature USE_GRADIENT
				#pragma shader_feature GRADIENT_LOCAL_SPACE
				#pragma shader_feature LIGHTING_FULL
				#pragma shader_feature USE_FOG
				#pragma shader_feature USE_DIST_FOG
				#pragma shader_feature TRANSPARENT
				#pragma shader_feature CUTOUT
				#pragma shader_feature USE_DIST_LIGHT
				#pragma shader_feature DIST_LIGHT_ADDITIVE
				#pragma shader_feature USE_REALTIME_SHADOWS
				#pragma shader_feature USE_LIGHT_PROBES
				#pragma shader_feature USE_SPECULAR
				#pragma shader_feature USE_SPECULAR_PIXEL_SHADING
				#pragma shader_feature FOG_STATIC_START_POS
				#pragma shader_feature USE_VERTEX_COLOR

				#pragma multi_compile_fwdbase
				#pragma fragmentoption ARB_precision_hint_fastest
				
				#pragma vertex vert
				#pragma fragment frag

				#pragma multi_compile_instancing

				#define USE_INSTANCING
				#define MASTER_SHADER

				#include "UnityCG.cginc"

UNITY_INSTANCING_BUFFER_START (InstProperties)
            	UNITY_DEFINE_INSTANCED_PROP (half, _MainTexPower)
#define _MainTexPower_arr InstProperties
				UNITY_DEFINE_INSTANCED_PROP (half3, _RightColor)
#define _RightColor_arr InstProperties
				UNITY_DEFINE_INSTANCED_PROP (half3, _TopColor)
#define _TopColor_arr InstProperties
				UNITY_DEFINE_INSTANCED_PROP (half3, _FrontColor)
#define _FrontColor_arr InstProperties

				UNITY_DEFINE_INSTANCED_PROP (half3, _MainColor)
#define _MainColor_arr InstProperties
				UNITY_DEFINE_INSTANCED_PROP (half, _TopLight)
#define _TopLight_arr InstProperties
				UNITY_DEFINE_INSTANCED_PROP (half, _RightLight)
#define _RightLight_arr InstProperties
				UNITY_DEFINE_INSTANCED_PROP (half, _FrontLight)
#define _FrontLight_arr InstProperties

				UNITY_DEFINE_INSTANCED_PROP (half3, _MainColorBottom)
#define _MainColorBottom_arr InstProperties
				UNITY_DEFINE_INSTANCED_PROP (half3, _RightColorBottom)
#define _RightColorBottom_arr InstProperties
				UNITY_DEFINE_INSTANCED_PROP (half3, _TopColorBottom)
#define _TopColorBottom_arr InstProperties
				UNITY_DEFINE_INSTANCED_PROP (half3, _FrontColorBottom)
#define _FrontColorBottom_arr InstProperties
				UNITY_DEFINE_INSTANCED_PROP (half3, _RimColorBottom)
#define _RimColorBottom_arr InstProperties

				UNITY_DEFINE_INSTANCED_PROP (half, _GradientStartY)
#define _GradientStartY_arr InstProperties
				UNITY_DEFINE_INSTANCED_PROP (half, _GradientHeight)
#define _GradientHeight_arr InstProperties

				//UNITY_DEFINE_INSTANCED_PROP (half, _UseFog)
				UNITY_DEFINE_INSTANCED_PROP (half3, _RimColor)
#define _RimColor_arr InstProperties
				UNITY_DEFINE_INSTANCED_PROP (half, _RimPower)
#define _RimPower_arr InstProperties

				UNITY_DEFINE_INSTANCED_PROP (half3, _AmbientColor)
#define _AmbientColor_arr InstProperties
				UNITY_DEFINE_INSTANCED_PROP (half, _AmbientPower)
#define _AmbientPower_arr InstProperties

				//UNITY_DEFINE_INSTANCED_PROP (half, _LayoutTexturePower)

				UNITY_DEFINE_INSTANCED_PROP (half3, _FogColor)
#define _FogColor_arr InstProperties

				//UNITY_DEFINE_INSTANCED_PROP (half, _FogYStartPos)
				//UNITY_DEFINE_INSTANCED_PROP (half, _FogHeight)
				//UNITY_DEFINE_INSTANCED_PROP (half, _FogAnimationHeight)
				//UNITY_DEFINE_INSTANCED_PROP (half, _FogAnimationFreq)

				//UNITY_DEFINE_INSTANCED_PROP (half3, _FogStaticStartPos)
				//UNITY_DEFINE_INSTANCED_PROP (half, _UseFogStaticStart)

				UNITY_DEFINE_INSTANCED_PROP (half, _FogStart)
#define _FogStart_arr InstProperties
				UNITY_DEFINE_INSTANCED_PROP (half, _FogEnd)
#define _FogEnd_arr InstProperties
				UNITY_DEFINE_INSTANCED_PROP (half, _FogDensity)
#define _FogDensity_arr InstProperties

				UNITY_DEFINE_INSTANCED_PROP (half, _LightMaxDistance)
#define _LightMaxDistance_arr InstProperties
				UNITY_DEFINE_INSTANCED_PROP (half3, _LightPos)
#define _LightPos_arr InstProperties

				UNITY_DEFINE_INSTANCED_PROP (half, _Alpha)
#define _Alpha_arr InstProperties
				UNITY_DEFINE_INSTANCED_PROP (half, _Cutoff)
#define _Cutoff_arr InstProperties

				UNITY_DEFINE_INSTANCED_PROP (half, _LightProbePower)
#define _LightProbePower_arr InstProperties

				UNITY_DEFINE_INSTANCED_PROP (half3, _SpecColorc)
#define _SpecColorc_arr InstProperties
				UNITY_DEFINE_INSTANCED_PROP (half, _Shininess)
#define _Shininess_arr InstProperties
				UNITY_DEFINE_INSTANCED_PROP (half, _Specular)
#define _Specular_arr InstProperties
UNITY_INSTANCING_BUFFER_END(InstProperties)

				uniform half _FogYStartPos;
				uniform half _FogHeight;
				uniform half _FogAnimationHeight;
				uniform half _FogAnimationFreq;	
				uniform half3 _FogStaticStartPos;
				uniform half _UseFogStaticStart;

				uniform half _LayoutTexturePower;

				uniform half _UseLightMap;
				uniform half _LightmapPower;
				uniform half3 _LightTint;
				uniform half3 _LightmapColor;
				uniform half _ShadowPower;

				#define IP UNITY_ACCESS_INSTANCED_PROP
				#include "MarvelousInstanced.cginc"

            	

				CL_OUT_WPOS vert(CL_IN v) {
					UNITY_SETUP_INSTANCE_ID(v);

					#ifndef LIGHTING_FULL
						#ifdef USE_FOG
							#ifndef USE_DIST_FOG
								return customLightingSimpleSoftFogVert(v, 
								IP (InstProperties,_MainColor), 
								IP (InstProperties,_RimColor), 
								IP (InstProperties,_RimPower), 
								IP (InstProperties,_RightLight),
								IP (InstProperties,_FrontLight),  
								IP (InstProperties,_TopLight), 
								IP (InstProperties,_AmbientColor), 
								IP (InstProperties,_AmbientPower), 
								_FogYStartPos, 
								_FogAnimationHeight, 
								_FogAnimationFreq);
							#else // USE_DIST_FOG
								CL_OUT_WPOS o=customLightingSimpleSoftFogVert(v, 
								IP (InstProperties,_MainColor), 
								IP (InstProperties,_RimColor), 
								IP (InstProperties,_RimPower), 
								IP (InstProperties,_RightLight), 
								IP (InstProperties,_FrontLight), 
								IP (InstProperties,_TopLight), 
								IP (InstProperties,_AmbientColor), 
								IP (InstProperties,_AmbientPower),
								IP(InstProperties,_FogYStartPos), 1, 1);
								#ifndef FOG_STATIC_START_POS 
								float cameraVertDist = length(_WorldSpaceCameraPos - o.wpos)*IP(_FogDensity); 
								#else
								float cameraVertDist = length(_FogStaticStartPos - o.wpos)*IP(_FogDensity); 
								#endif
								o.color.w = saturate((IP(_FogEnd) - cameraVertDist) / (IP(_FogEnd) - IP(_FogStart)));	
								return o;		
							#endif // USE_DIST_FOG
						#else // USE_FOG
							return customLightingWPosVertSimple(v, 
							IP(InstProperties,_MainColor), 
							IP(InstProperties,_RimColor), 
							IP(InstProperties,_RimPower), 
							IP(InstProperties,_RightLight), 
							IP(InstProperties,_FrontLight), 
							IP(InstProperties,_TopLight), 
							IP(InstProperties,_AmbientColor), 
							IP(InstProperties,_AmbientPower));
						#endif // USE_FOG
					#else // LIGHTING_FULL
						#ifdef USE_FOG
							#ifndef USE_DIST_FOG
								return customLightingSoftFogVert(v, 
								IP(InstProperties,_RimColor), 
								IP(InstProperties,_RimPower), 
								IP(InstProperties,_RightColor), 
								IP(InstProperties,_FrontColor), 
								IP(InstProperties,_TopColor),
								IP(InstProperties,_AmbientColor), 
								IP(InstProperties,_AmbientPowe)r,
								 _FogYStartPos, 
								 _FogAnimationHeight, 
								 _FogAnimationFreq);
							#else
								CL_OUT_WPOS o=customLightingSoftFogVert(v, 
								IP(InstProperties,_RimColor, 
								IP(InstProperties,_RimPower, 
								IP(InstProperties,_RightColor, 
								IP(InstProperties,_FrontColor, 
								IP(InstProperties,_TopColor, 
								IP(InstProperties,_AmbientColor, 
								IP(InstProperties,_AmbientPower,
								_FogYStartPos, 
								_FogAnimationHeight,
								 _FogAnimationFreq);
								#ifndef FOG_STATIC_START_POS 
								float cameraVertDist = length(_WorldSpaceCameraPos - o.wpos)*IP(_FogDensity); 
								#else
								float cameraVertDist = length((_FogStaticStartPos) - o.wpos)*IP(_FogDensity); 
								#endif 
								o.color.w = saturate((IP(_FogEnd) - cameraVertDist) / (IP(_FogEnd) - IP(_FogStart)));	
								return o;		
							#endif
						#else
							return customLightingWPosVert(v, 
							IP(InstProperties,_RimColor), 
							IP(InstProperties,_RimPower), 
							IP(InstProperties,_RightColor), 
							IP(InstProperties,_FrontColor), 
							IP(InstProperties,_TopColor), 
							IP(InstProperties,_AmbientColor), 
							IP(InstProperties,_AmbientPower));
						#endif
					#endif // LIGHTING_FULL


				}
				
				fixed4 frag(CL_OUT_WPOS v) : COLOR {
					fixed4 o = fixed4(1,1,1,1);
					#ifdef USE_FOG
						#ifndef USE_DIST_FOG
							o = customLightingSoftFogFrag(v, 
							IP(InstProperties,_FogColor), 
							(_FogHeight), 
							_LightTint, 
							_UseLightMap, 
							_LightmapPower, 
							_LightmapColor, 
							_ShadowPower);
						#else
							fixed4 c = customLightingSoftFogFrag(v, 
							IP(InstProperties,_FogColor), 
							(_FogHeight), 
							IP(InstProperties,_LightTint), 
							_UseLightMap, 
							_LightmapPower, 
							_LightmapColor, 
							_ShadowPower);
							o = lerp(half4(IP(InstProperties,_FogColor),1),c,v.color.w);
							o.a = c.a;
						#endif
					#else
						o = customLightingFrag(v, 
						IP(InstProperties,_LightTint), 
						_UseLightMap, 
						_LightmapPower, 
						_LightmapColor, 
						_ShadowPower);
					#endif

					#ifdef TRANSPARENT
						o.a *=  (1 - IP(InstProperties,_Alpha));
						#ifdef CUTOUT
							clip (o.a - IP(_Cutoff));
						#endif
						

					#endif 

					return o;
				}
			ENDCG
		}
	}

	FallBack "Diffuse"
	CustomEditor "CustomLightingMasterGUI"
}
