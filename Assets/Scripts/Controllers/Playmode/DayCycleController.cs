using Assets.Scripts.Util;
using UnityEngine;

namespace Assets.Scripts.Controllers {

public class DayCycleController:SingletonMonoBehaviour<DayCycleController> {
    public Light dirLight;
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

    private float sunriseShadowStrength = 0.3f;
    private float dayShadowStrength = 0.45f;
    private float sunsetShadowStrength = 0.4f;
    private float nightShadowStrength = 0.25f;

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
        dirLight.transform.localRotation = Quaternion.Euler(xRot, yRot, 0);
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
        float currMonth = TimeController.Instance.GetMonth();
		switch (currMonth) {
			case 1:
				SetCurrentSeason(Seasons.SUMMER);
            	currentSettings = summerSettings;
            	break;
            case 2:
            	SetCurrentSeason(Seasons.AUTUMN);
            	currentSettings = autumnSettings;
            	break;
            case 3:
            	SetCurrentSeason(Seasons.WINTER);
            	currentSettings = winterSettings;
            	break;
            default:
                SetCurrentSeason(Seasons.SPRING);
                currentSettings = springSettings;
                break;
        }
    }

    private void SetCurrentSeason(Seasons currentSeason) {
        currSeason = currentSeason;
    }

    private void SetCurrentTimeset(Timeset currTime) {
        currTimeset = currTime;
    }
    
    private void SetLightAndParticles() {

        switch (currTimeset) {
            case Timeset.SUNRISE:
                UpdateAll(currentSettings.sunriseLightIntensity, currentSettings.sunriseLightColor, sunriseShadowStrength, currentSettings.sunriseSkyTintColor);
                break;
            case Timeset.SUNRISEENDING:
                UpdateAll(currentSettings.endingSunriseLightIntensity, currentSettings.dayLightColor, sunriseShadowStrength, currentSettings.daySkyTintColor);
                break;
            case Timeset.DAY:
                UpdateAll(currentSettings.dayLightIntensity, currentSettings.dayLightColor, dayShadowStrength, currentSettings.daySkyTintColor);
                break;
            case Timeset.SUNSET:
                UpdateAll(currentSettings.sunsetLightIntensity, currentSettings.sunsetLightColor, sunsetShadowStrength, currentSettings.sunsetSkyTintColor);
                break;
            case Timeset.NIGHT:
                UpdateAll(currentSettings.nightLightIntensity, currentSettings.moonNightLightColor, nightShadowStrength, currentSettings.nightSkyTintColor);
                break;
            default:
                UpdateAll(currentSettings.dayLightIntensity, currentSettings.dayLightColor, dayShadowStrength, currentSettings.daySkyTintColor);
                break;
        }
        SetTimesetParticleActive(sunriseParticle, currTimeset == Timeset.SUNRISE || currTimeset == Timeset.SUNRISEENDING);
        SetTimesetParticleActive(dayParticle, currTimeset == Timeset.DAY);
        SetTimesetParticleActive(sunsetParticle, currTimeset == Timeset.SUNSET);
        SetTimesetParticleActive(nightParticle, currTimeset == Timeset.NIGHT);
    }

    private void UpdateAll(float sunIntensity, Color sunLightColor, float shadowStrength, Color skyTint) {
        // Light
        dirLight.intensity = Mathf.Lerp(dirLight.intensity, sunIntensity, Time.unscaledDeltaTime / fadeTime);
        dirLight.color = Color.Lerp(dirLight.color, sunLightColor, Time.unscaledDeltaTime / fadeTime);
        dirLight.shadowStrength = Mathf.Lerp(dirLight.shadowStrength, shadowStrength, Time.unscaledDeltaTime / fadeTime);

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