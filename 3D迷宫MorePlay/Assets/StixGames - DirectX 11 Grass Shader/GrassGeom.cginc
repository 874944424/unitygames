﻿// Upgrade NOTE: replaced '_World2Object' with 'unity_WorldToObject'

#ifndef GRASS_GEOM
#define GRASS_GEOM

FS_INPUT geomToFrag(GS_OUTPUT v)
{
	float3 worldPos = v.vertex.xyz;

	//This is necessary for shadow calculation
	v.vertex = mul(unity_WorldToObject, v.vertex);

	#ifdef UNITY_COMPILER_HLSL
	FS_INPUT o = (FS_INPUT) 0;
	#else
	FS_INPUT o;
	#endif

	o.worldPos = worldPos;

	#if !defined(SIMPLE_GRASS) && !defined(SIMPLE_GRASS_DENSITY)
		o.uv = v.uv;
		o.texIndex = v.texIndex;
	#endif

	#ifndef SHADOWPASS
		o.pos = mul(UNITY_MATRIX_MVP, v.vertex);

		o.color = v.color;

		o.normal = v.normal;
		o.reflectionNormal = v.reflectionNormal;

		o.lightDir = v.lightDir;
		o.viewDir =  v.viewDir;

		
		TRANSFER_SHADOW(o); // pass shadow coordinates to pixel shader
		UNITY_TRANSFER_FOG(o, o.pos); // pass fog coordinates to pixel shader
	#elif defined(UNITY_PASS_SHADOWCASTER)
		TRANSFER_SHADOW_CASTER(o)
	#endif
	return o;
}

