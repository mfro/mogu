using System;
using System.Linq;
using UnityEngine;

public class Checkpoint : MonoBehaviour
{
    [SerializeField] public Balloon balloon;

    private MyCollider physics;

    void Awake()
    {
        Util.GetComponent(this, out physics);
    }

    void Start()
    {
        physics.mask |= CollisionMask.Player;
    }

    void FixedUpdate()
    {
        if (!physics.enabled) return;

        var down = (Vector2)Util.Round(transform.rotation * Vector3.down);

        var (other, _) = Physics.AllOverlaps(physics)
            .FirstOrDefault(o => o.Item1.GetComponent<PlayerController>() != null);

        if (other != null && other.flip.down == down)
        {
            MarkReached();

            var levelController = FindObjectOfType<LevelController>();
            levelController.Advance();
        }
    }

    public void MarkReached()
    {
        enabled = false;
        physics.enabled = false;
        balloon.DoRelease();
    }
}
