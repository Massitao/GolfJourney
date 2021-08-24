#ifndef Game2D_WaterKit_INCLUDED
#define Game2D_WaterKit_INCLUDED

#define Water2D_HasWaterTextureSheet defined(Water2D_WaterTextureSheet) || defined(Water2D_WaterTextureSheetWithLerp)
#define Water2D_HasWaterTexture  defined(Water2D_WaterTexture) || Water2D_HasWaterTextureSheet
#define Water2D_HasSurfaceTextureSheet defined(Water2D_SurfaceTextureSheet) || defined(Water2D_SurfaceTextureSheetWithLerp)
#define Water2D_HasSurfaceTexture  defined(Water2D_SurfaceTexture) || Water2D_HasSurfaceTextureSheet

			#if defined(Water2D_Refraction) || defined(Water2D_Reflection)
				CBUFFER_START(Water2D_FrequentlyUpdatedVariables)
					#if defined(Water2D_Refraction)
					uniform sampler2D _RefractionTexture;
					#endif

					#if defined(Water2D_Reflection)
					uniform sampler2D _ReflectionTexture;
					uniform fixed _ReflectionLowerLimit;
					#endif

					uniform float4x4 _WaterMVP;
				CBUFFER_END
			#endif

			#if defined(Water2D_Refraction) || defined(Water2D_Reflection) || (Water2D_HasWaterTexture && defined(Water2D_WaterNoise)) || (defined(Water2D_Surface) && Water2D_HasSurfaceTexture && defined(Water2D_SurfaceNoise))
				sampler2D _NoiseTexture:register(s0);
				float4 _NoiseTexture_ST;
			#endif

			CBUFFER_START(Water2D_RarelyUpdatedVariables)
				#if defined(Water2D_Refraction) 
				half _RefractionNoiseSpeed;
				#endif

				#if defined(Water2D_Reflection) 
				half _ReflectionNoiseSpeed;

					#if defined(Water2D_Refraction)
						fixed _ReflectionVisibility;
					#endif

				#endif

				#if defined(Water2D_Surface)
				fixed _SurfaceLevel;
				fixed4 _SurfaceColor;

					#if Water2D_HasSurfaceTexture
						sampler2D _SurfaceTexture;
						float4 _SurfaceTexture_ST;
						fixed _SurfaceTextureOpacity;

						#if defined(Water2D_SurfaceNoise)
							half _SurfaceNoiseSpeed;
						#endif

						#if Water2D_HasSurfaceTextureSheet
							half _SurfaceTextureSheetFramesPerSecond;
							half _SurfaceTextureSheetColumns;
							half _SurfaceTextureSheetRows;
							half _SurfaceTextureSheetFramesCount;
							half _SurfaceTextureSheetInverseColumns;
							half _SurfaceTextureSheetInverseRows;
						#endif

					#endif

				#endif

				#if defined(Water2D_ColorGradient)
					fixed4 _WaterColorGradientStart;
					fixed4 _WaterColorGradientEnd;
				#else
					fixed4 _WaterColor;
				#endif

				#if Water2D_HasWaterTexture
					sampler2D _WaterTexture;
					float4 _WaterTexture_ST;
					fixed _WaterTextureOpacity;

					#if defined(Water2D_WaterNoise)
						half _WaterNoiseSpeed;
					#endif

					#if Water2D_HasWaterTextureSheet
						half _WaterTextureSheetFramesPerSecond;
						half _WaterTextureSheetColumns;
						half _WaterTextureSheetRows;
						half _WaterTextureSheetFramesCount;
						half _WaterTextureSheetInverseColumns;
						half _WaterTextureSheetInverseRows;
					#endif
				#endif

				#if defined(Water2D_ApplyEmissionColor)
						fixed3 _WaterEmissionColor;
						fixed _WaterEmissionColorIntensity;
				#endif

			CBUFFER_END

			struct water2D_VertexInput
			{
				float4 pos : POSITION;
				#if defined(Water2D_Surface) || defined(Water2D_ColorGradient)
				float2 uv : TEXCOORD0;
				#endif
				#if  defined(LIGHTMAP_ON)
				float2 lightmapCoord : TEXCOORD1;
				#endif
			};

			struct Water2D_VertexOutput
			{
				float4 pos : SV_POSITION;

				#if defined(Water2D_Refraction) || defined(Water2D_Reflection) || defined(Water2D_Surface) || defined(Water2D_ColorGradient)
					#if defined(Water2D_Surface) || defined(Water2D_ColorGradient)
						float4 uv : TEXCOORD0;
					#else
						#if	defined(Water2D_Reflection)
							float3 uv : TEXCOORD0;
						#else
							float2 uv : TEXCOORD0;
						#endif
					#endif
				#endif

				#if defined(Water2D_Refraction) || defined(Water2D_Reflection) 
					#if defined(Water2D_Reflection) 
						float4 refractionReflectionUV : TEXCOORD1;
					#else
						float2 refractionReflectionUV : TEXCOORD1;
					#endif
				#endif

				#if Water2D_HasWaterTexture
					#if defined(Water2D_WaterNoise)
						float4 waterTextureUV : TEXCOORD2;
					#else
						float2 waterTextureUV : TEXCOORD2;
					#endif
				#endif

				#if defined(Water2D_Surface) && Water2D_HasSurfaceTexture
					#if defined(Water2D_SurfaceNoise)
						float4 surfaceTextureUV : TEXCOORD3;
					#else
						float2 surfaceTextureUV : TEXCOORD3;
					#endif
				#endif

				#if Water2D_HasWaterTextureSheet
					#if defined(Water2D_WaterTextureSheetWithLerp)
						float4 waterTextureSheetUV : TEXCOORD4;
					#else
						float2 waterTextureSheetUV : TEXCOORD4;
					#endif
				#endif

				#if Water2D_HasSurfaceTextureSheet
					#if defined(Water2D_SurfaceTextureSheetWithLerp)
						float4 surfaceTextureSheetUV : TEXCOORD5;
					#else
						float2 surfaceTextureSheetUV : TEXCOORD5;
					#endif
				#endif

				#if defined(LIGHTMAP_ON)
				    float2 lightmapCoord : TEXCOORD6;
				#else
					#if defined(UNITY_SHOULD_SAMPLE_SH)
			 		half3 sh : TEXCOORD6;
					#endif
				#endif

				#if defined(UNITY_PASS_FORWARDBASE) || defined(UNITY_PASS_FORWARDADD)
  				float3 worldPos : TEXCOORD7;
 				#endif

				UNITY_FOG_COORDS(8)

				#if defined(Water2D_LIGHT_ON)
				fixed3 lightColor : COLOR0;
				#endif
			};

			inline Water2D_VertexOutput Water2D_Vert(water2D_VertexInput v){
				Water2D_VertexOutput o;
 				UNITY_INITIALIZE_OUTPUT(Water2D_VertexOutput,o);
				
				o.pos = UnityObjectToClipPos(v.pos.xyz);

				#if defined(Water2D_Refraction) || defined(Water2D_Reflection)
					float4 pos = mul(_WaterMVP,v.pos);
					o.uv.xy = pos.xy * 0.5 + 0.5;
					#if defined(Water2D_Reflection)
						o.uv.z = v.pos.y;
					#endif
				#endif

				#if defined(Water2D_Surface) || defined(Water2D_ColorGradient)
					o.uv.w = v.uv.y;
				#endif

				#if defined(Water2D_Refraction) || defined(Water2D_Reflection) || (defined(Water2D_Surface) && Water2D_HasSurfaceTexture) || Water2D_HasWaterTexture
				float2 vertexPositionWorldSpace = mul(unity_ObjectToWorld, v.pos);
				#endif

				#if defined(Water2D_Refraction) 
					o.refractionReflectionUV.xy = TRANSFORM_TEX((vertexPositionWorldSpace + _Time.w * _RefractionNoiseSpeed), _NoiseTexture);
				#endif

				#if defined(Water2D_Reflection) 
					o.refractionReflectionUV.zw = TRANSFORM_TEX((vertexPositionWorldSpace + _Time.w * _ReflectionNoiseSpeed), _NoiseTexture);
				#endif

				#if defined(Water2D_Surface) && Water2D_HasSurfaceTexture
					o.surfaceTextureUV.xy = TRANSFORM_TEX(vertexPositionWorldSpace,_SurfaceTexture);

					#if Water2D_HasSurfaceTextureSheet
						half surfaceFrame = fmod(_Time.y * _SurfaceTextureSheetFramesPerSecond, _SurfaceTextureSheetFramesCount);
						half surfaceCurrentFrame = floor(surfaceFrame);
						o.surfaceTextureSheetUV.x = fmod(surfaceCurrentFrame, _SurfaceTextureSheetColumns) * _SurfaceTextureSheetInverseColumns;
						o.surfaceTextureSheetUV.y = -(floor(surfaceCurrentFrame * _SurfaceTextureSheetInverseColumns) * _SurfaceTextureSheetInverseRows);
						#if defined(Water2D_SurfaceTextureSheetWithLerp)
							half surfaceNextFrame = floor(fmod(surfaceFrame + 1.0, _SurfaceTextureSheetFramesCount));
							o.surfaceTextureSheetUV.z = fmod(surfaceNextFrame, _SurfaceTextureSheetColumns) * _SurfaceTextureSheetInverseColumns;
							o.surfaceTextureSheetUV.w = -(floor(surfaceNextFrame * _SurfaceTextureSheetInverseColumns) * _SurfaceTextureSheetInverseRows);
						#endif
					#endif

					#if defined(Water2D_SurfaceNoise)
						o.surfaceTextureUV.zw = TRANSFORM_TEX((o.surfaceTextureUV.xy + _Time.w * _SurfaceNoiseSpeed),_NoiseTexture);
					#endif

				#endif

				#if Water2D_HasWaterTexture
					o.waterTextureUV.xy = TRANSFORM_TEX(vertexPositionWorldSpace, _WaterTexture);
					
					#if Water2D_HasWaterTextureSheet
						half waterFrame = fmod(_Time.y * _WaterTextureSheetFramesPerSecond, _WaterTextureSheetFramesCount);
						half waterCurrentFrame = floor(waterFrame);
						o.waterTextureSheetUV.x = fmod(waterCurrentFrame, _WaterTextureSheetColumns) * _WaterTextureSheetInverseColumns;
						o.waterTextureSheetUV.y = -(floor(waterCurrentFrame * _WaterTextureSheetInverseColumns) * _WaterTextureSheetInverseRows);
						#if defined(Water2D_WaterTextureSheetWithLerp)
							half waterNextFrame = floor(fmod(waterFrame + 1.0, _WaterTextureSheetFramesCount));
							o.waterTextureSheetUV.z = fmod(waterNextFrame, _WaterTextureSheetColumns) * _WaterTextureSheetInverseColumns;
							o.waterTextureSheetUV.w = -(floor(waterNextFrame * _WaterTextureSheetInverseColumns) * _WaterTextureSheetInverseRows);
						#endif
					#endif

					#if defined(Water2D_WaterNoise)
						o.waterTextureUV.zw = TRANSFORM_TEX((o.waterTextureUV.xy + _Time.w * _WaterNoiseSpeed),_NoiseTexture);
					#endif

				#endif
				return o;
			}

			inline fixed4 Water2D_Frag(Water2D_VertexOutput i){
				fixed4 finalColor = fixed4(0.0,0.0,0.0,1.0);

				#if defined(Water2D_Surface)
					bool isSurface = false;
					if(i.uv.w > _SurfaceLevel){
						isSurface = true;
					}
				#endif


					//Refraction

					#if defined(Water2D_Refraction)
						float refractionDistortion = tex2D(_NoiseTexture,i.refractionReflectionUV.xy).r;
						fixed4 refractionColor = tex2D(_RefractionTexture,float2(i.uv.xy + refractionDistortion));
						finalColor = refractionColor;
					#endif

					//Reflection

					#if defined(Water2D_Reflection)
						i.uv.y = 1.0 - i.uv.y;
						float reflectionDistortion = tex2D(_NoiseTexture,i.refractionReflectionUV.zw).g;
						fixed4 reflectionColor = tex2D(_ReflectionTexture,float2(i.uv.xy + reflectionDistortion));
						reflectionColor *= step(i.uv.z, _ReflectionLowerLimit);

						#if defined(Water2D_Refraction)
							finalColor.rgb += _ReflectionVisibility * reflectionColor.a * (reflectionColor.rgb - finalColor.rgb);
						#else
							finalColor = reflectionColor;
						#endif
					#endif

				//Applying water surface texture

				#if defined (Water2D_Surface) && Water2D_HasSurfaceTexture
					if(isSurface){

						#if defined(Water2D_SurfaceNoise)
							i.surfaceTextureUV.xy += tex2D(_NoiseTexture,i.surfaceTextureUV.zw).b;
						#endif

						#if Water2D_HasSurfaceTextureSheet
								i.surfaceTextureUV.xy = frac(i.surfaceTextureUV.xy) * float2(_SurfaceTextureSheetInverseColumns, _SurfaceTextureSheetInverseRows);
								#if defined(Water2D_SurfaceTextureSheetWithLerp)
									fixed4 currentFrameColor = tex2D(_SurfaceTexture, float2(i.surfaceTextureUV.x + i.surfaceTextureSheetUV.x, i.surfaceTextureUV.y + i.surfaceTextureSheetUV.y));
									fixed4 nextFrameColor = tex2D(_SurfaceTexture, float2(i.surfaceTextureUV.x + i.surfaceTextureSheetUV.z, i.surfaceTextureUV.y + i.surfaceTextureSheetUV.w));
									half delta = frac(fmod(_Time.y * _SurfaceTextureSheetFramesPerSecond, _SurfaceTextureSheetFramesCount));
									fixed4 sampledColor = lerp(currentFrameColor, nextFrameColor, delta);
								#else
									fixed4 sampledColor = tex2D(_SurfaceTexture, float2(i.surfaceTextureUV.x + i.surfaceTextureSheetUV.x, i.surfaceTextureUV.y + i.surfaceTextureSheetUV.y));
								#endif
							#else
								fixed4 sampledColor = tex2D(_SurfaceTexture,i.surfaceTextureUV.xy);
							#endif

							#if defined(Water2D_Refraction) || defined(Water2D_Reflection)
								finalColor.rgb += _SurfaceTextureOpacity * sampledColor.a * (sampledColor.rgb - finalColor.rgb);
							#else 
								finalColor = _SurfaceTextureOpacity * sampledColor;
							#endif
					}
				#endif

				//Applying water body texture

				#if Water2D_HasWaterTexture

					#if defined(Water2D_Surface)
						if(!isSurface){
					#endif

							#if defined(Water2D_WaterNoise)
								i.waterTextureUV.xy += tex2D(_NoiseTexture,i.waterTextureUV.zw).a;
							#endif

							#if Water2D_HasWaterTextureSheet
								i.waterTextureUV.xy = frac(i.waterTextureUV.xy) * float2(_WaterTextureSheetInverseColumns, _WaterTextureSheetInverseRows);
								#if defined(Water2D_WaterTextureSheetWithLerp)
									fixed4 currentFrameColor = tex2D(_WaterTexture, float2(i.waterTextureUV.x + i.waterTextureSheetUV.x, i.waterTextureUV.y + i.waterTextureSheetUV.y));
									fixed4 nextFrameColor = tex2D(_WaterTexture, float2(i.waterTextureUV.x + i.waterTextureSheetUV.z, i.waterTextureUV.y + i.waterTextureSheetUV.w));
									half delta = frac(fmod(_Time.y * _WaterTextureSheetFramesPerSecond, _WaterTextureSheetFramesCount));
									fixed4 sampledColor = lerp(currentFrameColor, nextFrameColor, delta);
								#else
									fixed4 sampledColor = tex2D(_WaterTexture, float2(i.waterTextureUV.x + i.waterTextureSheetUV.x, i.waterTextureUV.y + i.waterTextureSheetUV.y));
								#endif
							#else
								fixed4 sampledColor = tex2D(_WaterTexture,i.waterTextureUV.xy);
							#endif

							#if defined(Water2D_Refraction) || defined(Water2D_Reflection)
								finalColor.rgb += _WaterTextureOpacity * sampledColor.a * (sampledColor.rgb - finalColor.rgb);
							#else
								finalColor = _WaterTextureOpacity * sampledColor;
							#endif

					#if defined(Water2D_Surface)
							}
					#endif

				#endif


				//Appying water surface color

				#if defined(Water2D_Surface)
					if(isSurface)
					{
						#if Water2D_HasSurfaceTexture || defined(Water2D_Refraction) || defined(Water2D_Reflection)
							finalColor += _SurfaceColor.a * (_SurfaceColor - finalColor);
						#else
							finalColor = _SurfaceColor;
						#endif
					}else{
				#endif

						//Applying water body color

						fixed4 waterColor;
						#if defined(Water2D_ColorGradient)
							#if defined (Water2D_Surface)
								waterColor = _WaterColorGradientEnd + (i.uv.w / _SurfaceLevel) * (_WaterColorGradientStart - _WaterColorGradientEnd);
							#else
								waterColor = _WaterColorGradientEnd + i.uv.w * (_WaterColorGradientStart - _WaterColorGradientEnd);
							#endif
						#else
							waterColor = _WaterColor;
						#endif

						#if Water2D_HasWaterTexture || defined(Water2D_Refraction) || defined(Water2D_Reflection)
							finalColor += waterColor.a * (waterColor - finalColor);
						#else
							finalColor = waterColor;
						#endif


				#if defined(Water2D_Surface)
					}
				#endif

				return finalColor;
			}

	#if defined(Water2D_LIGHT_ON) && (defined(UNITY_PASS_VERTEX) || defined(UNITY_PASS_VERTEXLM))
			int4 unity_VertexLightParams;

			// ES2.0/WebGL/3DS can not do loops with non-constant-expression iteration counts.
			#if defined(SHADER_API_GLES)
			  #define LIGHT_LOOP_LIMIT 8
			#elif defined(SHADER_API_N3DS)
			  #define LIGHT_LOOP_LIMIT 4
			#else
			  #define LIGHT_LOOP_LIMIT unity_VertexLightParams.x // x: vertex lights count
			#endif

			// Compute attenuation & illumination from one light
			#if defined(POINT) || defined(SPOT)
				inline half3 computeLight(int idx, float3 mvPos) {
			#else
				inline half3 computeLight(int idx) {
			#endif
			  float4 lightPos = unity_LightPosition[idx];
			  float3 dirToLight = lightPos.xyz;
			  half attenuation = 1.0;

			  #if defined(POINT) || defined(SPOT)
			    dirToLight -= mvPos * lightPos.w;
			    // distance attenuation

			    half4 lightAtten = unity_LightAtten[idx];
			    float distSqr = dot(dirToLight, dirToLight);
			    if (lightPos.w != 0.0 && distSqr > lightAtten.w) 
			    	attenuation = 0.0; // set to 0 if outside of range
			    else
			    	attenuation /= (1.0 + lightAtten.z * distSqr);

			    distSqr = max(distSqr, 0.000001); // don't produce NaNs if some vertex position overlaps with the light 
			    dirToLight *= rsqrt(distSqr);
			    #if defined(SPOT)
			      // spot angle attenuation
			      half rho = max(dot(dirToLight, unity_SpotDirection[idx].xyz), 0.0);
			      half spotAtt = (rho - lightAtten.x) * lightAtten.y;
			      attenuation *= saturate(spotAtt);
			    #endif
			  #endif

			  // Compute illumination from one light, given attenuation
			  attenuation *= max(dirToLight.z, 0.0);
			  return attenuation * unity_LightColor[idx].rgb;
			}
	#endif //Water2D_LIGHT_ON && (UNITY_PASS_VERTEX || UNITY_PASS_VERTEXLM)

#endif // Game2D_WaterKit_INCLUDED
