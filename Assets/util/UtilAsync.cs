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
    public static async Task<float> NextFrame()
    {
        var t0 = Time.time;
        await (AsyncHelper.instance.nextFrame ?? (AsyncHelper.instance.nextFrame = new CallbackTask()));
        return Time.time - t0;
    }

    public static async Task<float> NextFixedUpdate()
    {
        var t0 = Time.time;
        await (AsyncHelper.instance.nextFixedUpdate ?? (AsyncHelper.instance.nextFixedUpdate = new CallbackTask()));
        return Time.time - t0;
    }

    public static async Task EveryFrame(Func<float, bool> callback)
    {
        var deltaT = 0f;
        while (true)
        {
            if (!callback(deltaT)) break;
            deltaT = await NextFrame();
        }
    }

    public static async Task Seconds(float seconds, bool obeyPhysics)
    {
        while (seconds >= 0)
        {
            if (obeyPhysics)
            {
                var deltaT = await NextFixedUpdate();
                if (Physics.IsEnabled) seconds -= deltaT;
            }
            else
            {
                var deltaT = await NextFrame();
                seconds -= deltaT;
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

        public CallbackTask nextFrame;
        public CallbackTask nextFixedUpdate;

        void Update()
        {
            if (nextFrame != null)
            {
                var awaiter = nextFrame.Awaiter;
                nextFrame = null;
                awaiter.Complete();
            }
        }

        void FixedUpdate()
        {
            if (nextFixedUpdate != null)
            {
                var awaiter = nextFixedUpdate.Awaiter;
                nextFixedUpdate = null;
                awaiter.Complete();
            }
        }
    }
}
