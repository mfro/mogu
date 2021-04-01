using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class Balloon : MonoBehaviour
{
    public SpriteRenderer balloon;

    public Sprite upSprite;
    public Sprite downSprite;
    public Sprite leftSprite;
    public Sprite rightSprite;

    private Vector3 position;
    private Vector3 velocity;
    private Vector3 acceleration;

    private bool released = false;

    private Animator anim;

    void Awake()
    {
        Util.GetComponent(this, out anim);
    }

    void Start()
    {
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
        var down = transform.rotation * Vector3.down;

        if (Vector2.Dot(down, Vector2.up) > 0.5f)
            balloon.sprite = upSprite;
        if (Vector2.Dot(down, Vector2.down) > 0.5f)
            balloon.sprite = downSprite;
        if (Vector2.Dot(down, Vector2.left) > 0.5f)
            balloon.sprite = leftSprite;
        if (Vector2.Dot(down, Vector2.right) > 0.5f)
            balloon.sprite = rightSprite;

        var spriteOut = (transform.rotation * Vector3.forward);
        balloon.flipX = spriteOut.z < 0;

        if (!released) return;

        velocity += acceleration;
        position += velocity;

        transform.position = Physics.ToUnity(Util.Round(position));
    }

    public void DoRelease()
    {
        if (released) return;

        position = Physics.FromUnity(transform.position);

        var up = Util.Round(transform.rotation * Vector3.up);
        var right = Util.Round(transform.rotation * Vector3.right);

        velocity = up / 4f;
        acceleration = (up / 320f) + (right / 3200f);

        released = true;
        anim.SetBool("release", true);
        transform.parent = null;
    }
}
