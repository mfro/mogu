using System;
using UnityEngine;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;

public interface IAwaitable
{
    IAwaiter GetAwaiter();
}

public interface IAwaiter : INotifyCompletion
{
    bool IsCompleted { get; }
    void GetResult();
}

public static partial class Util
{
    public static async Task NextFrame()
    {
        await (AsyncHelper.instance.update ?? (AsyncHelper.instance.update = new CallbackTask()));
    }

    public static async Task NextFixedUpdate()
    {
        await (AsyncHelper.instance.fixedUpdate ?? (AsyncHelper.instance.fixedUpdate = new CallbackTask()));
    }

    public static async Task EveryFrame(Func<bool> callback)
    {
        while (true)
        {
            if (!callback()) break;
            await NextFrame();
        }
    }

    public static async Task EveryFixedUpdate(Func<bool> callback)
    {
        while (true)
        {
            if (Physics.IsEnabled && !callback()) break;
            await NextFixedUpdate();
        }
    }

    public static async Task Seconds(float seconds, bool obeyPhysics)
    {
        while (seconds >= 0)
        {
            if (obeyPhysics)
            {
                await NextFixedUpdate();

                if (Physics.IsEnabled) seconds -= Time.fixedDeltaTime;
            }
            else
            {
                await NextFrame();
                seconds -= Time.deltaTime;
            }
        }
    }

    public class CallbackTask : IAwaitable
    {
        public CallbackAwaiter Awaiter { get; } = new CallbackAwaiter();

        public IAwaiter GetAwaiter() => Awaiter;
    }

    public class CallbackAwaiter : IAwaiter
    {
        private bool completed;
        private Action continuation;

        public bool IsCompleted => completed;

        public void GetResult() { }

        public void OnCompleted(Action continuation)
        {
            if (completed)
                continuation();
            else
                this.continuation += continuation;
        }

        public void Complete()
        {
            completed = true;
            this.continuation?.Invoke();
        }
    }

    public class AsyncHelper : MonoBehaviour
    {
        private static AsyncHelper _instance;
        public static AsyncHelper instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new GameObject("async").AddComponent<AsyncHelper>();
                    DontDestroyOnLoad(_instance);
                }

                return _instance;
            }
        }

        public CallbackTask update;
        public CallbackTask fixedUpdate;

        void Update()
        {
            if (update != null)
            {
                var awaiter = update.Awaiter;
                update = null;
                awaiter.Complete();
            }
        }

        void FixedUpdate()
        {
            if (fixedUpdate != null)
            {
                var awaiter = fixedUpdate.Awaiter;
                fixedUpdate = null;
                awaiter.Complete();
            }
        }
    }
}
