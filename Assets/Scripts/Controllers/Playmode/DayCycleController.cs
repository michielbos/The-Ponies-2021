using UnityEngine;
using System.Collections;
using System;
using Assets.Scripts.Util;

namespace Assets.Scripts.Controllers
{
    public class DayCycleController : VolatileSingletonController<DayCycleController>
    {

        public Light SunLight;
        public Light NightLight;
        private float currentTime;

        private float fHoursbeforeMidnight;
        private float fHoursAfterMidnight;
        private float nightTime;
        private float dayTime;

        private float xRot;
        private float angleCompensation = 25;

        private float fSunrisePhase;
        private float fNoonPhase;

        [Range(0f, 24f)] public float _StartingSunrise = 7f;
        [Range(0f, 24f)] public float _StartingDay = 12f;
        [Range(0f, 24f)] public float _StartingSunset = 18f;
        [Range(0f, 24f)] public float _StartingNight = 22f;

        [Range(0f, 1f)] public float _SunriseLightIntensity = 0.6f;
        [Range(0f, 1f)] public float _DayLightIntensity = 1.0f;
        [Range(0f, 1f)] public float _SunsetLightIntensity = 0.7f;
        [Range(0f, 1f)] public float _NightLightIntensity = 0f;
		[Range(0f, 1f)] public float _NightMoonLightIntensity = 0.4f;

		//Sun
        public Color _Sunrise_LightColor;
        public Color _Day_LightColor;
        public Color _Sunset_LightColor;
        public Color _Night_LightColor;
		//Moon
		public Color _MoonNight_LightColor;
		
		// Skybox
		public Color _Sunrise_SkyTintColor = Color.yellow;
		public Color _Day_SkyTintColor = Color.cyan;
		public Color _Sunset_SkyTintColor = Color.red;
		public Color _Night_SkyTintColor = Color.grey;

        public float _SunriseShadowStrength = 0.25f;
        public float _DayShadowStrength = 0.45f;
        public float _SunsetShadowStrength = 0.6f;
        public float _NightShadowStrength = 0f;


        public bool _UseDifferentFadeTimes;
        public float _DefaultFadeTime = 45.0f;
        public float _SunriseFadeTime = 5.0f;
        public float _DayFadeTime = 5.0f;
        public float _SunsetFadeTime = 5.0f;
        public float _NightFadeTime = 5.0f;
        [HideInInspector]
        public Timeset CurrTimeset;

        public enum Timeset
        {
            SUNRISE,
            DAY,
            SUNSET,
            NIGHT
        };

        void Start()
        {

            if ((24 - _StartingNight) > 0 && 24 - _StartingNight < 12)
            {
                fHoursbeforeMidnight = 24 - _StartingNight;
            }

            fHoursAfterMidnight = _StartingSunrise;

            fSunrisePhase = ((24 - _StartingSunrise) - (24 - _StartingDay));
            fNoonPhase = ((24 - _StartingDay) - (24 - _StartingNight));

            UpdateTime();
        }

        public void UpdateTime()
        {
            float hour = SpeedController.Instance.GetHourOfDay();
            float minute = SpeedController.Instance.GetMinuteOfHour();
            float second = SpeedController.Instance.GetSecondOfMinute();

            currentTime = hour + ((minute / 60) + (second/60/60));

        }

        void Update()
        {

            UpdateTime();

            UpdateTimeset();
            UpdateLightFadeOut();
            UpdateWeather();
            UpdateSunAndMoon();
        }

        void UpdateSunAndMoon()
        {
			//SunLight.transform.localRotation = Quaternion.Euler((currentTime/24 * 360) - 65, 170, 0);

            //NightLight.transform.localRotation = Quaternion.Euler((currentTime/24 * 360) - 245, 170, 0);

            if (currentTime < _StartingSunrise || currentTime > _StartingNight)
            {
                if (currentTime < 24 && currentTime > _StartingNight)
                {
                    nightTime = currentTime - _StartingNight;
                    xRot = Mathf.Lerp(5f, 90f, nightTime / fHoursbeforeMidnight);
                }
                else if (currentTime < _StartingSunrise)
                {
                    xRot = Mathf.Lerp(90f, 175f, currentTime / fHoursAfterMidnight);
                }

                NightLight.transform.localRotation = Quaternion.Euler(xRot + angleCompensation, 170, 0);
            }
            else
            {
                if (currentTime > _StartingSunrise && currentTime < _StartingDay)
                {
                    dayTime = currentTime - _StartingSunrise;
                    xRot = Mathf.Lerp(5f, 90f, dayTime / fSunrisePhase);
                }
                else if (currentTime > _StartingDay)
                {
                    dayTime = currentTime - _StartingDay;
                    xRot = Mathf.Lerp(90f, 175f, dayTime / fNoonPhase);
                }
                 
                SunLight.transform.localRotation = Quaternion.Euler(xRot + angleCompensation, 170, 0);
            }


        }

        void UpdateTimeset()
        {
            if (currentTime >= _StartingSunrise && currentTime <= _StartingDay && CurrTimeset != Timeset.SUNRISE)
            {
                SetCurrentTimeset(Timeset.SUNRISE);
            }
            else if (currentTime >= _StartingDay && currentTime <= _StartingSunset && CurrTimeset != Timeset.DAY)
            {
                SetCurrentTimeset(Timeset.DAY);
            }
            else if (currentTime >= _StartingSunset && currentTime <= _StartingNight && CurrTimeset != Timeset.SUNSET)
            {
                SetCurrentTimeset(Timeset.SUNSET);
            }
            else if (currentTime >= _StartingNight || currentTime <= _StartingSunrise && CurrTimeset != Timeset.NIGHT)
            {
                SetCurrentTimeset(Timeset.NIGHT);
            }
        }

