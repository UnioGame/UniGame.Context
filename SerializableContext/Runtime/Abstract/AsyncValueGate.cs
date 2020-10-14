using System;
using System.Threading;
using UniModules.UniCore.Runtime.DataFlow;
using UniRx;
using Object = UnityEngine.Object;

namespace UniModules.UniGame.SerializableContext.Runtime.Abstract
{
    using Cysharp.Threading.Tasks;
    using UniModules.UniCore.Runtime.Interfaces;
    
    public interface IAsyncContextPrototype<TValue>
    {
        UniTask<TValue> Create(IContext context);
    }

    public class AsyncValueFactory<TValue>
        where TValue : class
    {
        private IAsyncContextPrototype<TValue> valueSource = null;
        private bool                 isProtected = true;
        private bool isShared = true;
        private IAsyncContextPrototype<TValue> _instance;
        
        private TValue _value;
        private static SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1,1);

        public TValue Value => valueSource?.Value;

        public AsyncValueFactory(
            IAsyncContextPrototype<TValue> value,
            bool isProtected = true, 
            bool isShared = true)
        {
            this.valueSource  = value;
            this.isProtected = isProtected;
            this.isShared = isShared;
        }

        public async UniTask<TValue> Create(IContext context)
        {
            await _semaphoreSlim.WaitAsync();
            try
            {
                
                _instance = _instance == null && 
                            isProtected && 
                            valueSource is  Object sourceAsset ? 
                    Object.Instantiate(sourceAsset) as ISourceValue<TValue> :
                    valueSource;

                var value = isShared && _instance.HasValue ? 
                    _instance.Value :
                    await _instance.Create();

                return value;
            }
            finally
            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                _semaphoreSlim.Release();
            }
            
        }

        #region public methods
        
        /// <summary>
        /// service factory
        /// </summary>
        /// <param name="context"></param>
        /// <returns></returns>
        public async UniTask<TApi> CreateServiceAsync(IContext context)
        {
            await semaphoreSlim.WaitAsync();
            try
            {
                if (isSharedSystem && _sharedService == null) {
                    _sharedService = await CreateServiceInternalAsync(context);
                    _sharedService.AddTo(LifeTime);
                }
            }
            finally
            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                semaphoreSlim.Release();
            }
            var service = isSharedSystem ? _sharedService : 
                (await CreateServiceInternalAsync(context)).AddTo(LifeTime);
            
            service.Bind(context);
            return service;
        }
        
        #endregion

        protected sealed override async UniTask<Unit> OnInitialize(IObservable<IContext> source)
        {
            source?.DistinctUntilChanged().
                Subscribe(async x => await CreateServiceAsync(x)).
                AddTo(LifeTime);
            return Unit.Default;
        }

        protected abstract UniTask<TApi> CreateServiceInternalAsync(IContext context);


    }
}
