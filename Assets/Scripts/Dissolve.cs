using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    [SerializeField] float dissolveTime;
    private MeshRenderer rend;

    // Start is called before the first frame update
    void Start()
    {
        rend = GetComponent<MeshRenderer>();
    }

    private IEnumerator OnCollisionEnter2D(Collision2D collision)
    {
        Color initialColor = rend.material.color;
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);
        float elapsedTime = 0f;

        while (elapsedTime < dissolveTime)
        {
            elapsedTime += Time.deltaTime;
            Color currentColor = Color.Lerp(initialColor, targetColor, elapsedTime / dissolveTime);
            rend.material.color = currentColor;
            yield return null;
        }

        Destroy(gameObject);
    }
}
