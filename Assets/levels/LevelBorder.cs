using System.Threading.Tasks;
using UnityEngine;

public class LevelBorder : MonoBehaviour
{
    [SerializeField] public MyStatic up;
    [SerializeField] public MyStatic down;
    [SerializeField] public MyStatic left;
    [SerializeField] public MyStatic right;

    [SerializeField] public MyDynamic player;

    void Start()
    {
        player.flip.EndFlip += (delta) => UpdateBorder();

        up.mask = CollisionMask.Player;
        down.mask = CollisionMask.Player;
        left.mask = CollisionMask.Player;
        right.mask = CollisionMask.Player;
    }

    public async void UpdateBorder()
    {
        await Util.NextFixedUpdate();
        if (this == null) return;

        up.enabled = player.down != Vector2.up;
        down.enabled = player.down != Vector2.down;
        left.enabled = player.down != Vector2.left;
        right.enabled = player.down != Vector2.right;
    }

    public void Move(Vector2 position)
    {
        transform.position = position;

        up.position = Physics.FromUnity(up.transform.position);
        down.position = Physics.FromUnity(down.transform.position);
        left.position = Physics.FromUnity(left.transform.position);
        right.position = Physics.FromUnity(right.transform.position);

        UpdateBorder();
    }
}