[maxvertexcount(MAX_VERTEX_COUNT)]
void geom (point GS_INPUT p[1], inout TriangleStream<FS_INPUT> triStream)
{
	float3 rendererPos = p[0].cameraPos;
	float3 cameraPos = _WorldSpaceCameraPos;

	//Init pos, lightDir and uv
	fixed3 lightDir = p[0].lightDir;
	float3 oPos = p[0].position.xyz;
	fixed4 uv = fixed4(p[0].uv, 0, 0);

	//This variable is used for calculating random values. If you have a better name for it, I'm all ears!
#ifdef GRASS_OBJECT_MODE
	float3 randCalcPos = p[0].objectSpacePos;
#else
	float3 randCalcPos = oPos;
#endif

	//Define LightDir
	#ifndef USING_DIRECTIONAL_LIGHT
		lightDir = normalize(UnityWorldSpaceLightDir(oPos));
	#endif

	//Calculate viewDir and groundRight vector
	fixed3 up = fixed3(0, 1, 0);
	fixed3 viewDir = normalize(rendererPos - oPos);
	
	//I'll keep this in case I ever want to orient the grass in another direction.
	fixed3 orientationDir = viewDir;

	//Grass variable declaration
	fixed maxHeight;
	fixed minHeight;
	fixed width;
	fixed softness;
	#ifndef SHADOWPASS
		fixed4 mainColor;
		fixed4 secColor;
	#endif
	
	#if defined(SIMPLE_GRASS) || defined(SIMPLE_GRASS_DENSITY)
		#if defined(SIMPLE_GRASS_DENSITY)
		
		#ifndef UNIFORM_DENSITY
		fixed4 density = tex2Dlod(_Density, uv);
		#endif

		float randVal = rand(randCalcPos.xz + float2(1789, -2360));

		if(randVal < DENSITY00)
		{
		#endif
			maxHeight = _MaxHeight00;
			minHeight = _MinHeight00;
			width = _Width00;
			softness = _Softness00;
			#ifndef SHADOWPASS
				mainColor = _Color00;
				secColor = _SecColor00;
			#endif
		#if defined(SIMPLE_GRASS_DENSITY)
		}
		else
		{
			#ifdef UNITY_COMPILER_HLSL
				GS_OUTPUT pIn = (GS_OUTPUT)0;
			#else
				GS_OUTPUT pIn;
			#endif

			triStream.Append(geomToFrag(pIn));
			return;
		}
		#endif
	#else //If textured grass
		//Grass Type
		//Selects a random type of grass. If the probability is over 1, it will be scaled down.
		#ifndef UNIFORM_DENSITY
		fixed4 density = tex2Dlod(_Density, uv);
		#endif

		#ifdef FOUR_GRASS_TYPES
			float randVal = rand(randCalcPos.xz + float2(1789, -2360)) * max(DENSITY00 + DENSITY01 + DENSITY02 + DENSITY03, 1);
		#elif defined(THREE_GRASS_TYPES)
			float randVal = rand(randCalcPos.xz + float2(1789, -2360)) * max(DENSITY00 + DENSITY01 + DENSITY02, 1);
		#elif defined(TWO_GRASS_TYPES)
			float randVal = rand(randCalcPos.xz + float2(1789, -2360)) * max(DENSITY00 + DENSITY01, 1);
		#else
			float randVal = rand(randCalcPos.xz + float2(1789, -2360));
		#endif
		int texIndex;
		
		if(randVal < DENSITY00)
		{
			texIndex = 0;
			maxHeight = _MaxHeight00;
			minHeight = _MinHeight00;
			width = _Width00;
			softness = _Softness00;
			#ifndef SHADOWPASS
				mainColor = _Color00;
				secColor = _SecColor00;
			#endif
		}
		#if !defined(ONE_GRASS_TYPE)
		else if(randVal < (DENSITY00 + DENSITY01))
		{
			texIndex = 1;
			maxHeight = _MaxHeight01;
			minHeight = _MinHeight01;
			width = _Width01;
			softness = _Softness01;
			#ifndef SHADOWPASS
				mainColor = _Color01;
				secColor = _SecColor01;
			#endif
		}
		#if !defined(TWO_GRASS_TYPES)
		else if(randVal < (DENSITY00 + DENSITY01 + DENSITY02))
		{
			texIndex = 2;
			maxHeight = _MaxHeight02;
			minHeight = _MinHeight02;
			width = _Width02;
			softness = _Softness02;
			#ifndef SHADOWPASS
				mainColor = _Color02;
				secColor = _SecColor02;
			#endif
		}
		#if !defined(THREE_GRASS_TYPES)
		else if(randVal < (DENSITY00 + DENSITY01 + DENSITY02 + DENSITY03))
		{
			texIndex = 3;
			maxHeight = _MaxHeight03;
			minHeight = _MinHeight03;
			width = _Width03;
			softness = _Softness03;
			#ifndef SHADOWPASS
				mainColor = _Color03;
				secColor = _SecColor03;
			#endif
		}
		#endif
		#endif
		#endif
		else
		{
			//If no grass type was randomized, return a single vertex, so no blade of grass will be rendered.
			#ifdef UNITY_COMPILER_HLSL
				GS_OUTPUT pIn = (GS_OUTPUT)0;
			#else
				GS_OUTPUT pIn;
			#endif

			pIn.texIndex = -1;

			triStream.Append(geomToFrag(pIn));
			return;
		}
	#endif

	fixed3 groundRight = normalize(cross(up, orientationDir));

	//Init position offset
	fixed randX = rand(randCalcPos.xz + 1000) * _Disorder * 2 - _Disorder;
	fixed randZ = rand(randCalcPos.xz - 1000) * _Disorder * 2 - _Disorder;
				
	fixed2 windDir = wind(randCalcPos, fixed2(randX, randZ));

	//Grass height modifier
	fixed4 tex = tex2Dlod(_ColorMap, uv);

	fixed dist = distance(oPos, _WorldSpaceCameraPos);
	fixed grassHeightMod = tex.a * smoothstep(_GrassFadeEnd, _GrassFadeStart, dist);
	
	//Smooth height
	#ifdef GRASS_HEIGHT_SMOOTHING
		grassHeightMod *= p[0].smoothing;
	#endif

	//Calculate real height
	fixed realHeight = (rand(randCalcPos.xz) * (maxHeight - minHeight) + minHeight) * grassHeightMod;

	//Smooth width
	#ifdef GRASS_WIDTH_SMOOTHING
		width *= p[0].smoothing;
	#endif

	//Color
	#ifndef SHADOWPASS
		fixed4 color = tex * lerp(mainColor, secColor, rand(randCalcPos.xz + fixed2(1000, -1000)));
	#endif
	
	//LOD
	fixed lod = (int)p[0].lod;
	fixed invLod = 1.0f/lod;

	GS_OUTPUT pIn;
	float3 lastLeftPos = oPos;

	//Define first vertices most values only have to be defined on the first vertex and will be used for the others as well
	pIn.vertex = float4(lastLeftPos,1);
	getNormals(up, lightDir, groundRight, /*out*/ pIn.normal, /*out*/ pIn.reflectionNormal);

	#if !defined(SIMPLE_GRASS) && !defined(SIMPLE_GRASS_DENSITY)
		pIn.uv = fixed2(0.0f, 0.0f);
		pIn.texIndex = texIndex;
	#endif
	
	#ifndef SHADOWPASS
		pIn.color = color * _GrassBottomColor;

		pIn.lightDir = lightDir;
		pIn.viewDir = viewDir;
	#endif
	triStream.Append(geomToFrag(pIn));

	pIn.vertex =  float4((oPos + width * groundRight).xyz, 1);
	getNormals(up, lightDir, groundRight, /*out*/ pIn.normal, /*out*/ pIn.reflectionNormal);

	#if !defined(SIMPLE_GRASS) && !defined(SIMPLE_GRASS_DENSITY)
	pIn.uv = fixed2(1.0f, 0.0f);
	pIn.texIndex = texIndex;
	#endif

	triStream.Append(geomToFrag(pIn));

	fixed stiffnessFactor = realHeight * softness;

	#ifdef GRASS_DISPLACEMENT
	fixed4 displacement = tex2Dlod(_Displacement, uv);

	//Convert from texture to vector
	displacement.xy = (displacement.xy * 2.0) - fixed2(1, 1);
	#endif

	for(fixed i = 1; i <= lod; i++)
	{
		fixed segment = i*invLod;
		fixed sqrSegment = segment*segment;

		float3 pos = float3(up*segment*realHeight);

		#ifdef GRASS_DISPLACEMENT
			pos.xz += (windDir.xy * pow(displacement.z, 3) + displacement.xy) * sqrSegment * stiffnessFactor;
			pos.y -= length(windDir.xy + displacement.xy) * sqrSegment * 0.5f * stiffnessFactor;
			pos.y *= max(displacement.z,0.01);
		#else
			pos.xz += windDir.xy * sqrSegment * stiffnessFactor;
			pos.y  -= length(windDir) * sqrSegment * 0.5f * stiffnessFactor;
		#endif

		pos += oPos;

		fixed uvHeight = segment;

		#ifndef USING_DIRECTIONAL_LIGHT
			lightDir = normalize(UnityWorldSpaceLightDir(pos));
		#endif
		viewDir = normalize(rendererPos - pos);

		fixed3 localUp = pos - lastLeftPos;

		//Vertex definition
		pIn.vertex =  float4(pos, 1);
		getNormals(localUp, lightDir, groundRight, /*out*/ pIn.normal, /*out*/ pIn.reflectionNormal);

		#if !defined(SIMPLE_GRASS) && !defined(SIMPLE_GRASS_DENSITY)
			pIn.uv = fixed2(0.0f, uvHeight);
			pIn.texIndex = texIndex;
		#endif

		#ifndef SHADOWPASS
			pIn.color = color * lerp(_GrassBottomColor, fixed4(1, 1, 1, 1), segment);

			pIn.lightDir = lightDir;
			pIn.viewDir = viewDir;
		#endif
		triStream.Append(geomToFrag(pIn));

		//Vertex definition
		#if defined(SIMPLE_GRASS) || defined(SIMPLE_GRASS_DENSITY)
			//Simple grass has no texture, so the mesh has to look like a blade of grass
			pIn.vertex =  float4((pos + width * groundRight * (1 - sqrSegment)).xyz, 1);
		#else
			pIn.vertex =  float4((pos + width * groundRight).xyz, 1);
		#endif
		getNormals(localUp, lightDir, groundRight, /*out*/ pIn.normal, /*out*/ pIn.reflectionNormal);

		#if !defined(SIMPLE_GRASS) && !defined(SIMPLE_GRASS_DENSITY)
			pIn.uv = fixed2(1.0f, uvHeight);
			pIn.texIndex = texIndex;
		#endif
		
		triStream.Append(geomToFrag(pIn));

		lastLeftPos = pos;
	}
				
	triStream.RestartStrip();
}
#endif