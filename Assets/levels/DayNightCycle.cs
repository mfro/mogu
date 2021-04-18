using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DayNightCycle : MonoBehaviour
{
    public float TransitionDuration = 1;

    public static float TimeOfDay = 11;
    public float SunriseTime = 8;
    public float SunsetTime = 20;
    public float LengthOfCycle = 300;
    public float DayNightTransitionLength = 4;
    public bool IsPassive = false;

    private float lastTimeOfDay = 0;
    private float velocity = 0;
    private float advancing;

    [SerializeField] GameObject Sun;
    [SerializeField] GameObject SunriseSun;
    [SerializeField] GameObject SunsetSun;
    [SerializeField] GameObject Moon;
    [SerializeField] SpriteRenderer Sunrise;
    [SerializeField] SpriteRenderer Day;
    [SerializeField] SpriteRenderer Sunset;
    [SerializeField] SpriteRenderer Night;
    [SerializeField] AnimationCurve DayNightTransitionCurve;

    private static Color Transparent = new Color(0, 0, 0, 0);

    // Start is called before the first frame update
    void Start()
    {
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
        SunriseSun.GetComponentInChildren<SpriteRenderer>().color = Transparent;
        SunsetSun.GetComponentInChildren<SpriteRenderer>().color = Transparent;
    }

    // Update is called once per frame
    void Update()
    {
        if (!Physics.IsEnabled) return;

        if (IsPassive)
        {
            Color Transparent = new Color(0, 0, 0, 0);
            TimeOfDay += (Time.deltaTime / LengthOfCycle) * 24;
            TimeOfDay %= 24;
        }
        else if (advancing > 0)
        {
            if (velocity < 0.5f)
                velocity += 0.5f * Time.deltaTime;

            if (advancing < 0.25f)
                velocity = 2 * advancing;

            if (velocity * Time.deltaTime > advancing)
            {
                velocity = 0;
                advancing = 0;
                TimeOfDay += advancing;
            }
            else
            {
                advancing -= velocity * Time.deltaTime;
                TimeOfDay += velocity * Time.deltaTime;
            }

            TimeOfDay %= 24;
        }

        if (TimeOfDay == lastTimeOfDay) return;

        // If Night and was Day
        if ((TimeOfDay < SunriseTime || TimeOfDay >= SunsetTime) && (lastTimeOfDay >= SunriseTime && lastTimeOfDay < SunsetTime))
        {

            Night.color = Color.white;
            Day.color = Transparent;
        }
        // If Dan and was Night
        else if ((lastTimeOfDay < SunriseTime || lastTimeOfDay >= SunsetTime) && (TimeOfDay >= SunriseTime && TimeOfDay < SunsetTime))
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
            SunriseSun.GetComponentInChildren<SpriteRenderer>().color = SunriseTransparency;
        }
        else
        {
            Sunrise.color = Transparent;
            SunriseSun.GetComponentInChildren<SpriteRenderer>().color = Transparent;
        }

        if (TimeOfDay > SunsetTime - DayNightTransitionLength / 2 && TimeOfDay < SunsetTime + DayNightTransitionLength / 2)
        {
            float CurvePosition = (TimeOfDay - (SunsetTime - DayNightTransitionLength / 2)) / DayNightTransitionLength;
            float CurveAlpha = DayNightTransitionCurve.Evaluate((TimeOfDay - (SunsetTime - DayNightTransitionLength / 2)) / DayNightTransitionLength);
            Color SunsetTransparency = Color.Lerp(Transparent, Color.white, CurveAlpha);
            Sunset.color = SunsetTransparency;
            SunsetSun.GetComponentInChildren<SpriteRenderer>().color = SunsetTransparency;

        }
        else
        {
            Sunset.color = Transparent;
            SunsetSun.GetComponentInChildren<SpriteRenderer>().color = Transparent;
        }

        float t = Mathf.Lerp(-0.15f, 1.15f, (TimeOfDay - (SunriseTime - DayNightTransitionLength / 2)) / (SunsetTime - SunriseTime + DayNightTransitionLength)) * Mathf.PI;
        Sun.transform.localPosition = new Vector3(-5.5f * Mathf.Cos(t), 10 * Mathf.Sin(t) - 6, 0);
        SunsetSun.transform.position = Sun.transform.position;
        SunriseSun.transform.position = Sun.transform.position;

        float LengthOfNight = 24 - (SunsetTime - SunriseTime);
        float TimeSinceSunset = (TimeOfDay - SunsetTime);
        if (TimeSinceSunset < 0)
            TimeSinceSunset += 24;
        t = Mathf.Lerp(0.05f, 0.95f, TimeSinceSunset / LengthOfNight) * Mathf.PI;
        Moon.transform.localPosition = new Vector3(-7 * Mathf.Cos(t) + 0.3f, 8 * Mathf.Sin(t) - 5, 0);

        lastTimeOfDay = TimeOfDay;
    }

    public void Advance(float hours)
    {
        if (IsPassive) return;

        advancing += hours;
        // var t0 = DayNightCycle.TimeOfDay;
        // var t1 = t0 + 3;

        // var anim = Animations.Animate(6, Animations.EaseInOutSine);
        // while (!anim.isComplete)
        // {
        //     if (!Physics.IsEnabled) { await Util.NextFrame(); continue; }
        //     await anim.NextFrame();
        //     if (this == null) return;

        //     DayNightCycle.TimeOfDay = Mathf.Lerp(t0, t1, anim.progress) % 24;
        // }
    }
}
