using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Controllers {

public class DayCycleController : SingletonMonoBehaviour<DayCycleController> {
    public Light SunLight;
    public Light NightLight;
    public Material skyboxMaterialBase;

    private float currentTime;

    private float fHoursbeforeMidnight;
    private float fHoursAfterMidnight;
    private float nightTime;
    private float dayTime;

    private float xRot;
    private float angleCompensation = 0;

    private float fSunrisePhase;
    private float fNoonPhase;

    private Material skyboxMaterial;

    [System.Serializable]
    public class LightSettings {
        ///***Starting times***
        [Header("Time Cycle")] [Range(0f, 24f)]
        public float startingSunrise = 6f;

        [Range(0f, 24f)] public float endingSunrise = 10f;
        [Range(0f, 24f)] public float startingDay = 12f;
        [Range(0f, 24f)] public float startingSunset = 18f;
        [Range(0f, 24f)] public float startingNight = 20f;

        [Header("Light Intensity")] [Range(0f, 1f)]
        public float sunriseLightIntensity = 0.6f;

        [Range(0f, 1f)] public float endingSunriseLightIntensity = 0.8f;
        [Range(0f, 1f)] public float dayLightIntensity = 1.0f;
        [Range(0f, 1f)] public float sunsetLightIntensity = 0.7f;
        [Range(0f, 1f)] public float nightLightIntensity = 0f;
        [Range(0f, 1f)] public float nightMoonLightIntensity = 0.4f;

        ///***COLORS***
        [Header("Light Colors")]
        //Sun
        public Color sunriseLightColor;

        public Color dayLightColor;
        public Color sunsetLightColor;

        public Color nightLightColor;

        //Moon
        public Color moonNightLightColor;

        [Header("Skybox Colors")]
        // Skybox
        public Color sunriseSkyTintColor;

        public Color daySkyTintColor;
        public Color sunsetSkyTintColor;
        public Color nightSkyTintColor;
    }

    private LightSettings currentSettings;
    public LightSettings sripngSettings;
    public LightSettings summerSettings;
    public LightSettings autumnSettings;
    public LightSettings winterSettings;

    private float sunriseLightIntensity;
    private Color sunriseLightColor;

    private float sunriseShadowStrength = 0.25f;
    private float dayShadowStrength = 0.45f;
    private float sunsetShadowStrength = 0.4f;
    private float nightShadowStrength = 0.1f;

    public GameObject sunriseParticle;
    public GameObject dayParticle;
    public GameObject sunsetParticle;
    public GameObject nightParticle;

    public float fadeTime = 45.0f;

    [HideInInspector] public Timeset currTimeset;
    [HideInInspector] public Seasons currSeason;

    private static readonly int TintProperty = Shader.PropertyToID("_Tint");

    public enum Timeset {
        SUNRISE,
        SUNRISEENDING,
        DAY,
        SUNSET,
        NIGHT
    };

    public enum Seasons {
        SPRING,
        SUMMER,
        AUTUMN,
        WINTER
    };

    void Start() {
        skyboxMaterial = new Material(skyboxMaterialBase);
        RenderSettings.skybox = skyboxMaterial;
        currentSettings = sripngSettings;

        UpdateTime();
    }

    public void UpdateTime() {
        float hour = SpeedController.Instance.GetHourOfDay();
        float minute = SpeedController.Instance.GetMinuteOfHour();
        float second = SpeedController.Instance.GetSecondOfMinute();

        currentTime = hour + ((minute / 60) + (second / 60 / 60));
    }

    void Update() {
        UpdateTime();

        UpdateSeasons();
        UpdateTimeset();
        UpdateLightFadeOut();
        UpdateWeather();
        UpdateSunAndMoon();
    }

    void UpdateSunAndMoon() {
        if (currentTime < currentSettings.startingSunrise || currentTime > currentSettings.startingNight) {
            if (currentTime < 24 && currentTime > currentSettings.startingNight) {
                nightTime = currentTime - currentSettings.startingNight;
                xRot = Mathf.Lerp(45f, 90f, nightTime / fHoursbeforeMidnight);
            } else if (currentTime < currentSettings.startingSunrise) {
                xRot = Mathf.Lerp(90f, 175f, currentTime / fHoursAfterMidnight);
            }

            NightLight.transform.localRotation = Quaternion.Euler(xRot + angleCompensation, 170, 0);
        } else {
            if (currentTime > currentSettings.startingSunrise && currentTime < currentSettings.startingDay) {
                dayTime = currentTime - currentSettings.startingSunrise;
                xRot = Mathf.Lerp(5f, 90f, dayTime / fSunrisePhase);
            } else if (currentTime > currentSettings.startingDay) {
                dayTime = currentTime - currentSettings.startingDay;
                xRot = Mathf.Lerp(90f, 175f, dayTime / fNoonPhase);
            }

            SunLight.transform.localRotation = Quaternion.Euler(xRot + angleCompensation, 170, 0);
        }
    }

    void UpdateTimeset() {
        if (currentTime >= currentSettings.startingSunrise && currentTime <= currentSettings.endingSunrise &&
            currTimeset != Timeset.SUNRISE) {
            SetCurrentTimeset(Timeset.SUNRISE);
        } else if (currentTime >= currentSettings.endingSunrise && currentTime <= currentSettings.startingDay &&
                   currTimeset != Timeset.SUNRISEENDING) {
            SetCurrentTimeset(Timeset.SUNRISEENDING);
        } else if (currentTime >= currentSettings.startingDay && currentTime <= currentSettings.startingSunset &&
                   currTimeset != Timeset.DAY) {
            SetCurrentTimeset(Timeset.DAY);
        } else if (currentTime >= currentSettings.startingSunset && currentTime <= currentSettings.startingNight &&
                   currTimeset != Timeset.SUNSET) {
            SetCurrentTimeset(Timeset.SUNSET);
        } else if (currentTime >= currentSettings.startingNight ||
                   currentTime <= currentSettings.startingSunrise && currTimeset != Timeset.NIGHT) {
            SetCurrentTimeset(Timeset.NIGHT);
        }

        if ((24 - currentSettings.startingNight) > 0 && 24 - currentSettings.startingNight < 12) {
            fHoursbeforeMidnight = 24 - currentSettings.startingNight;
        }

        fHoursAfterMidnight = currentSettings.startingSunrise;

        fSunrisePhase = ((24 - currentSettings.startingSunrise) - (24 - currentSettings.startingDay));
        fNoonPhase = ((24 - currentSettings.startingDay) - (24 - currentSettings.startingNight));
    }

    void UpdateSeasons() {
        float DaysInOneYear = SpeedController.Instance.DaysInOneYear;
        float DaysInOneMonth = SpeedController.Instance.DaysInOneMonth;
        float CurrDayOfYear = SpeedController.Instance.GetDayOfYear();

        if (CurrDayOfYear >= 0 && CurrDayOfYear <= DaysInOneMonth && currSeason != Seasons.SPRING) {
            SetCurrentSeason(Seasons.SPRING);

            currentSettings = sripngSettings;
        } else if (CurrDayOfYear >= DaysInOneMonth && CurrDayOfYear <= (DaysInOneMonth * 2) &&
                   currSeason != Seasons.SUMMER) {
            SetCurrentSeason(Seasons.SUMMER);

            currentSettings = summerSettings;
        } else if (CurrDayOfYear >= (DaysInOneMonth * 2) && CurrDayOfYear <= (DaysInOneMonth * 3) &&
                   currSeason != Seasons.AUTUMN) {
            SetCurrentSeason(Seasons.AUTUMN);

            currentSettings = autumnSettings;
        } else if (CurrDayOfYear >= (DaysInOneMonth * 3) && CurrDayOfYear <= (DaysInOneMonth * 4) &&
                   currSeason != Seasons.WINTER) {
            SetCurrentSeason(Seasons.WINTER);

            currentSettings = winterSettings;
        }
    }

    void UpdateLightFadeOut() {
        if (currTimeset == Timeset.SUNRISE) {
            NightLight.shadowStrength = Mathf.Lerp(NightLight.shadowStrength, 0, Time.deltaTime / 10f);
            SunLight.shadowStrength = Mathf.Lerp(SunLight.shadowStrength, sunriseShadowStrength, Time.deltaTime / 30f);
            if (NightLight.shadowStrength <= 0.05 && NightLight.enabled == true) {
                SunLight.enabled = true;
                NightLight.enabled = false;
            }
        }
        if (currTimeset == Timeset.SUNRISEENDING) {
            NightLight.shadowStrength = Mathf.Lerp(NightLight.shadowStrength, 0, Time.deltaTime / 10f);
            SunLight.shadowStrength = Mathf.Lerp(SunLight.shadowStrength, sunriseShadowStrength, Time.deltaTime / 30f);
            if (NightLight.shadowStrength <= 0.05 && NightLight.enabled == true) {
                SunLight.enabled = true;
                NightLight.enabled = false;
            }
        }
        if (currTimeset == Timeset.DAY) {
            NightLight.shadowStrength = Mathf.Lerp(NightLight.shadowStrength, 0, Time.deltaTime / 30f);
            SunLight.shadowStrength = Mathf.Lerp(SunLight.shadowStrength, dayShadowStrength, Time.deltaTime / 30f);
            if (NightLight.shadowStrength <= 0.05 && NightLight.enabled == true) {
                SunLight.enabled = true;
                NightLight.enabled = false;
            }
        }
        if (currTimeset == Timeset.SUNSET) {
            NightLight.shadowStrength = Mathf.Lerp(NightLight.shadowStrength, 0, Time.deltaTime / 30f);
            SunLight.shadowStrength = Mathf.Lerp(SunLight.shadowStrength, sunsetShadowStrength, Time.deltaTime / 30f);
            if (NightLight.shadowStrength <= 0.05 && NightLight.enabled == true) {
                SunLight.enabled = true;
                NightLight.enabled = false;
            }
        }
        if (currTimeset == Timeset.NIGHT) {
            SunLight.shadowStrength = Mathf.Lerp(SunLight.shadowStrength, 0, Time.deltaTime / 5f);
            NightLight.shadowStrength =
                Mathf.Lerp(NightLight.shadowStrength, nightShadowStrength, Time.deltaTime / 15f);

            if (SunLight.shadowStrength <= 0.1 && SunLight.shadowStrength > 0.05)
                NightLight.enabled = true;
            if (SunLight.shadowStrength <= 0.05 && SunLight.intensity <= 0.05 && SunLight.enabled == true) {
                SunLight.enabled = false;
                if (NightLight.enabled == false)
                    NightLight.enabled = true;
            }
        }
    }

    void SetCurrentSeason(Seasons currentSeason) {
        currSeason = currentSeason;
    }

    void SetCurrentTimeset(Timeset currTime) {
        currTimeset = currTime;
    }

    public void UpdateWeather() {
        if (currTimeset == Timeset.SUNRISE) {
            sunriseLightIntensity = currentSettings.sunriseLightIntensity;
            sunriseLightColor = currentSettings.sunriseLightColor;
        } else if (currTimeset == Timeset.SUNRISEENDING) {
            sunriseLightIntensity = currentSettings.endingSunriseLightIntensity;
            sunriseLightColor = currentSettings.dayLightColor;
        }
        DifferentFadeTimes();
    }

    private void DifferentFadeTimes() {
        if (currTimeset == Timeset.SUNRISE || currTimeset == Timeset.SUNRISEENDING) {
            UpdateAll(sunriseLightIntensity, sunriseLightColor, 0.0f, currentSettings.moonNightLightColor,
                currentSettings.sunriseSkyTintColor, fadeTime);
            DeactivateTimesetParticle(nightParticle);
            DeactivateTimesetParticle(dayParticle);
            DeactivateTimesetParticle(sunsetParticle);
            ActivateTimesetParticle(sunriseParticle);
        } else if (currTimeset == Timeset.DAY) {
            UpdateAll(currentSettings.dayLightIntensity, currentSettings.dayLightColor, 0.0f,
                currentSettings.moonNightLightColor, currentSettings.daySkyTintColor, fadeTime);
            DeactivateTimesetParticle(nightParticle);
            DeactivateTimesetParticle(sunriseParticle);
            DeactivateTimesetParticle(dayParticle);
            ActivateTimesetParticle(dayParticle);
        } else if (currTimeset == Timeset.SUNSET) {
            UpdateAll(currentSettings.sunsetLightIntensity, currentSettings.sunsetLightColor, 0.0f,
                currentSettings.moonNightLightColor, currentSettings.sunsetSkyTintColor, fadeTime);
            DeactivateTimesetParticle(nightParticle);
            DeactivateTimesetParticle(sunriseParticle);
            DeactivateTimesetParticle(dayParticle);
            ActivateTimesetParticle(sunsetParticle);
        } else if (currTimeset == Timeset.NIGHT) {
            UpdateAll(currentSettings.nightLightIntensity, currentSettings.nightLightColor,
                currentSettings.nightMoonLightIntensity, currentSettings.moonNightLightColor,
                currentSettings.nightSkyTintColor, 20);

            DeactivateTimesetParticle(sunriseParticle);
            DeactivateTimesetParticle(dayParticle);
            DeactivateTimesetParticle(sunsetParticle);
            ActivateTimesetParticle(nightParticle);
        }
    }

    public void UpdateAll(float sunIntensity, Color sunLightColor, float moonIntensity, Color moonLightColor,
        Color skyTint, float fadeTime) {
        // Sun Light
        SunLight.intensity = Mathf.Lerp(SunLight.intensity, sunIntensity, Time.deltaTime / fadeTime);
        SunLight.color = Color.Lerp(SunLight.color, sunLightColor, Time.deltaTime / fadeTime);

        NightLight.intensity = Mathf.Lerp(NightLight.intensity, moonIntensity, Time.deltaTime / fadeTime);
        NightLight.color = Color.Lerp(NightLight.color, moonLightColor, Time.deltaTime / fadeTime);

        // Skybox settings
        skyboxMaterial.SetColor(TintProperty,
            Color.Lerp(skyboxMaterial.GetColor(TintProperty), skyTint, Time.deltaTime / fadeTime));
    }

    public void ActivateTimesetParticle(GameObject CurrParticles) {
        if (CurrParticles != null) {
            if (CurrParticles.gameObject.GetComponent<ParticleSystem>() != false) {
                CurrParticles.SetActive(true);
                if (!CurrParticles.GetComponent<ParticleSystem>().isPlaying) {
                    CurrParticles.GetComponent<ParticleSystem>().Play(true);
                }
            }
        }
    }

    public void DeactivateTimesetParticle(GameObject CurrParticles) {
        if (CurrParticles != null) {
            if (CurrParticles.gameObject.GetComponent<ParticleSystem>() != false) {
                CurrParticles.SetActive(false);
                if (CurrParticles.GetComponent<ParticleSystem>().isPlaying) {
                    CurrParticles.GetComponent<ParticleSystem>().Stop(true);
                }
            }
        }
    }
}

}