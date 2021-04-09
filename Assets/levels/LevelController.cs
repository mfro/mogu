using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using TMPro;
using UnityEngine;
using UnityEngine.InputSystem;

public class LevelController : MonoBehaviour
{
    public static Level CurrentLevel;

    [SerializeField] public new GameObject camera;
    [SerializeField] public LevelBorder border;
    [SerializeField] public PlayerController player;
    [SerializeField] public GameObject[] deathScreen;
    [SerializeField] public TextMeshProUGUI levelText;
    [SerializeField] public CanvasGroup levelScreen;
    [SerializeField] public float CameraTime;
    [SerializeField] public AudioClip LevelTransitionSound;
    [SerializeField] public int World;

    [SerializeField] private Audio backgroundMusic;

    private AudioSource audioSource;

    public Action PausePressed;

    private class SaveState : IDisposable
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

            level = Instantiate(controller.levels[controller.currentIndex]);
            level.gameObject.SetActive(false);
            level.gameObject.name = $"{controller.levels[controller.currentIndex].name} (save state)";
        }

        public void Apply(LevelController controller)
        {
            FlipPanel.cancelFlip?.Invoke();

            controller.playerPhysics.position = position;
            controller.playerPhysics.velocity = Vector2.zero;
            controller.playerPhysics.remainder = Vector2.zero;
            controller.playerPhysics.UpdatePosition();
            controller.player.transform.rotation = rotation;
            controller.playerFlip.down = down;
            controller.player.gameObject.SetActive(true);
            foreach (var item in controller.deathScreen) item.SetActive(false);

            controller.player.UpdateMovement();

            controller.levels[controller.currentIndex].gameObject.SetActive(false);
            Destroy(controller.levels[controller.currentIndex].gameObject);

            var replacement = Instantiate(level);
            replacement.gameObject.SetActive(true);
            replacement.name = controller.levels[controller.currentIndex].name;
            controller.levels[controller.currentIndex] = replacement;
        }

        public void Dispose()
        {
            Destroy(level.gameObject);
        }
    }

    private Level[] levels;
    private int currentIndex;
    private Level currentLevel => levels[currentIndex];

    private MyDynamic playerPhysics => player.GetComponent<MyDynamic>();
    private Flippable playerFlip => player.GetComponent<Flippable>();
    private bool moving = false;

    private SaveState restartState;
    private Stack<SaveState> undoStack;

    void Awake()
    {
        Util.GetComponent(this, out audioSource);
    }

    void Start()
    {
        AudioManager.instance?.PlayMusic(backgroundMusic);

        levels = FindObjectsOfType<Level>().OrderBy(l => l.name).ToArray();
        undoStack = new Stack<SaveState>();

#if UNITY_EDITOR
        for (var i = 0; i < levels.Length; ++i)
        {
            if (UnityEditor.Selection.activeTransform?.IsChildOf(levels[i].transform) == true)
            {
                currentIndex = i;
                break;
            }
        }
#endif

        GoToLevel(currentIndex, false);
    }

    public void SaveUndoState()
    {
        undoStack.Push(new SaveState(this));
    }

    public void Advance()
    {
        if (moving) return;

        if (currentIndex + 1 < levels.Length)
        {
            GoToLevel(currentIndex + 1, true);
        }
        else
        {
            SceneController.instance.SwitchScene(SceneController.NextLevel);
        }
    }

    public async void GoToLevel(int index, bool transition)
    {
        if (index < 0 || index >= levels.Length) return;

        levels[index].start.MarkReached();
        levelScreen.alpha = 1;
        levelText.text = $"{World}-{index + 1}\n{levels[index].title}";
        levelScreen.gameObject.SetActive(true);

        if (transition)
        {
            if (LevelTransitionSound != null) audioSource.PlayOneShot(LevelTransitionSound);

            var from = currentLevel;
            currentIndex = index;

            var delta = currentLevel.transform.position - from.transform.position;
            await MoveCamera(delta);
        }
        else
        {
            currentIndex = index;

            player.transform.rotation = currentLevel.start.transform.rotation;
            playerFlip.down = Util.Round(currentLevel.start.transform.rotation * Vector2.down);

            playerPhysics.velocity = Vector2.zero;
            playerPhysics.position = Physics.FromUnity(currentLevel.start.transform.position);
            playerPhysics.UpdatePosition();
            levelScreen.alpha = 1;
        }

        CurrentLevel = currentLevel;

        player.UpdateMovement();

        var pos = currentLevel.transform.position;
        pos.z = camera.transform.position.z;
        camera.transform.position = pos;

        border.Move(currentLevel.transform.position);

        restartState?.Dispose();
        restartState = new SaveState(this);
        foreach (var item in undoStack) item.Dispose();
        undoStack.Clear();

        UpdateColliders();

        var end = Time.time + 2f;
        while (Time.time < end) await Task.Yield();

        var t0 = Time.time;
        var t1 = t0 + CameraTime;

        while (Time.time < t1)
        {
            levelScreen.alpha = 1 - (Time.time - t0) / CameraTime;
            await Task.Yield();
            if (currentIndex != index || !Physics.IsEnabled) return;
        }

        levelScreen.alpha = 0;
        levelScreen.gameObject.SetActive(false);
    }

    private void UpdateColliders()
    {
        border.UpdateBorder();

        var visible = Util.RectFromCenterSize(Physics.FromUnity(camera.transform.position), Physics.FromUnity(new Vector2(12, 12)));

        foreach (var item in FindObjectsOfType<MyCollider>())
        {
            item.enabled = true;

            var overlap = Physics.Overlap(visible, item.bounds);
            item.enabled = overlap != null;
        }
    }

    private async Task MoveCamera(Vector3 delta)
    {
        moving = true;
        Physics.IsEnabled = false;
        var p0 = camera.transform.position;
        var p1 = p0 + delta;

        var t0 = Time.time;
        var t1 = t0 + CameraTime;

        while (Time.time < t1)
        {
            levelScreen.alpha = (Time.time - t0) / CameraTime;
            camera.transform.position = Vector3.Lerp(p0, p1, (Time.time - t0) / CameraTime);
            await Task.Yield();
        }

        camera.transform.position = p1;
        levelScreen.alpha = 1;
        Physics.IsEnabled = true;
        moving = false;
    }

    public void DoRestart()
    {
        if (moving) return;

        player.gameObject.SetActive(true);
        foreach (var item in deathScreen) item.SetActive(false);

        restartState.Apply(this);
        foreach (var item in undoStack) item.Dispose();
        undoStack.Clear();

        UpdateColliders();
    }

    public void DoDeath()
    {
        if (moving) return;

        player.gameObject.SetActive(false);
        foreach (var item in deathScreen) item.SetActive(true);
    }

    public void DoUndo()
    {
        if (moving) return;

        if (undoStack.Any())
        {
            player.gameObject.SetActive(true);
            foreach (var item in deathScreen) item.SetActive(false);

            var state = undoStack.Pop();
            state.Apply(this);
            state.Dispose();

            UpdateColliders();
        }
        else
        {
            DoRestart();
        }
    }

    private static float Distance(Vector2 a, Vector2 b, int dim)
    {
        return Mathf.Abs(a[dim] - b[dim]);
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
            GoToLevel(currentIndex + 1, false);

        _onCheat1 = c.ReadValueAsButton();
    }

    private bool _onCheat2 = false;
    public void OnCheat2(InputAction.CallbackContext c)
    {
        if (c.ReadValueAsButton() && !_onCheat2 && !moving)
            GoToLevel(currentIndex - 1, false);

        _onCheat2 = c.ReadValueAsButton();
    }
}
