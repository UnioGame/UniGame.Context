using System;
using UniCore.Runtime.ProfilerTools;
using UniModules.UniCore.Runtime.ProfilerTools;

namespace UniModules.UniGameFlow.GameFlow.Runtime.Services
{
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using global::UniGame.GameFlow.Runtime.Interfaces;
    using global::UniGame.Core.Runtime;
    using global::UniGame.Core.Runtime.ScriptableObjects;
    using global::UniGame.Context.Runtime;

    public interface IEmptyGameService : IGameService
    {
        
    }

    public abstract class ServiceDataSourceAsset : ServiceDataSourceAsset<IEmptyGameService>
    {
    }
    
        
    public abstract class ServiceDataSourceAsset<TApi> :
        LifetimeScriptableObject,
        IAsyncDataSource
        where TApi : class, IGameService
    {
        #region inspector
        
        public bool enabled = true;

        public bool isSharedSystem = true;

        public bool waitServiceReady = false;
        
        public bool ownServiceLifeTime = true;
        
        #endregion

        private        TApi          _sharedService;
        private        SemaphoreSlim _semaphoreSlim;

        #region public methods
    
        public async UniTask<IContext> RegisterAsync(IContext context)
        {

#if UNITY_EDITOR || GAME_LOGS_ENABLED || DEBUG
            var profileId = ProfilerUtils.BeginWatch($"Service_{typeof(TApi).Name}");
            GameLog.LogRuntime($"GameService Profiler Init : {typeof(TApi).Name} | {DateTime.Now}");
#endif 
            
            var service = await CreateServiceAsync(context)
                .AttachExternalCancellation(LifeTime.Token);

#if UNITY_EDITOR || GAME_LOGS_ENABLED || DEBUG
            var watchResult = ProfilerUtils.GetWatchData(profileId);
            GameLog.LogRuntime($"GameService Profiler Create : {typeof(TApi).Name} | Take {watchResult.watchMs} | {DateTime.Now}");
#endif

#if UNITY_EDITOR || GAME_LOGS_ENABLED || DEBUG
            watchResult = ProfilerUtils.GetWatchData(profileId,true);
            GameLog.LogRuntime($"GameService Profiler Publish: {typeof(TApi).Name} | Take {watchResult.watchMs} | {DateTime.Now}");
#endif
            
            context.Publish(service);
            return context;
        }
    
        /// <summary>
        /// service factory
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async UniTask<TApi> CreateServiceAsync(IContext context)
        {
            if (!enabled)
                await UniTask.Never<TApi>(LifeTime.Token);

            _semaphoreSlim ??= new SemaphoreSlim(1,1);
            await _semaphoreSlim.WaitAsync(LifeTime.Token);
            
            try {
                if (isSharedSystem && _sharedService == null) {
                    _sharedService = await CreateServiceInternalAsync(context).AttachExternalCancellation(LifeTime.Token);
                    if (ownServiceLifeTime)
                    {
                        _sharedService.AddTo(LifeTime);
                    }
                }
            }
            finally {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                _semaphoreSlim.Release();
            }

            var service = isSharedSystem 
                ? _sharedService 
                : ownServiceLifeTime 
                    ? (await CreateServiceInternalAsync(context).AttachExternalCancellation(LifeTime.Token)).AddTo(LifeTime)
                    : (await CreateServiceInternalAsync(context).AttachExternalCancellation(LifeTime.Token));
            
            return service;
        }

        #endregion

        protected abstract UniTask<TApi> CreateServiceInternalAsync(IContext context);

        protected sealed override void OnActivate()
        {
            OnReset();
        }

        private void OnReset()
        {
            _semaphoreSlim?.Dispose();
            _semaphoreSlim = new SemaphoreSlim(1,1).AddTo(LifeTime);
            _sharedService = null;
        }
    }
}