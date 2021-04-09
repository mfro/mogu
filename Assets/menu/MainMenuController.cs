using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

public class MainMenuController : MonoBehaviour
{
    [SerializeField] NavMenu nav;
    [SerializeField] OptionsMenu options;
    [SerializeField] CreditsMenu credits;
    [SerializeField] GameObject carousel;

    [SerializeField] Audio mainMenuMusic;

    [SerializeField] float animationTime = 0.3f;

    void Start()
    {
        AudioManager.instance?.PlayMusic(mainMenuMusic);
        options.Close += DoOptionsReturn;
        credits.Close += DoCreditsReturn;
        nav.Options += DoOptions;
        nav.Credits += DoCredits;
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
        var p0 = carousel.transform.position;
        var p1 = p0 + (Vector3)delta;

        var t0 = Time.time;
        var t1 = t0 + animationTime;

        while (Time.time < t1)
        {
            var t = (Time.time - t0) / animationTime;
            var d = t < 0.5 ? (Mathf.Pow(2 * t, 3) / 2) : (Mathf.Pow(2 * t - 2, 3) / 2 + 1);
            carousel.transform.position = Vector3.Lerp(p0, p1, d);
            await Task.Yield();
        }

        carousel.transform.position = p1;
        Physics.IsEnabled = true;
    }
}
