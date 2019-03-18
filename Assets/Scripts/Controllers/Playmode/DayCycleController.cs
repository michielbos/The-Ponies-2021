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

        [Range(0f, 24f)] public float _StartingSunrise = 7f;
        [Range(0f, 24f)] public float _StartingDay = 12f;
        [Range(0f, 24f)] public float _StartingSunset = 18f;
        [Range(0f, 24f)] public float _StartingNight = 22f;

        [Range(0f, 1f)] public float _SunriseLightIntensity = 0.6f;
        [Range(0f, 1f)] public float _DayLightIntensity = 1.0f;
        [Range(0f, 1f)] public float _SunsetLightIntensity = 0.7f;
        [Range(0f, 1f)] public float _NightLightIntensity = 0.4f;

        public Color _Sunrise_LightColor;
        public Color _Day_LightColor;
        public Color _Sunset_LightColor;
        public Color _Night_LightColor;

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
            UpdateTime();
        }

        public void UpdateTime()
        {
            float hour = SpeedController.Instance.GetHourOfDay();
            float minute = SpeedController.Instance.GetMinuteOfHour();

            currentTime = hour + (minute/60);
        }

        void Update()
        {

            UpdateTime();

            UpdateSunAndMoon();
            UpdateTimeset();
            UpdateWeather();

        }

        void UpdateSunAndMoon()
        {
            //need some tweaks here

            // Rotates the sun 360 degree in X-axis according to our current time of day.
            SunLight.transform.localRotation = Quaternion.Euler((currentTime / 24 * 360f) - 90, 170, 0);

            NightLight.transform.localRotation = Quaternion.Euler((currentTime / 24* 360f) + 80, 170, 0);

        }

        void UpdateTimeset()
        {
            if (currentTime >= _StartingSunrise && currentTime <= _StartingDay && CurrTimeset != Timeset.SUNRISE)
            {
                SetCurrentTimeset(Timeset.SUNRISE);
                SunLight.enabled = true;
                NightLight.enabled = false;
            }
            else if (currentTime >= _StartingDay && currentTime <= _StartingSunset && CurrTimeset != Timeset.DAY)
            {
                SetCurrentTimeset(Timeset.DAY);
                SunLight.enabled = true;
                NightLight.enabled = false;
            }
            else if (currentTime >= _StartingSunset && currentTime <= _StartingNight && CurrTimeset != Timeset.SUNSET)
            {
                SetCurrentTimeset(Timeset.SUNSET);
                SunLight.enabled = true;
                NightLight.enabled = false;
            }
            else if (currentTime >= _StartingNight || currentTime <= _StartingSunrise && CurrTimeset != Timeset.NIGHT)
            {
                SetCurrentTimeset(Timeset.NIGHT);
                SunLight.enabled = false;
                NightLight.enabled = true;
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
                UpdateAllWeather(_SunriseLightIntensity, _Sunrise_LightColor, 0.0f, _Night_LightColor, _DefaultFadeTime);

            }
            else if (CurrTimeset == Timeset.DAY)
            {
                UpdateAllWeather(_DayLightIntensity, _Day_LightColor, 0.0f, _Night_LightColor, _DefaultFadeTime);

            }
            else if (CurrTimeset == Timeset.SUNSET)
            {
                UpdateAllWeather(_SunsetLightIntensity, _Sunset_LightColor, 0.0f, _Night_LightColor, _DefaultFadeTime);
            }
            else if (CurrTimeset == Timeset.NIGHT)
            {
                UpdateAllWeather(_NightLightIntensity, _Night_LightColor, _NightLightIntensity, _Night_LightColor, _DefaultFadeTime);

            }
        }

        private void DifferentFadeTimes()
        {
            if (CurrTimeset == Timeset.SUNRISE)
            {
                UpdateAllWeather(_SunriseLightIntensity, _Sunrise_LightColor, 0.0f, _Night_LightColor, _SunriseFadeTime);
            }
            else if (CurrTimeset == Timeset.DAY)
            {
                UpdateAllWeather(_DayLightIntensity, _Day_LightColor, 0.0f, _Night_LightColor, _DayFadeTime);

            }
            else if (CurrTimeset == Timeset.SUNSET)
            {
                UpdateAllWeather(_SunsetLightIntensity, _Sunset_LightColor, 0.0f, _Night_LightColor, _SunsetFadeTime);

            }
            else if (CurrTimeset == Timeset.NIGHT)
            {
                UpdateAllWeather(_NightLightIntensity, _Night_LightColor, _NightLightIntensity, _Night_LightColor, _NightFadeTime);

            }
        }

        public void UpdateAllWeather(float sunIntensity, Color sunLightColor, float moonIntensity, Color moonLightColor, float fadeTime)
        {
            // Sun Light
            SunLight.intensity = Mathf.Lerp(SunLight.intensity, sunIntensity, Time.deltaTime / fadeTime);
            SunLight.color = Color.Lerp(SunLight.color, sunLightColor, Time.deltaTime / fadeTime);

            NightLight.intensity = Mathf.Lerp(NightLight.intensity, moonIntensity, Time.deltaTime / fadeTime);
            NightLight.color = Color.Lerp(NightLight.color, moonLightColor, Time.deltaTime / fadeTime);

        }


    }
}