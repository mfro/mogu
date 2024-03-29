﻿using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.EventSystems;
using System.Threading.Tasks;
using UnityEngine.UI;

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
    private Vector3 carouselOrigin;

    void Awake()
    {
        options.Close += DoOptionsReturn;
        carouselOrigin = carousel.transform.localPosition;
    }

    public void TogglePause()
    {
        if (isPaused == Physics.IsEnabled) return;

        isPaused = !isPaused;

        Physics.IsEnabled = !isPaused;
        nav.SetActive(isPaused);
        gameObject.SetActive(isPaused);
        options.gameObject.SetActive(false);
        carousel.transform.localPosition = carouselOrigin;

        BlurController.instance?.SetBlursEnabled(isPaused);

        if (isPaused)
        {
            AudioManager.instance.PlaySFX(pressButtonSound);
            EventSystem.current.SetSelectedGameObject(buttons[0]);
        }
    }

    public void DoQuit()
    {
        SceneController.instance.SwitchScene(2);
    }

    public void DoOptions() => EnterMenu(options.gameObject);
    public void DoOptionsReturn() => LeaveMenu(options.gameObject);

    private async void EnterMenu(GameObject target)
    {
        while (animating) await Util.NextFrame();
        animating = true;

        target.SetActive(true);

        var tasks = new[] {
            Animate(carousel, new Vector2(-384, 0), 1, Animations.EaseInOutCubic),
        };

        await Task.WhenAll(tasks);

        nav.gameObject.SetActive(false);

        animating = false;
    }

    private async void LeaveMenu(GameObject target)
    {
        while (animating) await Util.NextFrame();
        animating = true;

        nav.gameObject.SetActive(true);

        var tasks = new[] {
            Animate(carousel, new Vector2(384, 0), 1, Animations.EaseInOutCubic),
        };

        await Task.WhenAll(tasks);

        target.SetActive(false);

        EventSystem.current.SetSelectedGameObject(buttons[0]);

        animating = false;
    }

    private bool _onPause = false;
    public void OnPause(InputAction.CallbackContext c)
    {
        if (c.ReadValueAsButton() && !_onPause)
            TogglePause();

        _onPause = c.ReadValueAsButton();
    }

    private async Task Animate(GameObject target, Vector2 delta, float timeMultiplier, TimingFunction timing)
    {
        var p0 = target.transform.localPosition;
        var p1 = p0 + (Vector3)delta;

        var anim = Animations.Animate(animationTime * timeMultiplier, timing);
        while (!anim.isComplete)
        {
            await anim.NextFrame();
            target.transform.localPosition = Vector3.Lerp(p0, p1, anim.progress);
        }
    }
}
