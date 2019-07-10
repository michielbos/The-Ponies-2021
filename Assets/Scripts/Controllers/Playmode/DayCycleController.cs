using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Controllers {

public class DayCycleController:SingletonMonoBehaviour<DayCycleController> {
    public Light SunLight;
    public Material skyboxMaterialBase;

    private float currentTime;

    private float fHoursBeforeMidnight;
    private float fHoursAfterMidnight;
    private float nightTime;
    private float dayTime;

    private float fSunrisePhase;
    private float fNoonPhase;
    
    private float xRot;
    private float yRot;
    
    private Material skyboxMaterial;

    [System.Serializable]
    public class LightSettings {
        ///***Starting times***
        [Header("Time Cycle")]
        
        [Range(0f, 24f)] public float startingSunrise = 6f;
        [Range(0f, 24f)] public float endingSunrise = 10f;
        [Range(0f, 24f)] public float startingDay = 12f;
        [Range(0f, 24f)] public float startingSunset = 18f;
        [Range(0f, 24f)] public float startingNight = 20f;

        [Header("Light Intensity")]
        
        [Range(0f, 1f)] public float sunriseLightIntensity = 0.6f;
        [Range(0f, 1f)] public float endingSunriseLightIntensity = 0.8f;
        [Range(0f, 1f)] public float dayLightIntensity = 1.0f;
        [Range(0f, 1f)] public float sunsetLightIntensity = 0.7f;
        [Range(0f, 1f)] public float nightLightIntensity = 0.4f;

        ///***COLORS***
        [Header("Light Colors")] //Sun
        public Color sunriseLightColor;
        public Color dayLightColor;
        public Color sunsetLightColor;
        public Color moonNightLightColor;

        [Header("Skybox Colors")] // Skybox
        public Color sunriseSkyTintColor;
        public Color daySkyTintColor;
        public Color sunsetSkyTintColor;
        public Color nightSkyTintColor;
    }

    private LightSettings currentSettings;
    public LightSettings springSettings;
    public LightSettings summerSettings;
    public LightSettings autumnSettings;
    public LightSettings winterSettings;

    private float sunriseShadowStrength = 0.25f;
    private float dayShadowStrength = 0.45f;
    private float sunsetShadowStrength = 0.4f;
    private float nightShadowStrength = 0.2f;

    private float sunriseLightIntensity;
    private Color sunriseLightColor;
    
    public GameObject sunriseParticle;
    public GameObject dayParticle;
    public GameObject sunsetParticle;
    public GameObject nightParticle;

    public float fadeTime = 5.0f;

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

    private void Start() {
        skyboxMaterial = new Material(skyboxMaterialBase);
        RenderSettings.skybox = skyboxMaterial;
        currentSettings = springSettings;
    }

    private void UpdateTime() {
        float hour = TimeController.Instance.GetHourOfDay();
        float minute = TimeController.Instance.GetMinuteOfHour();

        currentTime = hour + minute / 60;
    }

    private void Update() {
        UpdateTime();

        UpdateSeasons();
        UpdateTimeset();
        SetLightAndParticles();
        UpdateSunAndMoon();
    }

    private void UpdateSunAndMoon() {

        if (currentTime < currentSettings.startingSunrise || currentTime > currentSettings.startingNight) {
            if (currentTime < 24 && currentTime > currentSettings.startingNight) {
                nightTime = currentTime - currentSettings.startingNight;
                xRot = Mathf.Lerp(0f, 90f, nightTime / fHoursBeforeMidnight);
                yRot = Mathf.Lerp(180f, 270f, nightTime / fHoursBeforeMidnight);
            } else if (currentTime < currentSettings.startingSunrise) {
                xRot = Mathf.Lerp(90f, 180f, currentTime / fHoursAfterMidnight);
                yRot = Mathf.Lerp(270f, 360f, currentTime / fHoursAfterMidnight);
            }
            
        } else {
            if (currentTime > currentSettings.startingSunrise && currentTime < currentSettings.startingDay) {
                dayTime = currentTime - currentSettings.startingSunrise;
                xRot = Mathf.Lerp(0f, 90f, dayTime / fSunrisePhase);
                yRot = xRot;
            } else if (currentTime > currentSettings.startingDay) {
                dayTime = currentTime - currentSettings.startingDay;
                xRot = Mathf.Lerp(90f, 180f, dayTime / fNoonPhase);
                yRot = xRot;
            }
        }
        SunLight.transform.localRotation = Quaternion.Euler(xRot, yRot, 0);
    }

    private void UpdateTimeset() {
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
            fHoursBeforeMidnight = 24 - currentSettings.startingNight;
        }

        fHoursAfterMidnight = currentSettings.startingSunrise;

        fSunrisePhase = ((24 - currentSettings.startingSunrise) - (24 - currentSettings.startingDay));
        fNoonPhase = ((24 - currentSettings.startingDay) - (24 - currentSettings.startingNight));
    }

    private void UpdateSeasons() {
        float daysInOneMonth = TimeController.Instance.DaysInOneMonth;
        float currDayOfYear = TimeController.Instance.GetDayOfYear();

        if (currDayOfYear >= 0 && currDayOfYear <= daysInOneMonth && currSeason != Seasons.SPRING) {
            SetCurrentSeason(Seasons.SPRING);

            currentSettings = springSettings;
        } else if (currDayOfYear >= daysInOneMonth && currDayOfYear <= (daysInOneMonth * 2) &&
                    currSeason != Seasons.SUMMER) {
            SetCurrentSeason(Seasons.SUMMER);

            currentSettings = summerSettings;
        } else if (currDayOfYear >= (daysInOneMonth * 2) && currDayOfYear <= (daysInOneMonth * 3) &&
                    currSeason != Seasons.AUTUMN) {
            SetCurrentSeason(Seasons.AUTUMN);

            currentSettings = autumnSettings;
        } else if (currDayOfYear >= (daysInOneMonth * 3) && currDayOfYear <= (daysInOneMonth * 4) &&
                    currSeason != Seasons.WINTER) {
            SetCurrentSeason(Seasons.WINTER);

            currentSettings = winterSettings;
        }
    }

    private void SetCurrentSeason(Seasons currentSeason) {
        currSeason = currentSeason;
    }

    private void SetCurrentTimeset(Timeset currTime) {
        currTimeset = currTime;
    }
    
    private void SetLightAndParticles() {
        if (currTimeset == Timeset.SUNRISE || currTimeset == Timeset.SUNRISEENDING) {
            if (currTimeset == Timeset.SUNRISE)
                UpdateAll(currentSettings.sunriseLightIntensity, currentSettings.sunriseLightColor, sunriseShadowStrength, currentSettings.sunriseSkyTintColor);
            else if (currTimeset == Timeset.SUNRISEENDING)
                UpdateAll(currentSettings.endingSunriseLightIntensity, currentSettings.dayLightColor, sunriseShadowStrength, currentSettings.daySkyTintColor);
        } else if (currTimeset == Timeset.DAY) {
            UpdateAll(currentSettings.dayLightIntensity, currentSettings.dayLightColor, dayShadowStrength, currentSettings.daySkyTintColor);

        } else if (currTimeset == Timeset.SUNSET) {
            UpdateAll(currentSettings.sunsetLightIntensity, currentSettings.sunsetLightColor, sunsetShadowStrength, currentSettings.sunsetSkyTintColor);

        } else if (currTimeset == Timeset.NIGHT) {
            UpdateAll(currentSettings.nightLightIntensity, currentSettings.moonNightLightColor, nightShadowStrength, currentSettings.nightSkyTintColor);
        }

        SetTimesetParticleActive(sunriseParticle, currTimeset == Timeset.SUNRISE || currTimeset == Timeset.SUNRISEENDING);
        SetTimesetParticleActive(dayParticle, currTimeset == Timeset.DAY);
        SetTimesetParticleActive(sunsetParticle, currTimeset == Timeset.SUNSET);
        SetTimesetParticleActive(nightParticle, currTimeset == Timeset.NIGHT);
    }

    private void UpdateAll(float sunIntensity, Color sunLightColor, float shadowStrength, Color skyTint) {
        // Light
        SunLight.intensity = Mathf.Lerp(SunLight.intensity, sunIntensity, Time.unscaledDeltaTime / fadeTime);
        SunLight.color = Color.Lerp(SunLight.color, sunLightColor, Time.unscaledDeltaTime / fadeTime);
        SunLight.shadowStrength = Mathf.Lerp(SunLight.shadowStrength, shadowStrength, Time.unscaledDeltaTime / fadeTime);

        // Skybox settings
        skyboxMaterial.SetColor(TintProperty,
            Color.Lerp(skyboxMaterial.GetColor(TintProperty), skyTint, Time.unscaledDeltaTime / fadeTime));
    }

    private void SetTimesetParticleActive(GameObject currParticles, bool active) {
        if (currParticles == null)
            return;

        ParticleSystem particleSystem = currParticles.gameObject.GetComponent<ParticleSystem>();
        if (particleSystem != null) {
            currParticles.SetActive(active);
            if (active) { // Alternatively, you could combine the active and isPlaying into one if-else-if set.
                if (!particleSystem.isPlaying) {
                    particleSystem.Play(true);
                }
            } else if (particleSystem.isPlaying) {
                particleSystem.Stop(true);
            }
        }
    }

}

}