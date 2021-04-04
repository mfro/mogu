using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HintController : MonoBehaviour
{
    public Hint hint1;
    public Hint hint2;

    public MyDynamic player;

    private FlipPanel previousPanel;

    void Update()
    {
        if (!player.enabled) return;

        var bounds = player.bounds;
        var panel = Physics.AllOverlaps(bounds, CollisionMask.Flipping)
            .Where(o => o.Item2 == bounds)
            .Select(o => o.Item1.GetComponent<FlipPanel>())
            .Where(o => o != null)
            .FirstOrDefault();

        if (panel == previousPanel) return;
        previousPanel = panel;

        if (panel == null)
        {
            hint1.gameObject.SetActive(false);
            hint2.gameObject.SetActive(false);
            previousPanel = null;
        }
        else
        {
            var x = panel.transform.localScale.x / 2;
            var y = panel.transform.localScale.y / 2;

            if (panel.flip1 == FlipKind.CW && panel.flip2 == FlipKind.CCW)
            {
                hint1.transform.position = panel.transform.position + new Vector3(x - 1 / 64f, y - 1 / 64f, -10);
                hint1.input = HintInput.Flip2;
                hint1.context = HintContext.CW;
                hint1.gameObject.SetActive(true);
                hint1.ReRender();

                hint2.transform.position = panel.transform.position + new Vector3(-x + 1 / 64f, y - 1 / 64f, -10);
                hint2.input = HintInput.Flip1;
                hint2.context = HintContext.CCW;
                hint2.gameObject.SetActive(true);
                hint2.ReRender();
            }
            else if (panel.flip1 == FlipKind.Vertical)
            {
                hint1.transform.position = panel.transform.position + new Vector3(1 / 64f, y - 1 / 64f, -10);
                hint1.input = HintInput.Flip1;
                hint1.context = player.down.x == 0 ? HintContext.Vertical : HintContext.Horizontal;
                hint1.gameObject.SetActive(true);
                hint1.ReRender();

                hint2.gameObject.SetActive(false);
            }
            else
            {
                hint1.gameObject.SetActive(false);
                hint2.gameObject.SetActive(false);
            }
        }
    }
}
