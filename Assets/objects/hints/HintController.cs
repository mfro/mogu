using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class HintController : MonoBehaviour
{
    public Hint cw, ccw, vertical, horizontal;

    public MyCollider player;

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
            cw.gameObject.SetActive(false);
            ccw.gameObject.SetActive(false);
            vertical.gameObject.SetActive(false);
            horizontal.gameObject.SetActive(false);
            previousPanel = null;
        }
        else
        {
            var x = panel.transform.localScale.x / 2;
            var y = panel.transform.localScale.y / 2;

            if (panel.flip1 == FlipKind.CW || panel.flip2 == FlipKind.CW)
            {
                cw.transform.position = panel.transform.position + new Vector3(x, y, -10);
                cw.gameObject.SetActive(true);
                cw.ReRender();
                cw.transform.localScale = LevelController.CurrentLevel.title == "Walking on the Walls"
                    ? new Vector3(2, 2, 1)
                    : new Vector3(1, 1, 1);
            }
            else
            {
                cw.gameObject.SetActive(false);
            }

            if (panel.flip1 == FlipKind.CCW || panel.flip2 == FlipKind.CCW)
            {
                ccw.transform.position = panel.transform.position + new Vector3(-x, y, -10);
                ccw.gameObject.SetActive(true);
                ccw.ReRender();
                ccw.transform.localScale = LevelController.CurrentLevel.title == "Walking on the Walls"
                    ? new Vector3(2, 2, 1)
                    : new Vector3(1, 1, 1);
            }
            else
            {
                ccw.gameObject.SetActive(false);
            }

            var relativeVertical = player.flip.down.x == 0 ? FlipKind.Vertical : FlipKind.Horizontal;
            var relativeHorizontal = player.flip.down.x == 0 ? FlipKind.Horizontal : FlipKind.Vertical;

            if (panel.flip1 == relativeVertical || panel.flip2 == relativeVertical)
            {
                vertical.transform.position = panel.transform.position + new Vector3(0, y, -10);
                vertical.gameObject.SetActive(true);
                vertical.ReRender();
                vertical.transform.localScale = LevelController.CurrentLevel.title == "Walking on the Ceiling"
                    ? new Vector3(2, 2, 1)
                    : new Vector3(1, 1, 1);
            }
            else
            {
                vertical.gameObject.SetActive(false);
            }

            if (panel.flip1 == relativeHorizontal || panel.flip2 == relativeHorizontal)
            {
                horizontal.transform.position = panel.transform.position + new Vector3(x, 0, -10);
                horizontal.gameObject.SetActive(true);
                horizontal.ReRender();
            }
            else
            {
                horizontal.gameObject.SetActive(false);
            }
        }
    }
}
