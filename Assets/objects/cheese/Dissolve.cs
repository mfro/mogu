using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class Dissolve : MonoBehaviour
{
    [SerializeField] float dissolveTime;
    [SerializeField] Material Cheese1;
    [SerializeField] Material Cheese2;
    [SerializeField] Material Cheese3;

    private MyStatic physics;
    private MeshRenderer rend;
    private bool dissolveStarted = false;

    void Awake()
    {
        Util.GetComponent(this, out rend);
        Util.GetComponent(this, out physics);
    }

    void FixedUpdate()
    {
        if (dissolveStarted || !physics.enabled) return;

        if (physics.touching.Any(c => c is MyDynamic))
        {
            DoDissolve();
        }
    }

    private async void DoDissolve()
    {
        dissolveStarted = true;
        float elapsedTime = 0f;

        while (elapsedTime < dissolveTime)
        {
            if (elapsedTime < dissolveTime / 3) rend.material = Cheese1;
            else if (elapsedTime < (2 * dissolveTime) / 3) rend.material = Cheese2;
            else rend.material = Cheese3;
            await Task.Yield();
            if (physics.enabled) elapsedTime += Time.deltaTime;
        }

        Destroy(gameObject);
    }
}
