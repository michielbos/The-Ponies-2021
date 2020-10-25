using Assets.Scripts.Util;
using System.Collections.Generic;
using Controllers.Global;
using Controllers.Playmode;
using Controllers.Singletons;
using UnityEngine;
using System;
using System.Globalization;
using Model.Property;

namespace Assets.Scripts.Controllers {

public class TimeController : SingletonMonoBehaviour<TimeController> {
    // The time at which households start.
    public const long StartingTime = 480; // 8 AM

    // The time which is used when there is no household.
    private const long DefaultTime = 600; // 10 AM

    private Speed currentSpeed;
    public int DaysInOneMonth = 25;
    public int DaysInOneYear = 100;
    private double timeToNextMinute;
    private bool forcePaused;
    private readonly List<ITimeTickListener> tickListeners = new List<ITimeTickListener>();

    private bool TwelveHourClock => SettingsController.Instance.twelveHourClock.Value;

    /// <summary>
    /// The current game time, in ingame minutes.
    /// If time does not matter for this property (because there is no household), a default time is returned.
    /// </summary>
    private long GameTime {
        get {
            long time = PropertyController.Instance.property.GameTime;
            return time > 0 ? time : DefaultTime;
        }
        set { PropertyController.Instance.property.GameTime = value; }
    }

    void Start() {
        currentSpeed = Speed.Normal;
        HUDController.Instance.UpdateTime();
        UpdateSpeed();
    }

    public void Update() {
        timeToNextMinute += Time.deltaTime;
        // In cases of bad lag, it would be possible to have deltaTimes larger than a second
        // This could happen as soon as the framerate drops below 10 FPS at 10x speed.
        while (timeToNextMinute > 1.0) {
            timeToNextMinute--;
            GameTime++;
            HUDController.Instance.UpdateTime();
            tickListeners.ForEach(listener => listener.OnTick());
            HouseholdController.Instance.AfterTick();
        }
    }

    public bool IsPaused() {
        return currentSpeed.GetIndex() == Speed.Paused.GetIndex() || forcePaused;
    }

    public void SetSpeed(int index) {
        SetSpeed(Speed.GetByIndex(index));
    }

    private void SetSpeed(Speed speed) {
        int from = currentSpeed.GetIndex();
        int to = speed.GetIndex();

        if (from != to) {
            PlaySpeedSound(from, to);
            currentSpeed = speed;
            HUDController.Instance.UpdateSpeed();

            if (!forcePaused) {
                UpdateSpeed();
            }
        }
    }
    
    /// <summary>
    /// Called every time the game speed or pause state changed.
    /// </summary>
    private void UpdateSpeed() {
        Time.timeScale = forcePaused ? 0 : currentSpeed.GetMultiplier();
        foreach (PropertyObject propertyObject in PropertyController.Property.propertyObjects.Values) {
            propertyObject.OnGameSpeedChanged();
        }
    }

    public Speed GetCurrentSpeed() {
        return currentSpeed;
    }

    private void PlaySpeedSound(int from, int to) {
        string ff = from.ToString();
        string tf = to.ToString();
        if (ff == "0")
            ff = "P";
        if (tf == "0")
            tf = "P";
        string clipName = "UI_SPEED_" + ff + "TO" + tf;
        SoundController.Instance.PlaySound(clipName);
    }

    public void ForcePause(bool paused) {
        if (paused != forcePaused) {
            forcePaused = paused;
            UpdateSpeed();
        }
    }

    public string GetTimeText() {
        var currTime = new DateTime(2023, 1, 13, GetHourOfDay(), GetMinuteOfHour(), 19);

        if (TwelveHourClock) {
            return GetDayOfWeek().GetShortName() + " " + currTime.ToString("hh:mm tt", new CultureInfo("en-US"));
        } else {
            return GetDayOfWeek().GetShortName() + " " + currTime.ToString("HH:mm");
        }
    }

    public int GetHourOfDay() {
        return (int) (GameTime / 60 % 24);
    }

    public int GetMinuteOfHour() {
        return (int) (GameTime % 60);
    }

    public int GetMonth() {
        return (int) (GameTime / 60 / 24 / DaysInOneMonth % (DaysInOneYear / DaysInOneMonth));
    }

    public int GetDayOfYear() {
        return (int) (GameTime / 60 / 24 % DaysInOneYear);
    }

    public int GetDay() {
        return (int) (GameTime / 60 / 24);
    }

    private Day GetDayOfWeek() {
        return Day.GetByIndex(GetDay() % 7);
    }

    public void AddTickListener(ITimeTickListener listener) {
        tickListeners.Add(listener);
    }

    public void RemoveTickListener(ITimeTickListener listener) {
        tickListeners.Remove(listener);
    }

    public struct Speed {
        public static readonly Speed Paused = new Speed(0, "Paused", 0);
        public static readonly Speed Normal = new Speed(1, "Normal", 1);
        public static readonly Speed Fast = new Speed(2, "Fast", 3);

        public static readonly Speed UltraFast = new Speed(3, "Ultra fast", 10);

        // I really wish C# supported proper enums.
        private static readonly Speed[] speeds = {Paused, Normal, Fast, UltraFast};

        private readonly int index;
        private readonly string name;
        private readonly float multiplier;

        public static Speed GetByIndex(int index) {
            return speeds[index];
        }

        private Speed(int index, string name, float multiplier) {
            this.index = index;
            this.name = name;
            this.multiplier = multiplier;
        }

        public int GetIndex() {
            return index;
        }

        public string GetName() {
            return name;
        }

        public float GetMultiplier() {
            return multiplier;
        }
    }

    public struct Day {
        public static readonly Day Monday = new Day("Monday");
        public static readonly Day Tuesday = new Day("Tuesday");
        public static readonly Day Wednesday = new Day("Wednesday");
        public static readonly Day Thursday = new Day("Thursday");
        public static readonly Day Friday = new Day("Friday");
        public static readonly Day Saturday = new Day("Saturday");
        public static readonly Day Sunday = new Day("Sunday");
        private static readonly Day[] days = {Monday, Tuesday, Wednesday, Thursday, Friday, Saturday, Sunday};

        private readonly string name;

        public static Day GetByIndex(int index) {
            return days[index];
        }

        public Day(string name) {
            this.name = name;
        }

        public string GetName() {
            return name;
        }

        public string GetShortName() {
            return name.Substring(0, 3);
        }
    }
}

}