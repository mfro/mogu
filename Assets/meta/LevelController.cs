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
    public GameObject restartText;

    [SerializeField]
    public float CameraTime;

    private class SaveState
    {
        public Vector3 position;
        public Quaternion rotation;
        public Vector2 down;
        public Level level;

        public SaveState(LevelController controller)
        {
            position = controller.playerPhysics.position;
            rotation = controller.player.transform.rotation;
            down = controller.playerFlip.down;

            level = Instantiate(controller.levels[controller.currentLevel]);
            level.gameObject.SetActive(false);
            level.gameObject.name = $"{controller.levels[controller.currentLevel].name} (save state)";

            foreach (var item in level.GetComponentsInChildren<Door>())
            {
                if (item.renderer != null)
                    item.renderer.enabled = true;
            }
        }

        public void Apply(LevelController controller)
        {
            controller.playerPhysics.position = position;
            controller.playerPhysics.velocity = Vector2.zero;
            controller.playerPhysics.UpdatePosition();
            controller.player.transform.rotation = rotation;
            controller.playerFlip.down = down;
            controller.player.gameObject.SetActive(true);
            controller.restartText.SetActive(false);

            controller.levels[controller.currentLevel].gameObject.SetActive(false);
            Destroy(controller.levels[controller.currentLevel].gameObject);

            var replacement = Instantiate(level);
            replacement.gameObject.SetActive(true);
            replacement.name = controller.levels[controller.currentLevel].name;
            controller.levels[controller.currentLevel] = replacement;
        }

        public void Cleanup()
        {
            Destroy(level.gameObject);
        }
    }

    private Level[] levels;
    private int currentLevel;

    private MyCollider playerPhysics => player.GetComponent<MyCollider>();
    private Flippable playerFlip => player.GetComponent<Flippable>();
    private bool moving = false;

    private SaveState restartState;
    private Stack<SaveState> undoStack;

    public void Start()
    {
        levels = FindObjectsOfType<Level>().OrderBy(l => l.name).ToArray();
        currentLevel = 0;

        var pos = levels[0].transform.position;
        pos.z = camera.transform.position.z;
        camera.transform.position = pos;
        player.transform.rotation = levels[0].start.transform.rotation;
        playerPhysics.velocity = Vector2.zero;
        playerPhysics.position = Physics.FromUnity(levels[0].start.transform.position);
        playerPhysics.UpdatePosition();
        playerFlip.down = levels[0].start.transform.rotation * Vector2.down;
        player.gameObject.SetActive(true);
        restartText.SetActive(false);

        restartState = new SaveState(this);
        undoStack = new Stack<SaveState>();

        test();
    }

    private void test()
    {
        var visible = Physics.RectFromCenterSize(Physics.FromUnity(camera.transform.position), Physics.FromUnity(new Vector2(12, 12)));

        foreach (var item in FindObjectsOfType<MyCollider>())
        {
            item.enabled = true;

            var overlap = Physics.Overlap(visible, item.bounds);
            item.enabled = overlap != null;
        }
    }

    public void SaveUndoState()
    {
        undoStack.Push(new SaveState(this));
    }

    async void FixedUpdate()
    {
        if (moving) return;

        if (currentLevel + 1 == levels.Length)
        {

        }
        else
        {
            var d0 = levels[currentLevel].transform.position - player.transform.position;
            var d1 = levels[currentLevel + 1].transform.position - player.transform.position;

            if (d1.sqrMagnitude <= d0.sqrMagnitude && playerFlip.down == levels[currentLevel].exitOrientation)
            {
                var delta = levels[currentLevel + 1].transform.position - levels[currentLevel].transform.position;
                await MoveCamera(delta);
                currentLevel += 1;

                restartState?.Cleanup();
                restartState = new SaveState(this);
                foreach (var item in undoStack) item.Cleanup();
                undoStack.Clear();

                test();
            }
        }
    }

    public void SkipToLevel(Level level)
    {
        var index = Array.IndexOf(levels, level);

        var delta = levels[index].transform.position - levels[currentLevel].transform.position;
        currentLevel = index;
        player.transform.rotation = levels[currentLevel].start.transform.rotation;
        playerPhysics.velocity = Vector2.zero;
        playerPhysics.position = Physics.FromUnity(levels[currentLevel].start.transform.position);
        playerPhysics.UpdatePosition();
        playerFlip.down = levels[currentLevel].start.transform.rotation * Vector2.down;
        camera.transform.position += delta;
        player.gameObject.SetActive(true);
        restartText.SetActive(false);

        restartState?.Cleanup();
        restartState = new SaveState(this);
        foreach (var item in undoStack) item.Cleanup();
        undoStack.Clear();

        test();
    }

    private bool _onRestart = false;
    public void OnRestart(InputAction.CallbackContext c)
    {
        if (c.ReadValueAsButton() && !_onRestart)
            DoRestart();

        _onRestart = c.ReadValueAsButton();
    }

    private bool _onUndo = false;
    public void OnUndo(InputAction.CallbackContext c)
    {
        if (c.ReadValueAsButton() && !_onUndo)
            DoUndo();

        _onUndo = c.ReadValueAsButton();
    }

    private bool _onCheat1 = false;
    public void OnCheat1(InputAction.CallbackContext c)
    {
        if (c.ReadValueAsButton() && !_onCheat1 && !moving)
            SkipToLevel(levels[currentLevel + 1]);

        _onCheat1 = c.ReadValueAsButton();
    }

    private bool _onCheat2 = false;
    public void OnCheat2(InputAction.CallbackContext c)
    {
        if (c.ReadValueAsButton() && !_onCheat2 && !moving)
            SkipToLevel(levels[currentLevel - 1]);

        _onCheat2 = c.ReadValueAsButton();
    }

    public void DoRestart()
    {
        if (moving) return;

        player.gameObject.SetActive(true);
        restartText.SetActive(false);

        restartState.Apply(this);
        foreach (var item in undoStack) item.Cleanup();
        undoStack.Clear();

        test();
    }

    public void DoDeath()
    {
        if (moving) return;

        player.gameObject.SetActive(false);
        restartText.SetActive(true);
    }

    public void DoUndo()
    {
        if (moving || undoStack.Count == 0) return;

        var state = undoStack.Pop();
        state.Apply(this);
        state.Cleanup();

        test();
    }

    private async Task MoveCamera(Vector3 delta)
    {
        moving = true;
        playerFlip.flipping = true;
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
        playerFlip.flipping = false;
        moving = false;
    }
}
