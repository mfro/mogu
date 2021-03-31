using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    private Vector3 position;
    private Vector3 velocity;
    private Vector3 acceleration;

    private bool released = false;

    private Animator anim;

    void OnEnable()
    {
        anim = GetComponent<Animator>();
        DoIdle();
    }

    async void DoIdle()
    {
        while (this != null && !released)
        {
            var delay = 2.0f + 2.0f * Random.value;
            var t0 = Time.time;

            anim.SetBool("bob", true);

            await Task.Yield();

            anim.SetBool("bob", false);

            while (this != null && !released && Time.time - t0 < delay) await Task.Yield();
        }
    }

    void FixedUpdate()
    {
        if (!released) return;

        velocity += acceleration;
        position += velocity;

        transform.position = Physics.ToUnity(Physics.Round(position));
    }

    public void DoRelease()
    {
        if (released) return;

        position = Physics.FromUnity(transform.position);

        var up = Physics.Round(transform.rotation * Vector3.up);
        var right = Physics.Round(transform.rotation * Vector3.right);

        velocity = up / 4f;
        acceleration = (up / 320f) + (right / 3200f);

        released = true;
        anim.SetBool("release", true);
        transform.parent = null;
    }
}
