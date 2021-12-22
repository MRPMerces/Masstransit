using System;
using UnityEngine;

public class SpeedController : MonoBehaviour {

    public static SpeedController speedController { get; protected set; }

    DateTime date;

    float secondsPerDay = 1f;

    public int currentDay { get; protected set; }

    float dayTime = 0.0f;
    float hourTime = 0.0f;

    void OnEnable() {
        speedController = this;
        currentDay = 0;
    }

    // Start is called before the first frame update
    void Start() {
        date = new DateTime(1850, 01, 01);
    }

    void Update() {
        if (secondsPerDay != 0) {
            dayTime += Time.deltaTime;
            hourTime += Time.deltaTime;

            if (hourTime > secondsPerDay / 24) {
                hourTime -= secondsPerDay / 24;

                cbHourTicked?.Invoke();
            }

            if (dayTime >= secondsPerDay) {
                dayTime -= secondsPerDay;
                date = date.AddDays(1);
                currentDay++;
                cbDayTicked?.Invoke();
            }
        }
    }

    public void changeSpeed(int speed) {
        secondsPerDay = speed;
    }

    public string currentDate() {
        return date.ToString("yyyy, MMM, dd");
    }

    Action cbHourTicked;
    Action cbDayTicked;

    /// <summary>
    /// Register a function to be called back when our tile type changes.
    /// </summary>
    public void RegisterHourTickCallback(Action callback) {
        cbHourTicked += callback;
    }

    public void UnregisterHourTickCallback(Action callback) {
        cbHourTicked -= callback;
    }

    /// <summary>
    /// Register a function to be called back when our tile type changes.
    /// </summary>
    public void RegisterDayTickCallback(Action callback) {
        cbDayTicked += callback;
    }

    public void UnregisterDayTickCallback(Action callback) {
        cbDayTicked -= callback;
    }
}
