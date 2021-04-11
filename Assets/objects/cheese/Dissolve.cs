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
        if (dissolveStarted || !physics.IsEnabled) return;

        if (physics.touching.Any(c => c is MyDynamic))
        {
            DoDissolve();
        }
    }

    private async void DoDissolve()
    {
        dissolveStarted = true;

        await Util.Seconds(dissolveTime / 3, true);
        rend.material = Cheese2;


        await Util.Seconds(dissolveTime / 3, true);
        rend.material = Cheese3;

        await Util.Seconds(dissolveTime / 3, true);
        Destroy(gameObject);
    }
}
