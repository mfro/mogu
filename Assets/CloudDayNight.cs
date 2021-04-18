using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudDayNight : MonoBehaviour
{
    [SerializeField]
    Sprite BigDark;
    [SerializeField]
    Sprite MediumDark;
    [SerializeField]
    Sprite SmallDark;

    DayNightCycle dnc;
    SpriteRenderer DayCloud;
    SpriteRenderer NightCloud;



    // Start is called before the first frame update
    void Start()
    {
        dnc = FindObjectOfType<DayNightCycle>();
        DayCloud = GetComponent<SpriteRenderer>();
        NightCloud = transform.GetChild(0).GetComponent<SpriteRenderer>();
        if (DayCloud.sprite.name == "big")
        {
            NightCloud.sprite = BigDark;
        }
        else if (DayCloud.sprite.name == "medium")
        {
            NightCloud.sprite = MediumDark;
        }
        else
        {
            NightCloud.sprite = SmallDark;
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (dnc)
        {
            Color opaque = new Color(1, 1, 1, 0.7f);
            Color transparent = new Color(0, 0, 0, 0);
            if (DayNightCycle.TimeOfDay > dnc.SunriseTime && DayNightCycle.TimeOfDay <= dnc.SunsetTime)
            {
                DayCloud.color = opaque;
                NightCloud.color = transparent;
            }
            else
            {
                DayCloud.color = transparent;
                NightCloud.color = opaque;
            }
            if (DayNightCycle.TimeOfDay > dnc.SunriseTime - dnc.DayNightTransitionLength / 2 && DayNightCycle.TimeOfDay < dnc.SunriseTime + dnc.DayNightTransitionLength / 2)
            {
                DayCloud.color = Color.Lerp(transparent, opaque, (DayNightCycle.TimeOfDay - (dnc.SunriseTime - dnc.DayNightTransitionLength / 2)) / dnc.DayNightTransitionLength);
                NightCloud.color = Color.Lerp(opaque, transparent, (DayNightCycle.TimeOfDay - (dnc.SunriseTime - dnc.DayNightTransitionLength / 2)) / dnc.DayNightTransitionLength);
            }
            else if (DayNightCycle.TimeOfDay > dnc.SunsetTime - dnc.DayNightTransitionLength / 2 && DayNightCycle.TimeOfDay < dnc.SunsetTime + dnc.DayNightTransitionLength / 2)
            {
                DayCloud.color = Color.Lerp(opaque, transparent, (DayNightCycle.TimeOfDay - (dnc.SunsetTime - dnc.DayNightTransitionLength / 2)) / dnc.DayNightTransitionLength);
                NightCloud.color = Color.Lerp(transparent, opaque, (DayNightCycle.TimeOfDay - (dnc.SunsetTime - dnc.DayNightTransitionLength / 2)) / dnc.DayNightTransitionLength);
            }
        }
    }
}
