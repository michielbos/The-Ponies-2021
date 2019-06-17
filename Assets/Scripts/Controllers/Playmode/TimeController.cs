using Assets.Scripts.Util;
using System;
using System.Collections.Generic;
using Controllers.Playmode;
using UnityEngine;

namespace Assets.Scripts.Controllers {

public class TimeController : SingletonMonoBehaviour<TimeController> {
    private const float REAL_TIME_TO_GAME_TIME_MULTIPLIER = 60;
    private Speed currentSpeed;
    private long fullSeconds;
    public double StaringGameTime = 720;
    public int DaysInOneMonth = 25;
    public int DaysInOneYear = 100;
    private double currentGameTime;
    private float pauseTimer;
    private bool forcePaused;
    public bool twelveHourClock = true;
    private readonly List<ITimeTickListener> tickListeners = new List<ITimeTickListener>();

    void Start() {
        currentSpeed = Speed.Normal;
        currentGameTime = StaringGameTime;
    }

    public void Update() {
        if (IsPaused()) {
            HUDController.Instance.UpdatePauseBlink(pauseTimer);

            if (Time.timeScale != 0) {
                Time.timeScale = 0;
            }
            pauseTimer += Time.unscaledDeltaTime;
        } else if (Time.timeScale != currentSpeed.GetMultiplier()) {
            Time.timeScale = currentSpeed.GetMultiplier();
        }

        currentGameTime += Time.deltaTime;
        if (Math.Floor(currentGameTime) > fullSeconds) {
            fullSeconds = (long) Math.Floor(currentGameTime);
            HUDController.Instance.UpdateTime();
            tickListeners.ForEach(listener => listener.OnTick());
        }
    }

    public bool IsPaused() {
        return currentSpeed.GetIndex() == Speed.Paused.GetIndex() || forcePaused;
    }

    public void SetSpeed(int index) {
        SetSpeed(Speed.GetByIndex(index));
    }

    private void SetSpeed(Speed speed) {
        if (forcePaused)
            return;

        int from = IsPaused() ? 0 : currentSpeed.GetIndex();
        int to = speed.GetIndex();

        if (from != to) {
            PlaySpeedSound(from, to);
            currentSpeed = speed;
            pauseTimer = 0;
            HUDController.Instance.UpdateSpeed();
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
        forcePaused = paused;
    }

    public string GetTimeText() {
        string currMinutes;
        string currHours;
        string disambiguation;
        if (GetMinuteOfHour() < 10) {
            currMinutes = "0" + GetMinuteOfHour();
        } else {
            currMinutes = "" + GetMinuteOfHour();
        }
        if (twelveHourClock) {
            if (GetHourOfDay() == 0) {
                currHours = "12";
                disambiguation = " AM";
            } else if (GetHourOfDay() < 12) {
                if (GetHourOfDay() < 10) {
                    currHours = "0" + GetHourOfDay();
                } else {
                    currHours = "" + GetHourOfDay();
                }
                disambiguation = " AM";
            } else if (GetHourOfDay() == 12) {
                currHours = "12";
                disambiguation = " PM";
            } else {
                int currHourHelper = (GetHourOfDay() - 12);
                if (GetHourOfDay() < 20) {
                    currHours = "0" + currHourHelper;
                } else {
                    currHours = "" + currHourHelper;
                }
                disambiguation = " PM";
            }
        } else {
            if (GetHourOfDay() < 10) {
                currHours = "0" + GetHourOfDay();
            } else {
                currHours = "" + GetHourOfDay();
            }
            disambiguation = "";
        }

        return GetDayOfWeek().GetShortName() + " " + currHours + ":" + currMinutes + "" + disambiguation;
    }

    public int GetHourOfDay() {
        return (int) (currentGameTime * REAL_TIME_TO_GAME_TIME_MULTIPLIER / 60 / 60 % 24);
    }

    public int GetMinuteOfHour() {
        return (int) (currentGameTime * REAL_TIME_TO_GAME_TIME_MULTIPLIER / 60 % 60);
    }

    public int GetSecondOfMinute() {
        return (int) (currentGameTime * REAL_TIME_TO_GAME_TIME_MULTIPLIER % 60);
    }

    public int GetDayOfYear() {
        return (int) (currentGameTime * REAL_TIME_TO_GAME_TIME_MULTIPLIER / 60 / 60 / 24 % DaysInOneYear);
    }

    public int GetDay() {
        return (int) (currentGameTime * REAL_TIME_TO_GAME_TIME_MULTIPLIER / 60 / 60 / 24);
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