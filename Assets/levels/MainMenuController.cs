using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using System;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] NavMenu nav;
    [SerializeField] OptionsMenu options;
    [SerializeField] CreditsMenu credits;
    [SerializeField] GameObject carousel;

    [SerializeField] new GameObject camera;
    [SerializeField] GameObject levelController;
    [SerializeField] GameObject userInterface;
    [SerializeField] GameObject skyBackground;

    [SerializeField] Audio mainMenuMusic;

    [SerializeField] float animationTime = 0.3f;

    void Start()
    {
        AudioManager.instance?.PlayMusic(mainMenuMusic);
        nav.Play += DoPlay;
        nav.Options += DoOptions;
        nav.Credits += DoCredits;
        options.Close += DoOptionsReturn;
        credits.Close += DoCreditsReturn;

        Physics.IsEnabled = false;
    }

    public async void DoPlay()
    {
        camera.transform.position -= new Vector3(0, 12, 0);
        skyBackground.transform.position -= new Vector3(0, -12, 0);
        userInterface.transform.localPosition -= new Vector3(0, -384, 0);

        var tasks = new[] {
            Animate(camera, new Vector2(0, 12), 4, EaseInOutQuadratic),
            Animate(skyBackground, new Vector2(0, -12), 4, EaseInOutQuadratic),
            Animate(gameObject, new Vector2(0, -384), 4, EaseInOutQuadratic),
            Animate(userInterface, new Vector2(0, -384), 4, EaseInOutQuadratic),
        };

        await Task.WhenAll(tasks);

        Physics.IsEnabled = true;
        nav.gameObject.SetActive(false);
        levelController.GetComponent<LevelController>().GoToLevel(0, false);
    }

    public async void DoOptions()
    {
        options.gameObject.SetActive(true);

        await AnimateCarousel(new Vector2(-384, 0));

        nav.gameObject.SetActive(false);
    }

    public async void DoOptionsReturn()
    {
        nav.gameObject.SetActive(true);

        await AnimateCarousel(new Vector2(384, 0));

        options.gameObject.SetActive(false);
    }

    public async void DoCredits()
    {
        credits.gameObject.SetActive(true);

        await AnimateCarousel(new Vector2(-384, 0));

        nav.gameObject.SetActive(false);
    }

    public async void DoCreditsReturn()
    {
        nav.gameObject.SetActive(true);

        await AnimateCarousel(new Vector2(384, 0));

        credits.gameObject.SetActive(false);
    }

    private async Task AnimateCarousel(Vector2 delta)
    {
        await Animate(carousel, delta, 1, EaseInOutCubic);
    }

    private async Task Animate(GameObject target, Vector2 delta, float timeMultiplier, Func<float, float> timing)
    {
        var p0 = target.transform.localPosition;
        var p1 = p0 + (Vector3)delta;

        var t0 = Time.time;
        var t1 = t0 + (animationTime * timeMultiplier);

        await Util.EveryFrame(() =>
        {
            if (Time.time >= t1) return false;

            var t = (Time.time - t0) / (animationTime * timeMultiplier);
            target.transform.localPosition = Vector3.Lerp(p0, p1, timing(t));

            return true;
        });

        target.transform.localPosition = p1;
    }

    private static float EaseInOutCubic(float t) => t < 0.5 ? (Mathf.Pow(2 * t, 3) / 2) : (Mathf.Pow(2 * t - 2, 3) / 2 + 1);
    private static float EaseInOutQuadratic(float t) => t < 0.5 ? (2 * Mathf.Pow(t, 2)) : (1 - Mathf.Pow(-2 * t + 2, 2) / 2);
    private static float EaseInOutSine(float t) => (-Mathf.Cos(Mathf.PI * t) + 1) / 2;
}