        void UpdateLightFadeOut()
        {

            if (CurrTimeset == Timeset.SUNRISE)
            {
                NightLight.shadowStrength = Mathf.Lerp(NightLight.shadowStrength, 0, Time.deltaTime / 10f);
                SunLight.shadowStrength = Mathf.Lerp(SunLight.shadowStrength, _SunriseShadowStrength, Time.deltaTime / 30f);
                if (NightLight.shadowStrength <= 0.05 && NightLight.enabled == true)
                {
                    SunLight.enabled = true;
                    NightLight.enabled = false;
                }
            }
            if (CurrTimeset == Timeset.DAY)
            {
                NightLight.shadowStrength = Mathf.Lerp(NightLight.shadowStrength, 0, Time.deltaTime / 30f);
                SunLight.shadowStrength = Mathf.Lerp(SunLight.shadowStrength, _DayShadowStrength, Time.deltaTime / 30f);
                if (NightLight.shadowStrength <= 0.05 && NightLight.enabled == true)
                {
                    SunLight.enabled = true;
                    NightLight.enabled = false;
                }
            }
            if (CurrTimeset == Timeset.SUNSET)
            {
                NightLight.shadowStrength = Mathf.Lerp(NightLight.shadowStrength, 0, Time.deltaTime / 30f);
                SunLight.shadowStrength = Mathf.Lerp(SunLight.shadowStrength, _SunsetShadowStrength, Time.deltaTime / 30f);
                if (NightLight.shadowStrength <= 0.05 && NightLight.enabled == true)
                {
                    SunLight.enabled = true;
                    NightLight.enabled = false;
                }
            }
            if (CurrTimeset == Timeset.NIGHT)
            {
                SunLight.shadowStrength = Mathf.Lerp(SunLight.shadowStrength, 0,Time.deltaTime / 5f);
                NightLight.shadowStrength = Mathf.Lerp(NightLight.shadowStrength, _NightShadowStrength, Time.deltaTime / 15f);

                if (SunLight.shadowStrength <= 0.1 && SunLight.shadowStrength > 0.05)
                    NightLight.enabled = true;
                if (SunLight.shadowStrength <= 0.05 && SunLight.enabled == true)
                {
                    SunLight.enabled = false;
                    if (NightLight.enabled == false)
                        NightLight.enabled = true;
                }
            }
        }
        void SetCurrentTimeset(Timeset currentTime)
        {
            CurrTimeset = currentTime;
        }

        public void UpdateWeather()
        {

            if (_UseDifferentFadeTimes == false)
            {
                OneFadeTimeToRuleThemAll();
            }
            else
            {
                DifferentFadeTimes();
            }
        }

        private void OneFadeTimeToRuleThemAll()
        {
            if (CurrTimeset == Timeset.SUNRISE)
            {
				
				UpdateAllWeather(_SunriseLightIntensity, _Sunrise_LightColor, 0.0f, _MoonNight_LightColor, _Sunrise_SkyTintColor, _DefaultFadeTime);

            }
            else if (CurrTimeset == Timeset.DAY)
            {
                UpdateAllWeather(_DayLightIntensity, _Day_LightColor, 0.0f, _Night_LightColor, _Day_SkyTintColor, _DefaultFadeTime);

            }
            else if (CurrTimeset == Timeset.SUNSET)
            {
                UpdateAllWeather(_SunsetLightIntensity, _Sunset_LightColor, 0.0f, _Night_LightColor, _Sunset_SkyTintColor, _DefaultFadeTime);
            }
            else if (CurrTimeset == Timeset.NIGHT)
            {
                UpdateAllWeather(_NightLightIntensity, _Night_LightColor, _NightMoonLightIntensity, _Night_LightColor, _Night_SkyTintColor, _DefaultFadeTime);

            }
        }

        private void DifferentFadeTimes()
        {
            if (CurrTimeset == Timeset.SUNRISE)
            {
                UpdateAllWeather(_SunriseLightIntensity, _Sunrise_LightColor, 0.0f, _Night_LightColor, _Sunrise_SkyTintColor, _SunriseFadeTime);
            }
            else if (CurrTimeset == Timeset.DAY)
            {
                UpdateAllWeather(_DayLightIntensity, _Day_LightColor, 0.0f, _Night_LightColor, _Day_SkyTintColor, _DayFadeTime);

            }
            else if (CurrTimeset == Timeset.SUNSET)
            {
                UpdateAllWeather(_SunsetLightIntensity, _Sunset_LightColor, 0.0f, _Night_LightColor, _Sunset_SkyTintColor, _SunsetFadeTime);

            }
            else if (CurrTimeset == Timeset.NIGHT)
            {
                UpdateAllWeather(_NightLightIntensity, _Night_LightColor, _NightLightIntensity, _Night_LightColor, _Night_SkyTintColor, _NightFadeTime);

            }
        }

        public void UpdateAllWeather(float sunIntensity, Color sunLightColor, float moonIntensity, Color moonLightColor, Color skyTint, float fadeTime)
        {
            // Sun Light
            SunLight.intensity = Mathf.Lerp(SunLight.intensity, sunIntensity, Time.deltaTime / fadeTime);
            SunLight.color = Color.Lerp(SunLight.color, sunLightColor, Time.deltaTime / fadeTime);

            NightLight.intensity = Mathf.Lerp(NightLight.intensity, moonIntensity, Time.deltaTime / fadeTime);
            NightLight.color = Color.Lerp(NightLight.color, moonLightColor, Time.deltaTime / fadeTime);
			
			// Skybox settings
			RenderSettings.skybox.SetColor("_Tint", Color.Lerp(RenderSettings.skybox.GetColor("_Tint"), skyTint, Time.deltaTime / fadeTime));
        }


    }
}