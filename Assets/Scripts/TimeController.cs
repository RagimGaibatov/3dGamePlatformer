using System;
using TMPro;
using UnityEngine;

public class TimeController : MonoBehaviour{
    [SerializeField] private float timeMultiplier;

    [SerializeField] private float startHour;

    [SerializeField] private TextMeshProUGUI timeText;


    [SerializeField] private float sunriseHours;

    [SerializeField] private float sunsetHours;

    [SerializeField] private Color dayAmbientLight;
    [SerializeField] private Color nightAmbientLight;

    [SerializeField] private AnimationCurve lightChangeCurve;

    [SerializeField] private Light sunLight;
    [SerializeField] private Light moonLight;

    [SerializeField] private float maxSunLightIntensity;
    [SerializeField] private float maxMoonLightInstensity;


    private TimeSpan sunriseTime;
    private TimeSpan sunsetTime;

    private DateTime currentTime;

    private void Start(){
        currentTime = DateTime.Now.Date + TimeSpan.FromHours(startHour);

        sunriseTime = TimeSpan.FromHours(sunriseHours);
        sunsetTime = TimeSpan.FromHours(sunsetHours);
    }

    private void Update(){
        UpdateTimeOrDay();
        RotateSun();
        UpdateLightSettings();
    }

    void RotateSun(){
        float sunLightRotation;

        if (currentTime.TimeOfDay > sunriseTime && currentTime.TimeOfDay < sunsetTime){
            TimeSpan sunriseToSunsetDuration = CalculateTimeDifference(sunriseTime, sunsetTime);
            TimeSpan timeSinceSunrise = CalculateTimeDifference(sunriseTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunrise.TotalMinutes / sunriseToSunsetDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(0, 180, (float) percentage);
        }
        else{
            TimeSpan sunsetToSunriseDuration = CalculateTimeDifference(sunsetTime, sunriseTime);
            TimeSpan timeSinceSunset = CalculateTimeDifference(sunsetTime, currentTime.TimeOfDay);

            double percentage = timeSinceSunset.TotalMinutes / sunsetToSunriseDuration.TotalMinutes;

            sunLightRotation = Mathf.Lerp(180, 360, (float) percentage);
        }

        sunLight.transform.rotation = Quaternion.AngleAxis(sunLightRotation, Vector3.right);
    }

    private void UpdateTimeOrDay(){
        currentTime = currentTime.AddSeconds(Time.deltaTime * timeMultiplier);

        if (timeText != null){
            timeText.text = currentTime.ToString("HH:mm");
        }
    }

    private void UpdateLightSettings(){
        float dotProduct = Vector3.Dot(sunLight.transform.forward, Vector3.down);
        sunLight.intensity = Mathf.Lerp(0, maxSunLightIntensity, lightChangeCurve.Evaluate(dotProduct));
        moonLight.intensity = Mathf.Lerp(maxMoonLightInstensity, 0, lightChangeCurve.Evaluate(dotProduct));
        RenderSettings.ambientLight =
            Color.Lerp(nightAmbientLight, dayAmbientLight, lightChangeCurve.Evaluate(dotProduct));
    }

    private TimeSpan CalculateTimeDifference(TimeSpan fromTime, TimeSpan toTime){
        TimeSpan difference = toTime - fromTime;

        if (difference.TotalSeconds < 0){
            difference += TimeSpan.FromHours(24);
        }

        return difference;
    }
}