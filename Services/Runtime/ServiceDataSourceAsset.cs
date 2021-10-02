namespace UniModules.UniGameFlow.GameFlow.Runtime.Services
{
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using Interfaces;
    using UniCore.Runtime.Extension;
    using UniGame.Core.Runtime.Interfaces;
    using UniGame.Core.Runtime.ScriptableObjects;
    using UniModules.UniContextData.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;

    public interface IEmptyGameService : IGameService
    {
        
    }

    public abstract class ServiceDataSourceAsset : ServiceDataSourceAsset<IEmptyGameService>
    {
    }

    public abstract class ServiceDataSourceAsset<TApi> :
        LifetimeScriptableObject,
        IAsyncContextDataSource
        where TApi : class, IGameService
    {
        #region inspector

        public bool isSharedSystem = true;

        public bool waitServiceReady = false;
        
        #endregion

        private        TApi          _sharedService;
        private        SemaphoreSlim _semaphoreSlim;

        #region public methods
    
        public async UniTask<IContext> RegisterAsync(IContext context)
        {
            var service = await CreateServiceAsync(context)
                .AttachExternalCancellation(LifeTime.TokenSource);

            service.AddTo(LifeTime);
            
            if(waitServiceReady)
                await service.IsReady
                    .Where(x => x)
                    .AwaitFirstAsync(context.LifeTime)
                    .AttachExternalCancellation(LifeTime.TokenSource);

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
            await _semaphoreSlim.WaitAsync(LifeTime.TokenSource);
            try {
                if (isSharedSystem && _sharedService == null) {
                    _sharedService = await CreateServiceInternalAsync(context).AttachExternalCancellation(LifeTime.TokenSource);
                    _sharedService.AddTo(LifeTime);
                }
            }
            finally {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                _semaphoreSlim.Release();
            }

            var service = isSharedSystem 
                ? _sharedService 
                : (await CreateServiceInternalAsync(context).AttachExternalCancellation(LifeTime.TokenSource)).AddTo(LifeTime);
            
            return service;
        }

        #endregion

        protected abstract UniTask<TApi> CreateServiceInternalAsync(IContext context);

        protected override void OnActivate()
        {
            _semaphoreSlim?.Dispose();
            _semaphoreSlim = new SemaphoreSlim(1,1).
                AddTo(LifeTime);
            base.OnActivate();
        }

        protected override void OnReset()
        {
            base.OnReset();
            _semaphoreSlim?.Dispose();
            _semaphoreSlim = new SemaphoreSlim(1,1);
            _sharedService = null;
        }
    }
}