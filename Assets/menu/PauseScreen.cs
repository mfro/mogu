using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Threading.Tasks;

public class PauseScreen : MonoBehaviour
{
    [SerializeField] GameObject nav;
    [SerializeField] OptionsMenu options;
    [SerializeField] GameObject carousel;
    [SerializeField] float animationTime = 0.3f;

    [SerializeField] Audio pressButtonSound;

    [SerializeField] public Text LevelNumber;
    [SerializeField] public Text LevelTitle;

    [SerializeField] GameObject[] buttons;

    private bool isPaused = false;
    private bool animating = false;

    void Start()
    {
        options.Close += DoOptionsReturn;
    }

    public void TogglePause()
    {
        if (isPaused == Physics.IsEnabled) return;

        isPaused = !isPaused;

        Physics.IsEnabled = !isPaused;
        nav.SetActive(isPaused);
        gameObject.SetActive(isPaused);
        options.gameObject.SetActive(false);

        if (isPaused)
        {
            AudioManager.instance.PlaySFX(pressButtonSound);
            EventSystem.current.SetSelectedGameObject(buttons[0]);
        }
    }

    public void DoQuit()
    {
        SceneController.instance.SwitchScene(0);
    }

    public void DoOptions() => EnterMenu(options.gameObject);
    public void DoOptionsReturn() => LeaveMenu(options.gameObject);

    private async void EnterMenu(GameObject target)
    {
        while (animating) await Util.NextFrame();
        target.SetActive(true);
        await Animate(carousel, new Vector2(-384, 0), 1, EaseInOutCubic);
        nav.gameObject.SetActive(false);
    }

    private async void LeaveMenu(GameObject target)
    {
        while (animating) await Util.NextFrame();
        nav.gameObject.SetActive(true);
        await Animate(carousel, new Vector2(384, 0), 1, EaseInOutCubic);
        target.SetActive(false);

        EventSystem.current.SetSelectedGameObject(buttons[0]);
    }

    private bool _onPause = false;
    public void OnPause(InputAction.CallbackContext c)
    {
        if (c.ReadValueAsButton() && !_onPause && FlipPanel.isFlipping == null)
            TogglePause();

        _onPause = c.ReadValueAsButton();
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
}
