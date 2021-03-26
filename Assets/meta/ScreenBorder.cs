using System.Threading.Tasks;
using UnityEngine;

public class ScreenBorder : MonoBehaviour
{
    [SerializeField] public MyStatic left;
    [SerializeField] public MyStatic right;
    [SerializeField] public MyStatic top;
    [SerializeField] public MyStatic bottom;

    [SerializeField] public MyDynamic player;

    void Start()
    {
        player.flip.EndFlip += (delta) => UpdateBorder();

        left.mask = CollisionMask.PlayerOnly;
        right.mask = CollisionMask.PlayerOnly;
        top.mask = CollisionMask.PlayerOnly;
        bottom.mask = CollisionMask.PlayerOnly;
    }

    public async void UpdateBorder()
    {
        await Task.Yield();

        left.enabled = player.down != Vector2.left;
        right.enabled = player.down != Vector2.right;
        top.enabled = player.down != Vector2.up;
        bottom.enabled = player.down != Vector2.down;
    }

    public void Move(Vector2 position)
    {
        transform.position = position;

        left.position = Physics.FromUnity(left.transform.position);
        right.position = Physics.FromUnity(right.transform.position);
        top.position = Physics.FromUnity(top.transform.position);
        bottom.position = Physics.FromUnity(bottom.transform.position);

        UpdateBorder();
    }
}
