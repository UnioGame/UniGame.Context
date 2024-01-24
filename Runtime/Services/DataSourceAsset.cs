using UniCore.Runtime.ProfilerTools;
using UniModules.UniCore.Runtime.ProfilerTools;

namespace UniGame.GameFlow.Runtime.Services
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using global::UniGame.Context.Runtime;
    using global::UniGame.Core.Runtime;
    using global::UniGame.Core.Runtime.ScriptableObjects;

    public abstract class DataSourceAsset<TApi> :
        LifetimeScriptableObject,
        IAsyncDataSource
    {
        #region inspector

        public bool enabled = true;

        public bool isSharedSystem = true;

        public bool ownDataLifeTime = true;

        #endregion

        private TApi _sharedValue;
        private SemaphoreSlim _semaphoreSlim;

        #region public methods

        public async UniTask<IContext> RegisterAsync(IContext context)
        {
            if (!enabled)
                await UniTask.Never<IContext>(LifeTime.Token);

#if UNITY_EDITOR || GAME_LOGS_ENABLED || DEBUG
            var profileId = ProfilerUtils.BeginWatch($"Service_{typeof(TApi).Name}");
            GameLog.Log($"GameService Profiler Init : {typeof(TApi).Name} | {DateTime.Now}");
#endif

            var result = await CreateAsync(context)
                .AttachExternalCancellation(LifeTime.Token);

#if UNITY_EDITOR || GAME_LOGS_ENABLED || DEBUG
            var watchResult = ProfilerUtils.GetWatchData(profileId);
            GameLog.Log(
                $"GameService Profiler Create : {typeof(TApi).Name} | Take {watchResult.watchMs} | {DateTime.Now}");
#endif

            context.Publish(result);
            return context;
        }

        /// <summary>
        /// service factory
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async UniTask<TApi> CreateAsync(IContext context)
        {
            await _semaphoreSlim.WaitAsync(LifeTime.Token);
            try
            {
                if (isSharedSystem && _sharedValue == null)
                {
                    _sharedValue = await CreateInternalAsync(context)
                        .AttachExternalCancellation(LifeTime.Token);
                    
                    if (_sharedValue is IDisposable disposable && ownDataLifeTime)
                        disposable.AddTo(LifeTime);
                }
            }
            finally
            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                _semaphoreSlim.Release();
            }

            if (isSharedSystem)
                return _sharedValue;

            var value = await CreateInternalAsync(context)
                .AttachExternalCancellation(LifeTime.Token);

            if (value is IDisposable disposableValue && ownDataLifeTime)
                disposableValue.AddTo(LifeTime);

            return value;
        }

        #endregion

        protected abstract UniTask<TApi> CreateInternalAsync(IContext context);

        protected override void OnActivate()
        {
            OnReset();
        }

        private void OnReset()
        {
            _semaphoreSlim?.Dispose();
            _semaphoreSlim = new SemaphoreSlim(1, 1).AddTo(LifeTime);
            _sharedValue = default;
        }
    }
}