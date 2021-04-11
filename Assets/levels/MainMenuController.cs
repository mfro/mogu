using UnityEngine;
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

    private bool animating;
    private bool inMenu;

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

    public void DoOptions() => EnterMenu(options.gameObject);
    public void DoCredits() => EnterMenu(credits.gameObject);

    public void DoOptionsReturn() => LeaveMenu(options.gameObject);
    public void DoCreditsReturn() => LeaveMenu(credits.gameObject);

    private async void EnterMenu(GameObject target)
    {
        while (animating) await Util.NextFrame();
        if (inMenu) return;
        inMenu = true;
        target.SetActive(true);
        await Animate(carousel, new Vector2(-384, 0), 1, EaseInOutCubic);
        nav.gameObject.SetActive(false);
    }

    private async void LeaveMenu(GameObject target)
    {
        while (animating) await Util.NextFrame();
        if (!inMenu) return;
        inMenu = false;
        nav.gameObject.SetActive(true);
        await Animate(carousel, new Vector2(384, 0), 1, EaseInOutCubic);
        target.SetActive(false);
    }

    private async Task Animate(GameObject target, Vector2 delta, float timeMultiplier, Func<float, float> timing)
    {
        animating = true;

        var p0 = target.transform.localPosition;
        var p1 = p0 + (Vector3)delta;

        var t0 = Time.time;
        var t1 = t0 + (animationTime * timeMultiplier);

        var cancelled = false;

        await Util.EveryFrame(() =>
        {
            if (Time.time >= t1 || cancelled) return false;

            var t = (Time.time - t0) / (animationTime * timeMultiplier);
            target.transform.localPosition = Vector3.Lerp(p0, p1, timing(t));

            return true;
        });

        target.transform.localPosition = p1;

        animating = false;
    }

    private static float EaseInOutCubic(float t) => t < 0.5 ? (Mathf.Pow(2 * t, 3) / 2) : (Mathf.Pow(2 * t - 2, 3) / 2 + 1);
    private static float EaseInOutQuadratic(float t) => t < 0.5 ? (2 * Mathf.Pow(t, 2)) : (1 - Mathf.Pow(-2 * t + 2, 2) / 2);
    private static float EaseInOutSine(float t) => (-Mathf.Cos(Mathf.PI * t) + 1) / 2;
}
