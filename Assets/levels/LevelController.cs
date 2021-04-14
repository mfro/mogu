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
    [SerializeField] public Text[] levelNumber;
    [SerializeField] public Text[] levelName;
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

            controller.player.GetComponent<Animator>().SetBool("dead", false);
            controller.player.isDead = false;

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
        // AudioManager.instance?.PlayMusic(backgroundMusic);

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
        foreach (var t in levelNumber) t.text = $"{World}-{index + 1}";
        foreach (var t in levelName) t.text = levels[index].title;
        levelScreen.gameObject.SetActive(true);

        if (transition)
        {
            if (LevelTransitionSound != null) audioSource.PlayOneShot(LevelTransitionSound);

            var from = currentLevel;
            currentIndex = index;

            var delta = currentLevel.transform.position - from.transform.position;
            await MoveCamera(delta);
            if (this == null) return;
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

            Parallax.parallax?.Invoke(currentLevel.transform.position - camera.transform.position);

            var pos = currentLevel.transform.position;
            pos.z = camera.transform.position.z;
            camera.transform.position = pos;
        }

        CurrentLevel = currentLevel;

        player.UpdateMovement();

        border.Move(currentLevel.transform.position);

        restartState?.Dispose();
        restartState = new SaveState(this);
        foreach (var item in undoStack) item.Dispose();
        undoStack.Clear();

        UpdateColliders();

        var delay = Animations.Animate(2f, Animations.Linear);
        while (!delay.isComplete)
        {
            if (!Physics.IsEnabled) { levelScreen.alpha = 0; return; }
            await delay.NextFrame();
            if (this == null || index != currentIndex) return;
        }

        var fadeOut = Animations.Animate(CameraTime, Animations.Linear);
        while (!fadeOut.isComplete)
        {
            if (!Physics.IsEnabled) { levelScreen.alpha = 0; return; }
            await fadeOut.NextFrame();
            if (this == null || index != currentIndex) return;

            levelScreen.alpha = 1 - fadeOut.progress;
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
        PlayerController.Frozen = true;
        var p0 = camera.transform.position;
        var p1 = p0 + delta;

        var move = Animations.Animate(CameraTime, Animations.EaseInOutQuadratic);
        while (!move.isComplete)
        {
            if (!Physics.IsEnabled) { await Util.NextFrame(); continue; }
            await move.NextFrame();
            if (this == null) return;

            var lastPos = camera.transform.position;
            var nextPos = Vector3.Lerp(p0, p1, move.progress);

            camera.transform.position = nextPos;
            Parallax.parallax?.Invoke(nextPos - lastPos);

            levelScreen.alpha = move.progress;
        }

        PlayerController.Frozen = false;
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

        player.GetComponent<Animator>().SetBool("dead", true);
        player.isDead = true;
        playerPhysics.enabled = false;

        foreach (var item in deathScreen)
        {
            item.SetActive(true);

            foreach (var hint in item.GetComponentsInChildren<Hint>())
            {
                hint.ReRender();
            }
        }

        if (CameraShake.instance != null)
        {
            CameraShake.instance.DoShake(CameraShake.Death);
        }
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
        if (c.ReadValueAsButton() && !_onRestart && !moving && Physics.IsEnabled)
            DoRestart();

        _onRestart = c.ReadValueAsButton();
    }

    private bool _onUndo = false;
    public void OnUndo(InputAction.CallbackContext c)
    {
        if (c.ReadValueAsButton() && !_onUndo && !moving && Physics.IsEnabled)
            DoUndo();

        _onUndo = c.ReadValueAsButton();
    }

    private bool _onCheat1 = false;
    public void OnCheat1(InputAction.CallbackContext c)
    {
        if (c.ReadValueAsButton() && !_onCheat1 && !moving && playerPhysics.IsEnabled)
            GoToLevel(currentIndex + 1, false);

        _onCheat1 = c.ReadValueAsButton();
    }

    private bool _onCheat2 = false;
    public void OnCheat2(InputAction.CallbackContext c)
    {
        if (c.ReadValueAsButton() && !_onCheat2 && !moving && playerPhysics.IsEnabled)
            GoToLevel(currentIndex - 1, false);

        _onCheat2 = c.ReadValueAsButton();
    }
}
