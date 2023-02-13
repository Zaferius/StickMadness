// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "DCG/Toon/Simple (CutOut)"
{
	Properties
	{
		_Diffuse("Diffuse", 2D) = "white" {}
		_Tint("Tint", Color) = (1,1,1,1)
		_MaskClip("Mask Clip ", Range( 0 , 1)) = 0.5
		[HDR]_OutlineTint("Outline Tint", Color) = (0,0,0,0)
		_OutlineSize("Outline Size", Range( 0 , 1)) = 0
		[Normal]_Normal("Normal", 2D) = "bump" {}
		_Gloss("Gloss", Range( 0 , 1)) = 0.1
		[NoScaleOffset]_Occlusion("Occlusion", 2D) = "white" {}
		_OcclusionIntensity("Occlusion Intensity", Range( 0 , 1)) = 0.6
		_Emission("Emission", 2D) = "black" {}
		[HDR]_EmissionTint("Emission Tint", Color) = (0,0,0,0)
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "TransparentCutout"  "Queue" = "AlphaTest+0" "IgnoreProjector" = "True" "IsEmissive" = "true"  }
		LOD 200
		Cull Off
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		#ifdef UNITY_PASS_SHADOWCASTER
			#undef INTERNAL_DATA
			#undef WorldReflectionVector
			#undef WorldNormalVector
			#define INTERNAL_DATA half3 internalSurfaceTtoW0; half3 internalSurfaceTtoW1; half3 internalSurfaceTtoW2;
			#define WorldReflectionVector(data,normal) reflect (data.worldRefl, half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal)))
			#define WorldNormalVector(data,normal) half3(dot(data.internalSurfaceTtoW0,normal), dot(data.internalSurfaceTtoW1,normal), dot(data.internalSurfaceTtoW2,normal))
		#endif
		struct Input
		{
			float2 uv_texcoord;
			float3 worldPos;
			float3 worldNormal;
			INTERNAL_DATA
			float3 worldRefl;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			half3 Albedo;
			half3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			half Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform sampler2D _Emission;
		uniform float4 _Emission_ST;
		uniform float4 _EmissionTint;
		uniform sampler2D _Diffuse;
		uniform float4 _Diffuse_ST;
		uniform float4 _Tint;
		uniform float _OutlineSize;
		uniform float4 _OutlineTint;
		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform float _Gloss;
		uniform sampler2D _Occlusion;
		uniform float _OcclusionIntensity;
		uniform float _MaskClip;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			#ifdef UNITY_PASS_FORWARDBASE
			float ase_lightAtten = data.atten;
			if( _LightColor0.a == 0)
			ase_lightAtten = 0;
			#else
			float3 ase_lightAttenRGB = gi.light.color / ( ( _LightColor0.rgb ) + 0.000001 );
			float ase_lightAtten = max( max( ase_lightAttenRGB.r, ase_lightAttenRGB.g ), ase_lightAttenRGB.b );
			#endif
			#if defined(HANDLE_SHADOWS_BLENDING_IN_GI)
			half bakedAtten = UnitySampleBakedOcclusion(data.lightmapUV.xy, data.worldPos);
			float zDist = dot(_WorldSpaceCameraPos - data.worldPos, UNITY_MATRIX_V[2].xyz);
			float fadeDist = UnityComputeShadowFadeDistance(data.worldPos, zDist);
			ase_lightAtten = UnityMixRealtimeAndBakedShadows(data.atten, bakedAtten, UnityComputeShadowFade(fadeDist));
			#endif
			float2 uv_Diffuse = i.uv_texcoord * _Diffuse_ST.xy + _Diffuse_ST.zw;
			float4 tex2DNode1 = tex2D( _Diffuse, uv_Diffuse );
			float4 temp_output_18_0 = ( tex2DNode1 * _Tint );
			float3 ase_worldPos = i.worldPos;
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aseld
			float3 ase_worldlightDir = 0;
			#else //aseld
			float3 ase_worldlightDir = Unity_SafeNormalize( UnityWorldSpaceLightDir( ase_worldPos ) );
			#endif //aseld
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			float3 tex2DNode4 = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			float dotResult6 = dot( ase_worldlightDir , normalize( (WorldNormalVector( i , tex2DNode4 )) ) );
			float smoothstepResult8 = smoothstep( 0.01 , 0.03 , dotResult6);
			float smoothstepResult9 = smoothstep( 0.25 , 0.26 , dotResult6);
			float clampResult11 = clamp( smoothstepResult9 , 0.5 , 1.0 );
			#if defined(LIGHTMAP_ON) && UNITY_VERSION < 560 //aselc
			float4 ase_lightColor = 0;
			#else //aselc
			float4 ase_lightColor = _LightColor0;
			#endif //aselc
			float4 temp_output_16_0 = ( ase_lightColor * ase_lightAtten );
			float dotResult23 = dot( ase_worldlightDir , normalize( WorldReflectionVector( i , tex2DNode4 ) ) );
			float smoothstepResult28 = smoothstep( 0.19 , 0.2 , pow( dotResult23 , exp2( (1.0 + (_Gloss - 0.0) * (11.0 - 1.0) / (1.0 - 0.0)) ) ));
			UnityGI gi41 = gi;
			float3 diffNorm41 = WorldNormalVector( i , tex2DNode4 );
			gi41 = UnityGI_Base( data, 1, diffNorm41 );
			float3 indirectDiffuse41 = gi41.indirect.diffuse + diffNorm41 * 0.0001;
			float2 uv_Occlusion36 = i.uv_texcoord;
			float lerpResult39 = lerp( 1.0 , tex2D( _Occlusion, uv_Occlusion36 ).r , _OcclusionIntensity);
			float smoothstepResult50 = smoothstep( ( _OutlineSize - 0.01 ) , _OutlineSize , tex2DNode1.a);
			float temp_output_54_0 = saturate( smoothstepResult50 );
			float4 lerpResult59 = lerp( _OutlineTint , ( ( ( ( temp_output_18_0 * ( smoothstepResult8 * clampResult11 ) * temp_output_16_0 ) + ( smoothstepResult28 * _Gloss * temp_output_16_0 ) ) + ( float4( indirectDiffuse41 , 0.0 ) * temp_output_18_0 * 0.25 ) ) * lerpResult39 ) , temp_output_54_0);
			c.rgb = lerpResult59.rgb;
			c.a = 1;
			clip( saturate( tex2DNode1.a ) - _MaskClip );
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			o.Normal = float3(0,0,1);
			float2 uv_Emission = i.uv_texcoord * _Emission_ST.xy + _Emission_ST.zw;
			float2 uv_Diffuse = i.uv_texcoord * _Diffuse_ST.xy + _Diffuse_ST.zw;
			float4 tex2DNode1 = tex2D( _Diffuse, uv_Diffuse );
			float4 temp_output_18_0 = ( tex2DNode1 * _Tint );
			float smoothstepResult50 = smoothstep( ( _OutlineSize - 0.01 ) , _OutlineSize , tex2DNode1.a);
			float temp_output_54_0 = saturate( smoothstepResult50 );
			o.Emission = ( ( ( tex2D( _Emission, uv_Emission ) * _EmissionTint ) + ( UNITY_LIGHTMODEL_AMBIENT * temp_output_18_0 ) ) * temp_output_54_0 ).rgb;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows exclude_path:deferred 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float2 customPack1 : TEXCOORD1;
				float4 tSpace0 : TEXCOORD2;
				float4 tSpace1 : TEXCOORD3;
				float4 tSpace2 : TEXCOORD4;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				Input customInputData;
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				half3 worldNormal = UnityObjectToWorldNormal( v.normal );
				half3 worldTangent = UnityObjectToWorldDir( v.tangent.xyz );
				half tangentSign = v.tangent.w * unity_WorldTransformParams.w;
				half3 worldBinormal = cross( worldNormal, worldTangent ) * tangentSign;
				o.tSpace0 = float4( worldTangent.x, worldBinormal.x, worldNormal.x, worldPos.x );
				o.tSpace1 = float4( worldTangent.y, worldBinormal.y, worldNormal.y, worldPos.y );
				o.tSpace2 = float4( worldTangent.z, worldBinormal.z, worldNormal.z, worldPos.z );
				o.customPack1.xy = customInputData.uv_texcoord;
				o.customPack1.xy = v.texcoord;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				return o;
			}
			half4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				surfIN.uv_texcoord = IN.customPack1.xy;
				float3 worldPos = float3( IN.tSpace0.w, IN.tSpace1.w, IN.tSpace2.w );
				half3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = float3( IN.tSpace0.z, IN.tSpace1.z, IN.tSpace2.z );
				surfIN.worldRefl = -worldViewDir;
				surfIN.internalSurfaceTtoW0 = IN.tSpace0.xyz;
				surfIN.internalSurfaceTtoW1 = IN.tSpace1.xyz;
				surfIN.internalSurfaceTtoW2 = IN.tSpace2.xyz;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				UnityGI gi;
				UNITY_INITIALIZE_OUTPUT( UnityGI, gi );
				o.Alpha = LightingStandardCustomLighting( o, worldViewDir, gi ).a;
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	//CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=16100
2067;62;1710;1011;2646.462;347.7744;1.252335;True;True
Node;AmplifyShaderEditor.SamplerNode;4;-2119.723,409.0343;Float;True;Property;_Normal;Normal;5;1;[Normal];Create;True;0;0;False;0;None;None;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldNormalVector;2;-1597.92,423.677;Float;False;True;1;0;FLOAT3;0,0,1;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;3;-1647.92,280.6771;Float;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.RangedFloatNode;25;-1493.801,1106.973;Float;False;Property;_Gloss;Gloss;6;0;Create;True;0;0;False;0;0.1;0.414;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;6;-1353.036,344.0935;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.WorldSpaceLightDirHlpNode;22;-1175.098,746.0082;Float;False;True;1;0;FLOAT;0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.WorldReflectionVector;21;-1138.164,904.6682;Float;False;True;1;0;FLOAT3;0,0,0;False;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.TFHCRemapNode;26;-1130.801,1155.973;Float;False;5;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;1;False;3;FLOAT;1;False;4;FLOAT;11;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;9;-1165.074,415.2291;Float;False;3;0;FLOAT;0;False;1;FLOAT;0.25;False;2;FLOAT;0.26;False;1;FLOAT;0
Node;AmplifyShaderEditor.DotProductOpNode;23;-852.0978,809.0082;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT;0
Node;AmplifyShaderEditor.Exp2OpNode;27;-887.8012,1168.973;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;8;-1162.074,273.2291;Float;False;3;0;FLOAT;0;False;1;FLOAT;0.01;False;2;FLOAT;0.03;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;17;-1979.736,59.56795;Float;False;Property;_Tint;Tint;1;0;Create;True;0;0;False;0;1,1,1,1;0.7830189,0.7793254,0.7793254,1;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.LightColorNode;14;-995.0769,-225.2099;Float;False;0;3;COLOR;0;FLOAT3;1;FLOAT;2
Node;AmplifyShaderEditor.LightAttenuation;15;-1046.077,-94.2099;Float;False;0;1;FLOAT;0
Node;AmplifyShaderEditor.PowerNode;24;-685.3356,954.171;Float;False;2;0;FLOAT;0;False;1;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.ClampOpNode;11;-975.0736,425.2291;Float;False;3;0;FLOAT;0;False;1;FLOAT;0.5;False;2;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;1;-2053.745,-134.9099;Float;True;Property;_Diffuse;Diffuse;0;0;Create;True;0;0;False;0;None;0000000000000000f000000000000000;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;12;-809.0736,317.2291;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;18;-1621.992,27.96976;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;28;-454.9344,937.9422;Float;False;3;0;FLOAT;0;False;1;FLOAT;0.19;False;2;FLOAT;0.2;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;16;-704.0769,-219.2099;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;46;-365.4356,251.9606;Float;False;Constant;_Float0;Float 0;8;0;Create;True;0;0;False;0;0.25;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;29;-230.6584,939.1172;Float;False;3;3;0;FLOAT;0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;51;189.2106,670.6191;Float;False;Property;_OutlineSize;Outline Size;4;0;Create;True;0;0;False;0;0;0.929;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.IndirectDiffuseLighting;41;-290.7891,105.6355;Float;False;Tangent;1;0;FLOAT3;0,0,1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;13;-684.5181,54.5285;Float;False;3;3;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;2;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;53;307.2106,868.6191;Float;False;Constant;_Float2;Float 2;10;0;Create;True;0;0;False;0;0.01;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;33;-293.9812,-272.1781;Float;False;Property;_EmissionTint;Emission Tint;10;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FogAndAmbientColorsNode;19;-1597.062,-107.4682;Float;False;UNITY_LIGHTMODEL_AMBIENT;0;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;44;-93.7337,215.4565;Float;False;3;3;0;FLOAT3;0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RelayNode;49;5.422894,50.54048;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;37;-250.0891,1284.8;Float;False;Constant;_Float1;Float 1;5;0;Create;True;0;0;False;0;1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;36;-385.1713,1412.335;Float;True;Property;_Occlusion;Occlusion;7;1;[NoScaleOffset];Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;38;-302.0891,1651.8;Float;False;Property;_OcclusionIntensity;Occlusion Intensity;8;0;Create;True;0;0;False;0;0.6;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;32;-306.9812,-502.1781;Float;True;Property;_Emission;Emission;9;0;Create;True;0;0;False;0;None;None;True;0;False;black;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleSubtractOpNode;52;474.2106,808.6191;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;31;-139.8173,388.2564;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleAddOpNode;45;104.5644,262.9606;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;39;-18.08909,1432.8;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;34;69.0188,-257.1781;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SmoothstepOpNode;50;477.394,537.3746;Float;False;3;0;FLOAT;0;False;1;FLOAT;0.89;False;2;FLOAT;0.9;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;20;-1247.062,-51.4682;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;40;93.31311,530.9118;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;54;821.8924,505.8019;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;57;943.7311,597.7395;Float;False;Property;_OutlineTint;Outline Tint;3;1;[HDR];Create;True;0;0;False;0;0,0,0,0;0,0,0,0;True;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;35;313.2554,-191.1197;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;55;361.8951,-77.68673;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.LerpOp;59;475.7813,355.3076;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;47;280.9852,85.23939;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;48;-280.7413,-647.5808;Float;False;Property;_MaskClip;Mask Clip ;2;0;Create;True;0;0;False;0;0.5;0.858;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;686.2297,-139.4314;Float;False;True;2;Float;ASEMaterialInspector;200;0;CustomLighting;DCG/Toon/Simple (CutOut);False;False;False;False;False;False;False;False;False;False;False;False;False;False;True;False;False;False;False;False;Off;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Masked;0.5;True;True;0;False;TransparentCutout;;AlphaTest;ForwardOnly;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;True;0;0;False;-1;0;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0.02;0,0,0,0;VertexOffset;False;False;Cylindrical;False;Relative;200;;-1;-1;-1;-1;0;False;0;0;False;-1;-1;0;True;48;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;2;0;4;0
WireConnection;6;0;3;0
WireConnection;6;1;2;0
WireConnection;21;0;4;0
WireConnection;26;0;25;0
WireConnection;9;0;6;0
WireConnection;23;0;22;0
WireConnection;23;1;21;0
WireConnection;27;0;26;0
WireConnection;8;0;6;0
WireConnection;24;0;23;0
WireConnection;24;1;27;0
WireConnection;11;0;9;0
WireConnection;12;0;8;0
WireConnection;12;1;11;0
WireConnection;18;0;1;0
WireConnection;18;1;17;0
WireConnection;28;0;24;0
WireConnection;16;0;14;0
WireConnection;16;1;15;0
WireConnection;29;0;28;0
WireConnection;29;1;25;0
WireConnection;29;2;16;0
WireConnection;41;0;4;0
WireConnection;13;0;18;0
WireConnection;13;1;12;0
WireConnection;13;2;16;0
WireConnection;44;0;41;0
WireConnection;44;1;18;0
WireConnection;44;2;46;0
WireConnection;49;0;1;4
WireConnection;52;0;51;0
WireConnection;52;1;53;0
WireConnection;31;0;13;0
WireConnection;31;1;29;0
WireConnection;45;0;31;0
WireConnection;45;1;44;0
WireConnection;39;0;37;0
WireConnection;39;1;36;1
WireConnection;39;2;38;0
WireConnection;34;0;32;0
WireConnection;34;1;33;0
WireConnection;50;0;49;0
WireConnection;50;1;52;0
WireConnection;50;2;51;0
WireConnection;20;0;19;0
WireConnection;20;1;18;0
WireConnection;40;0;45;0
WireConnection;40;1;39;0
WireConnection;54;0;50;0
WireConnection;35;0;34;0
WireConnection;35;1;20;0
WireConnection;55;0;35;0
WireConnection;55;1;54;0
WireConnection;59;0;57;0
WireConnection;59;1;40;0
WireConnection;59;2;54;0
WireConnection;47;0;49;0
WireConnection;0;2;55;0
WireConnection;0;10;47;0
WireConnection;0;13;59;0
ASEEND*/
//CHKSM=945010EB44C5AABDC187AA905E724ACDDE7BB166