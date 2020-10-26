namespace UniModules.UniGame.Context.SerializableContext.Runtime.States
{
    using System;
    using Abstract;
    using Core.Runtime.DataFlow.Interfaces;
    using Core.Runtime.Interfaces;
    using Core.Runtime.Rx;
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.DataFlow;
    using UniRx;

    [Serializable]
    public abstract class AsyncContextState<TValue> : IAsyncContextState<TValue>
    {
        private LifeTimeDefinition              _lifeTime;
        private bool                            _isActive;
        private UniTask<TValue>                 _taskHandle;
        private RecycleReactiveProperty<TValue> _value;
        

        public async UniTask<TValue> Execute(IContext value) {
            //state already active
            if (_isActive)
                return await _taskHandle;

            Initialize();
            
            //cleanup value on reset
            LifeTime.AddCleanUpAction(() => _value.Release());
            
            _isActive = true;
            var contextLifetime = value.
                LifeTime.
                Compose(LifeTime);
                    
            _taskHandle = this.OnExecute(value, contextLifetime);
            
            var result = await _taskHandle;

            await OnComplete(result, value,contextLifetime);

            return result;
        }

        public async UniTask Exit() {

            if (!_isActive)
                return;

            await OnExit();
            
            _isActive = false;

            _lifeTime?.Release();
            
        }

        public IReadOnlyReactiveProperty<TValue> Value => _value = (_value ?? new RecycleReactiveProperty<TValue>());
        
        public ILifeTime LifeTime => _lifeTime = (_lifeTime ?? new LifeTimeDefinition());

        public bool IsActive => _isActive;

        protected virtual async UniTask<TValue> OnExecute(IContext context, ILifeTime executionLifeTime) {
            return default;
        }

        protected virtual async UniTask OnComplete(TValue value,IContext context, ILifeTime lifeTime) { }
        
        protected virtual async UniTask OnExit() { }

        private void Initialize() {
            _lifeTime = (_lifeTime ?? new LifeTimeDefinition());
            _value    = (_value ?? new RecycleReactiveProperty<TValue>());
        }
    }
}