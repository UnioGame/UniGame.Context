namespace UniModules.UniGame.Context.SerializableContext.Runtime.States
{
    using System;
    using System.Threading;
    using Abstract;
    using Core.Runtime.DataFlow.Interfaces;
    using Core.Runtime.Interfaces;
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.DataFlow;

    [Serializable]
    public abstract class AsyncSharedContextState<TValue> : IAsyncContextState<TValue>
    {
        private LifeTimeDefinition _lifeTime = new LifeTimeDefinition();
        private SemaphoreSlim      _semafore = new SemaphoreSlim(1,1);
        private bool               _isActive = false;
        private UniTask<TValue>    _taskHandle;

        public async UniTask<TValue> Execute(IContext value)
        {
            await _semafore.WaitAsync();
            try
            {
                if (!this._isActive) {
                    
                    _isActive = true;
                    var contextLifetime = value.
                        LifeTime.
                        Compose(LifeTime);
                    
                    _taskHandle = this.OnExecute(value, contextLifetime);
                }
            }
            finally
            {
                //When the task is ready, release the semaphore. It is vital to ALWAYS release the semaphore when we are ready, or else we will end up with a Semaphore that is forever locked.
                //This is why it is important to do the Release within a try...finally clause; program execution may crash or take a different path, this way you are guaranteed execution
                _semafore.Release();
            }
            
            var result = await _taskHandle;
            return result;
        }

        public async UniTask Exit() {

            if (!_isActive)
                return;

            _isActive = false;
            
            await OnExit();
            
            _lifeTime?.Release();
            
        }

        public ILifeTime LifeTime => _lifeTime = (_lifeTime ?? new LifeTimeDefinition());

        public bool IsActive => this._isActive;

        protected abstract UniTask<TValue> OnExecute(IContext context, ILifeTime executionLifeTime);

        protected virtual async UniTask OnExit() { }
    }
}