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
    [SerializeField] GameObject background;

    [SerializeField] Audio mainMenuMusic;
    [SerializeField] Audio gameMusic;

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

        PlayerController.Frozen = true;
        Physics.IsEnabled = false;

        camera.transform.position = new Vector3(0, -15.59375f, 0);
        DayNightCycle.TimeOfDay = 11;
    }

    public async void DoPlay()
    {
        Physics.IsEnabled = true;

        AudioManager.instance?.PlayMusic(gameMusic);

        var ratio = (16.1875f - 19 / 32f) / 12f;
        userInterface.transform.localPosition -= new Vector3(0, -384 * ratio, 0);

        var tasks = new[] {
            Animate(camera, new Vector2(0, 12 * ratio), 4, Animations.EaseInOutQuadratic),
            Animate(gameObject, new Vector2(0, -384 * ratio), 4, Animations.EaseInOutQuadratic),
            Animate(userInterface, new Vector2(0, -384 * ratio), 4, Animations.EaseInOutQuadratic),
        };

        await Task.WhenAll(tasks);

        PlayerController.Frozen = false;
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

        var tasks = new[] {
            Animate(carousel, new Vector2(-384, 0), 1, Animations.EaseInOutCubic),
            Animate(background, new Vector2(-12, 0), 1, Animations.EaseInOutCubic),
        };

        await Util.Seconds(animationTime / 2, false);
        BlurController.instance?.SetBlursEnabled(true);
        await Task.WhenAll(tasks);

        nav.gameObject.SetActive(false);
    }

    private async void LeaveMenu(GameObject target)
    {
        while (animating) await Util.NextFrame();
        if (!inMenu) return;
        inMenu = false;
        nav.gameObject.SetActive(true);

        var tasks = new[] {
            Animate(carousel, new Vector2(384, 0), 1, Animations.EaseInOutCubic),
            Animate(background, new Vector2(12, 0), 1, Animations.EaseInOutCubic),
        };

        await Util.Seconds(animationTime / 2, false);
        BlurController.instance?.SetBlursEnabled(false);
        await Task.WhenAll(tasks);

        target.SetActive(false);
    }

    private async Task Animate(GameObject target, Vector2 delta, float timeMultiplier, TimingFunction timing)
    {
        animating = true;

        var p0 = target.transform.localPosition;
        var p1 = p0 + (Vector3)delta;

        var anim = Animations.Animate(animationTime * timeMultiplier, timing);
        while (!anim.isComplete)
        {
            await anim.NextFrame();
            target.transform.localPosition = Vector3.Lerp(p0, p1, anim.progress);
        }

        animating = false;
    }
}
