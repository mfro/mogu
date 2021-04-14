using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class VictoryScreen : MonoBehaviour
{
    [SerializeField] GameObject content;
    [SerializeField] new GameObject camera;
    [SerializeField] float animationTime = 2.4f;

    async void Start()
    {
        await Util.Seconds(8, false);

        var tasks = new[] {
            Animate(camera, new Vector2(0, 12), Animations.EaseInOutSine),
            Animate(content, new Vector2(0, -384), Animations.EaseInOutSine),
        };

        await Task.WhenAll(tasks);
    }

    public void DoPlayAgain()
    {
        SceneController.instance.SwitchScene(1);
    }

    private async Task Animate(GameObject target, Vector2 delta, TimingFunction timing)
    {
        var p0 = target.transform.localPosition;
        var p1 = p0 + (Vector3)delta;

        var anim = Animations.Animate(animationTime, timing);
        while (!anim.isComplete)
        {
            await anim.NextFrame();
            target.transform.localPosition = Vector3.Lerp(p0, p1, anim.progress);
        }
    }
}
