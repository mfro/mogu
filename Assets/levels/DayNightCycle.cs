using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float SunriseTime = 8;
    public float SunsetTime = 20;
    public static float TimeOfDay = 16;
    public float LengthOfCycle = 300;
    public float DayNightTransitionLength = 4;

    [SerializeField]
    float TimeOfDayView = 0;

    [SerializeField]
    GameObject Sun;
    [SerializeField]
    GameObject Moon;
    [SerializeField]
    SpriteRenderer Sunrise;
    [SerializeField]
    SpriteRenderer Day;
    [SerializeField]
    SpriteRenderer Sunset;
    [SerializeField]
    SpriteRenderer Night;
    [SerializeField]
    AnimationCurve DayNightTransitionCurve;

    // Start is called before the first frame update
    void Start()
    {
        Color Transparent = new Color(0, 0, 0, 0);
        if ((TimeOfDay < SunriseTime || TimeOfDay >= SunsetTime))
        {

            Night.color = Color.white;
            Day.color = Transparent;
        }
        // If Dan and was Night
        else if ((TimeOfDay >= SunriseTime && TimeOfDay < SunsetTime))
        {
            Night.color = Transparent;
            Day.color = Color.white;
        }
        Sunrise.color = Transparent;
        Sunset.color = Transparent;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Physics.IsEnabled) return;

        Color Transparent = new Color(0, 0, 0, 0);
        float PreviousTimeOfDay = TimeOfDay;
        TimeOfDay += (Time.deltaTime / LengthOfCycle) * 24;
        TimeOfDay %= 24;
        // If Night and was Day
        if ((TimeOfDay < SunriseTime || TimeOfDay >= SunsetTime) && (PreviousTimeOfDay >= SunriseTime && PreviousTimeOfDay < SunsetTime))
        {

            Night.color = Color.white;
            Day.color = Transparent;
        }
        // If Dan and was Night
        else if ((PreviousTimeOfDay < SunriseTime || PreviousTimeOfDay >= SunsetTime) && (TimeOfDay >= SunriseTime && TimeOfDay < SunsetTime))
        {
            Night.color = Transparent;
            Day.color = Color.white;
        }

        if (TimeOfDay > SunriseTime - DayNightTransitionLength / 2 && TimeOfDay < SunriseTime + DayNightTransitionLength / 2)
        {
            float CurvePosition = (TimeOfDay - (SunriseTime - DayNightTransitionLength / 2)) / DayNightTransitionLength;
            float CurveAlpha = DayNightTransitionCurve.Evaluate((TimeOfDay - (SunriseTime - DayNightTransitionLength / 2)) / DayNightTransitionLength);
            Color SunriseTransparency = Color.Lerp(Transparent, Color.white, CurveAlpha);
            Sunrise.color = SunriseTransparency;
        }
        else
        {
            Sunrise.color = Transparent;
        }

        if (TimeOfDay > SunsetTime - DayNightTransitionLength / 2 && TimeOfDay < SunsetTime + DayNightTransitionLength / 2)
        {
            float CurvePosition = (TimeOfDay - (SunsetTime - DayNightTransitionLength / 2)) / DayNightTransitionLength;
            float CurveAlpha = DayNightTransitionCurve.Evaluate((TimeOfDay - (SunsetTime - DayNightTransitionLength / 2)) / DayNightTransitionLength);
            Color SunsetTransparency = Color.Lerp(Transparent, Color.white, CurveAlpha);
            Sunset.color = SunsetTransparency;
        }
        else
        {
            Sunset.color = Transparent;
        }

        float t = Mathf.Lerp(-0.05f, 1.05f, (TimeOfDay - (SunriseTime - DayNightTransitionLength / 2)) / (SunsetTime - SunriseTime + DayNightTransitionLength)) * Mathf.PI;
        Sun.transform.position = new Vector3(-5.5f * Mathf.Cos(t), 10 * Mathf.Sin(t) - 6, 0);
        Sun.transform.position += Sun.transform.parent.position;
        float LengthOfNight = 24 - (SunsetTime - SunriseTime) - DayNightTransitionLength;
        float TimeSinceSunset = (TimeOfDay - (SunsetTime + DayNightTransitionLength / 2));
        if (TimeSinceSunset < 0)
            TimeSinceSunset += 24;
        t = Mathf.Lerp(0.05f, 0.95f, TimeSinceSunset / LengthOfNight) * Mathf.PI;
        Moon.transform.position = new Vector3(-7 * Mathf.Cos(t) + 0.3f, 8 * Mathf.Sin(t) - 5, 0);
        Moon.transform.position += Moon.transform.parent.position;

        TimeOfDayView = TimeOfDay;
    }
}
