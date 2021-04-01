using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    [SerializeField] float dissolveTime;
    [SerializeField] Material Cheese1;
    [SerializeField] Material Cheese2;
    [SerializeField] Material Cheese3;

    private MeshRenderer rend;
    private Flippable flip;

    void Awake()
    {
        Util.GetComponent(this, out rend);
        Util.GetComponent(this, out flip);
    }

    private IEnumerator OnCollisionEnter2D(Collision2D collision)
    {
        Color initialColor = rend.material.color;
        Color targetColor = new Color(initialColor.r, initialColor.g, initialColor.b, 0f);
        float elapsedTime = 0f;

        while (elapsedTime < dissolveTime)
        {
            if (!flip.flipping)
                elapsedTime += Time.deltaTime;

            if (elapsedTime < dissolveTime / 3) rend.material = Cheese1;
            else if (elapsedTime < (2 * dissolveTime) / 3) rend.material = Cheese2;
            else rend.material = Cheese3;
            /*
            Color currentColor = Color.Lerp(initialColor, targetColor, elapsedTime / dissolveTime);
            rend.material.color = currentColor;
            */
            yield return null;
        }

        Destroy(gameObject);
    }
}
