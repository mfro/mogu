using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelController : MonoBehaviour
{
    [SerializeField]
    public new Camera camera;

    [SerializeField]
    public PlayerController player;

    [SerializeField]
    public float CameraTime;

    private class SaveState
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector2 down;

        public SaveState(PlayerController player)
        {
            position = player.transform.position;
            rotation = player.transform.rotation;
            down = player.GetComponent<Flippable>().down;
        }

        public void Apply(PlayerController player)
        {
            player.transform.position = position;
            player.transform.rotation = rotation;
            player.GetComponent<Flippable>().down = down;
        }
    }

    private Level[] levels;
    private GameObject[] initialStates;
    private int currentLevel;

    private PhysicsObject playerPhysics => player.GetComponent<PhysicsObject>();
    private Flippable playerFlippable => player.GetComponent<Flippable>();
    private bool moving = false;

    private SaveState saveState;

    public void Start()
    {
        levels = FindObjectsOfType<Level>().OrderBy(l => l.name).ToArray();
        currentLevel = 0;

        var pos = levels[0].transform.position;
        pos.z = camera.transform.position.z;
        camera.transform.position = pos;
        player.transform.position = levels[0].start.transform.position;
        player.transform.rotation = levels[0].start.transform.rotation;
        playerFlippable.down = levels[0].start.transform.rotation * Vector2.down;

        saveState = new SaveState(player);

        initialStates = levels.Select(l =>
        {
            var o = Instantiate(l.gameObject);
            o.SetActive(false);
            o.name = $"{l.name} (initial state)";
            return o;
        }).ToArray();
    }

    public async void FixedUpdate()
    {
        if (moving) return;

        if (currentLevel + 1 == levels.Length)
        {

        }
        else
        {
            var d0 = levels[currentLevel].transform.position - player.transform.position;
            var d1 = levels[currentLevel + 1].transform.position - player.transform.position;

            if (d1.sqrMagnitude <= d0.sqrMagnitude && playerPhysics.grounded)
            {
                var delta = levels[currentLevel + 1].transform.position - levels[currentLevel].transform.position;
                await MoveCamera(delta);
                currentLevel += 1;
                saveState = new SaveState(player);
            }
        }
    }

    public void SkipToLevel(Level level)
    {
        var index = Array.IndexOf(levels, level);

        var delta = levels[index].transform.position - levels[currentLevel].transform.position;
        currentLevel = index;
        player.transform.position = levels[currentLevel].start.transform.position;
        player.transform.rotation = levels[currentLevel].start.transform.rotation;
        playerFlippable.down = levels[currentLevel].start.transform.rotation * Vector2.down;
        camera.transform.position += delta;
        saveState = new SaveState(player);
    }

    public void OnRestart(InputAction.CallbackContext c)
    {
        if (c.ReadValueAsButton()) DoRestart();
    }

    private bool dedupe = false;
    public void OnCheat1(InputAction.CallbackContext c)
    {
        if (c.ReadValueAsButton() && !moving)
        {
            if (dedupe) { dedupe = false; return; }
            dedupe = true;
            SkipToLevel(levels[currentLevel + 1]);
        }
    }

    public void OnCheat2(InputAction.CallbackContext c)
    {
        if (c.ReadValueAsButton() && !moving)
        {
            if (dedupe) { dedupe = false; return; }
            dedupe = true;
            SkipToLevel(levels[currentLevel - 1]);
        }
    }

    public void DoRestart()
    {
        if (moving) return;

        levels[currentLevel].gameObject.SetActive(false);
        Destroy(levels[currentLevel].gameObject);
        var replacement = Instantiate(initialStates[currentLevel]);
        replacement.SetActive(true);
        replacement.name = levels[currentLevel].name;
        levels[currentLevel] = replacement.GetComponent<Level>();
        saveState.Apply(player);
    }

    private async Task MoveCamera(Vector3 delta)
    {
        moving = true;
        playerFlippable.flipping = true;
        var p0 = camera.transform.position;
        var p1 = p0 + delta;

        var t0 = Time.time;
        var t1 = t0 + CameraTime;

        while (Time.time < t1)
        {
            camera.transform.position = Vector3.Lerp(p0, p1, (Time.time - t0) / CameraTime);
            await Task.Yield();
        }

        camera.transform.position = p1;
        playerFlippable.flipping = false;
        moving = false;
    }
}
