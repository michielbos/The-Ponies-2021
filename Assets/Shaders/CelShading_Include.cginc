#ifndef Celshading_included
	#define Celshading_included
	
	float _RampThreshold;
	float _RampSmooth;

	//Highlight/Shadow Colors
	fixed4 _HColor;
	fixed4 _SColor;
	
#endif

inline half4 LightingCelshading (SurfaceOutput s, half3 lightDir, half atten)
{
	fixed ndl = max(0, dot(s.Normal, lightDir));
	fixed3 ramp = smoothstep(_RampThreshold-_RampSmooth*0.5, _RampThreshold+_RampSmooth*0.5, ndl);
#if !(POINT) && !(SPOT)
	ramp *= atten;
#endif
	_SColor = lerp(_HColor, _SColor, _SColor.a);	//Shadows intensity through alpha
	ramp = lerp(_SColor.rgb,_HColor.rgb,ramp);
	fixed4 c;
	c.rgb = s.Albedo * _LightColor0.rgb * ramp;
#if (POINT || SPOT)
	c.rgb *= atten;
#endif
	c.a = s.Alpha;
	return c;
}
