using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraShake : MonoBehaviour
{
    // Start is called before the first frame update

    public bool shaking = false;

    [SerializeField] float duration, strength;

    public static CameraShake cameraShake;

    void Awake()
    {
        if (cameraShake == null)
        {
            cameraShake = this;
        }
        else
        {
            Destroy(this.gameObject);
            return;
        }
        shaking = false;
    }

    IEnumerator Shake(float strength)
    {
        shaking = true;
        Vector3 startPosition = transform.position;

        float currTime = 0f;

        while (true)
        {

            float x = Random.Range(-1f, 1f) * strength + startPosition.x;
            float y = Random.Range(-1f, 1f) * strength + startPosition.y;

            transform.position = new Vector3(x, y, startPosition.z);

            currTime += Time.deltaTime;
            if (currTime >= duration) break;

            yield return null;
        }
        transform.position = startPosition;
        shaking = false;
    }

    public void DoShake()
    {
        if (shaking) return;

        shaking = true;

        StartCoroutine(Shake(strength));
    }

    public void DoShake(float strengthOverride)
    {
        if (shaking) return;

        shaking = true;

        StartCoroutine(Shake(strengthOverride));
    }
}
