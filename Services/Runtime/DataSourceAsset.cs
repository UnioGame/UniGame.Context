using UniCore.Runtime.ProfilerTools;
using UniModules.UniCore.Runtime.ProfilerTools;

namespace UniModules.UniGameFlow.GameFlow.Runtime.Services
{
    using System;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UniContextData.Runtime.Interfaces;
    using UniCore.Runtime.Rx.Extensions;
    using UniGame.Core.Runtime.Interfaces;
    using UniGame.Core.Runtime.ScriptableObjects;

    public abstract class DataSourceAsset<TApi> :
        LifetimeScriptableObject,
        IAsyncContextDataSource
    {
        #region inspector

        public bool isSharedSystem = true;
        
        #endregion

        private TApi          _value;
        private SemaphoreSlim _semaphoreSlim;

        #region public methods
    
        public async UniTask<IContext> RegisterAsync(IContext context)
        {
#if UNITY_EDITOR || GAME_LOGS_ENABLED
            var profileId = ProfilerUtils.BeginWatch($"Service_{typeof(TApi).Name}");
            GameLog.Log($"GameService Profiler Init : {typeof(TApi).Name} | {DateTime.Now}");
#endif 
            
            var result = await CreateAsync(context)
                .AttachExternalCancellation(LifeTime.TokenSource);
            
#if UNITY_EDITOR || GAME_LOGS_ENABLED
            var watchResult = ProfilerUtils.GetWatchData(profileId);
            GameLog.Log($"GameService Profiler Create : {typeof(TApi).Name} | Take {watchResult.watchMs} | {DateTime.Now}");
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
            await _semaphoreSlim.WaitAsync(LifeTime.TokenSource);
            try {
                if (isSharedSystem && _value == null) {
                    _value = await CreateInternalAsync(context).AttachExternalCancellation(LifeTime.TokenSource);
                    if(_value is IDisposable disposable)
                        disposable.AddTo(LifeTime);
                }
            }
            finally {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                _semaphoreSlim.Release();
            }

            if (isSharedSystem)
                return _value;

            var value = await CreateInternalAsync(context).AttachExternalCancellation(LifeTime.TokenSource);
            if(value is IDisposable disposableValue)
                disposableValue.AddTo(LifeTime);
            
            return value;
        }

        #endregion

        protected abstract UniTask<TApi> CreateInternalAsync(IContext context);

        protected override void OnActivate()
        {
            _semaphoreSlim?.Dispose();
            _semaphoreSlim = new SemaphoreSlim(1,1).AddTo(LifeTime);
            base.OnActivate();
        }

        protected override void OnReset()
        {
            base.OnReset();
            _semaphoreSlim?.Dispose();
            _semaphoreSlim = new SemaphoreSlim(1,1);
            _value         = default;
        }
    }
}