Shader "Custom/kinectShader"
{
	
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
		[NoScaleOffset] _MainTex ("Texture", 2D) = "white" {}
		[NoScaleOffset] _DepthText ("Depth Info", 2D) = "black" {}
		_MinDepth("Min Depth (mm)", Int) = 0
		_MaxDepth("Max Depth (mm)", Int) = 5000
		_VertexOffset("Vertex Offset", Int) = 10
		_SampleOffset("Sample Offset", Float) = 0.01
		_NormalZValue("NormalZ Value", Float) = 0.001
		_NormalCutoff("Normal Cutoff", Float) = 0.1
		_DepthCutoff("Depth Cutoff", Float) = 0.6


		_RimColor("Rim Color", Color) = (0.26,0.19,0.16,0.0)
		_RimPower("Rim Power", Range(0.5,8.0)) = 3.0

    }
    SubShader
	{
	
		
			Tags { "RenderType" = "Opaque" }

			
			CGPROGRAM
			#pragma surface surf Lambert vertex:vert

			
			#include "UnityCG.cginc"
			#include "UnityLightingCommon.cginc" // for _LightColor0

			

			struct Input {
				float2 uv_MainTex;
				float3 viewDir;
			};


			sampler2D _MainTex;
			sampler2D _DepthTex;
			int _MinDepth;
			int _MaxDepth;
			int _VertexOffset;
			float _SampleOffset;
			float _NormalZValue;
			float _NormalCutoff;
			float _DepthCutoff;

			float4 _RimColor;
			float _RimPower;


			void vert(inout appdata_full v)
			{
				
				//get bunch of samples from the Depth Texture
				float4 tex0 = tex2Dlod(_DepthTex, float4(v.texcoord.xy, 0, 0));
				float4 tex1 = tex2Dlod(_DepthTex, float4(v.texcoord.xy + float2(-_SampleOffset, 0), 0, 0));
				float4 tex2 = tex2Dlod(_DepthTex, float4(v.texcoord.xy + float2(_SampleOffset, 0), 0, 0));
				float4 tex3 = tex2Dlod(_DepthTex, float4(v.texcoord.xy + float2(0,-_SampleOffset), 0, 0));
				float4 tex4 = tex2Dlod(_DepthTex, float4(v.texcoord.xy + float2(0, _SampleOffset), 0, 0));
				
				float depth = (tex0.r +tex1.r + tex2.r+tex3.r)/4.0;
				float2 depthRange = float2(_MinDepth, _MaxDepth) / 65535.;
				// Linear grayscale coloring along specified depth range, from white (near) to black (far)
				depth = clamp(depth, depthRange.x, depthRange.y);
				float c = (depthRange.y - depth) / (depthRange.y - depthRange.x);
				// Pixels of unknown depth are rendered transparent
				float a = depth == 0 ? 0 : 1;

				v.vertex.z = - c * _VertexOffset + _VertexOffset *0.5;
				


				// get vertex normal in world space
				float3 myNormal = float3(tex1.r - tex2.r, tex3.r - tex4.r, -_NormalZValue);
				//myNormal = UnpackNormal(tex2Dlod(_DepthTex, float4(v.texcoord.xy,0,0)));
				//myNormal = float3(0, 0, -1);
				myNormal = normalize(myNormal);
				
				if (a<0.5 || myNormal.z > _NormalCutoff || c > _DepthCutoff || c < 0.01) {
					v.color = float4(0, 0, 0, 0);
					v.vertex.z = 0.0 / 0.0;
				}
				else
				{
					v.color = float4(1, 1, 1, 1);
				}

				//v.vertex = UnityObjectToClipPos(v.vertex);
				
				//possibly flip Normal in y-Axis
				myNormal.y = -myNormal.y;

				half3 worldNormal = UnityObjectToWorldNormal(myNormal);
				//worldNormal.y = -worldNormal.y;

				v.normal = worldNormal;
				/*
				v.normal.x = myNormal.x;
				v.normal.y = myNormal.y;
				v.normal.z = myNormal.z;
				*/
				// dot product between normal and light direction for
				// standard diffuse (Lambert) lighting
				//half nl = max(0, dot(worldNormal, _WorldSpaceLightPos0.xyz));
				// factor in the light color
				//o.diff = nl * _LightColor0;


				//return o;
			}


			void surf(Input IN, inout SurfaceOutput o) {
				float4 col = tex2D(_MainTex, IN.uv_MainTex * 32).rgba;
				o.Albedo = col.rgb;
				if (col.a < 0.9) {
					discard;
				}
				o.Normal = UnpackNormal(tex2D(_DepthTex, IN.uv_MainTex));
				//half rim = 1.0 - saturate(dot(normalize(IN.viewDir), o.Normal));
				//o.Emission = _RimColor.rgb * pow(rim, _RimPower);
			}
			ENDCG

	}
    
}
