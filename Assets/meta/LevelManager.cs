using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelManager : MonoBehaviour
{
    public static void Restart()
    {
        var levelManager = FindObjectOfType<LevelManager>();
        levelManager.DoRestart();
    }

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
        SceneManager.activeSceneChanged += ActiveSceneChanged;
        OnSceneLoaded(SceneManager.GetActiveScene(), LoadSceneMode.Single);
    }

    public static void LoadLevel(Level level)
    {
        var activeScene = SceneManager.GetActiveScene();
        if (activeScene.buildIndex != 0)
        {
            SceneManager.LoadScene(0, LoadSceneMode.Additive);
        }
        else
        {
            var levelManager = FindObjectOfType<LevelManager>();

            if (levelManager.loadIndex != null)
            {
                var previous = levelManager.loaded[levelManager.loadIndex.Value - 1];
                var info = levelManager.DoLoadLevel(level, previous, levelManager.loaded[levelManager.loadIndex.Value]);

                levelManager.loaded[levelManager.loadIndex.Value] = info;
            }
            else
            {
                var previous = levelManager.loaded.LastOrDefault();
                var info = levelManager.DoLoadLevel(level, previous);

                levelManager.loaded.Add(info);
            }

            levelManager.loadIndex = null;
            if (levelManager.loaded.Count < 4 && levelManager.loaded.LastOrDefault() != null)
            {
                var index = levelManager.loaded.Last().scene;
                SceneManager.LoadSceneAsync(index + 1, LoadSceneMode.Additive);
            }
        }
    }

    private static void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        if (scene.buildIndex == 0)
        {
            var active = SceneManager.GetActiveScene();
            if (active == scene)
            {
                SceneManager.LoadScene(1, LoadSceneMode.Additive);
            }
            else
            {
                var level = active.GetRootGameObjects().Select(o => o.GetComponent<Level>()).Single(o => o != null);
                SceneManager.SetActiveScene(scene);
                LoadLevel(level);
            }
        }
        else
        {
            var level = scene.GetRootGameObjects().Select(o => o.GetComponent<Level>()).FirstOrDefault(o => o != null);
            if (level == null)
            {
                foreach (var o in scene.GetRootGameObjects())
                    o.SetActive(false);
                LoadLevel(null);
                // var active = SceneManager.GetActiveScene();
                // SceneManager.UnloadSceneAsync(active);
            }
        }
    }

    private static void ActiveSceneChanged(Scene from, Scene to)
    {
        foreach (var o in SceneManager.GetActiveScene().GetRootGameObjects())
            o.SetActive(true);
    }

    [SerializeField]
    public new Camera camera;
    [SerializeField]
    public PlayerController player;

    [SerializeField]
    public float CameraPanTime;

    private Vector2 initialDown;
    private Quaternion initialRotation;

    private class LevelInfo
    {
        public Action onComplete;
        public Vector3 start;
        public Vector3 center;
        public Vector3 end;

        public Quaternion endRotation;
        public Quaternion startRotation;

        public Level root;
        public int scene;
    }

    private int? loadIndex = null;
    private List<LevelInfo> loaded = new List<LevelInfo> { null };
    private Flippable playerFlippable => player.GetComponent<Flippable>();

    private void DoRestart()
    {
        if (loadIndex == 1) return;

        var current = loaded[1];
        loadIndex = 1;
        Destroy(current.root.gameObject);

        playerFlippable.down = current.startRotation * Vector2.down;
        player.transform.rotation = current.startRotation;
        player.transform.position = current.start;

        SceneManager.LoadSceneAsync(current.scene, LoadSceneMode.Additive);
    }

    private LevelInfo DoLoadLevel(Level level, LevelInfo previous, LevelInfo old = null)
    {
        if (level == null)
        {
            previous.onComplete += () =>
            {
                SceneManager.UnloadSceneAsync(SceneManager.GetActiveScene());
            };

            return null;
        }

        var scene = level.gameObject.scene;
        SceneManager.MoveGameObjectToScene(level.gameObject, SceneManager.GetActiveScene());
        SceneManager.UnloadSceneAsync(scene);

        level.transform.parent = transform;

        var down = Vector2.down;
        if (previous != null)
        {
            down = previous.endRotation * down;
            down.x = Mathf.Round(down.x);
            down.y = Mathf.Round(down.y);

            level.transform.rotation *= previous.endRotation;

            foreach (var flippable in level.GetComponentsInChildren<Flippable>())
            {
                flippable.down = previous.endRotation * flippable.down;
                flippable.down.x = Mathf.Round(flippable.down.x);
                flippable.down.y = Mathf.Round(flippable.down.y);
            }
        }

        if (previous != null)
        {
            var nextStartPosition = level.start.transform.position;
            var position = previous.end - nextStartPosition;
            level.transform.localPosition = position;

            previous.onComplete += async () =>
            {
                await MoveCamera(position);
                while (loaded.IndexOf(previous) > 0)
                {
                    if (loaded[0] != null) Destroy(loaded[0].root.gameObject);
                    loaded.RemoveAt(0);
                }

                if (loaded.Count < 4 && loaded.LastOrDefault() != null)
                {
                    var index = loaded.Last().scene;
                    SceneManager.LoadSceneAsync(index + 1, LoadSceneMode.Additive);
                }
            };
        }
        else
        {
            player.transform.position = level.start.transform.position;
        }

        var info = new LevelInfo
        {
            start = level.start.transform.position,
            center = level.transform.position,
            end = level.end.transform.position,
            endRotation = level.end.transform.rotation,
            startRotation = Quaternion.FromToRotation(Vector2.down, down),
            root = level,
            scene = scene.buildIndex,
            onComplete = old?.onComplete,
        };

        level.end.next = () => info.onComplete?.Invoke();

        return info;
    }

    private async Task MoveCamera(Vector3 target)
    {
        playerFlippable.flipping = true;
        var p0 = camera.transform.position;
        target.z += p0.z;

        var t0 = Time.time;
        var t1 = t0 + CameraPanTime;

        while (Time.time < t1)
        {
            camera.transform.position = Vector3.Lerp(p0, target, (Time.time - t0) / CameraPanTime);
            await Task.Yield();
        }

        camera.transform.position = target;
        playerFlippable.flipping = false;
    }
}
