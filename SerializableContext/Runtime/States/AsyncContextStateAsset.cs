namespace UniModules.UniGame.Context.SerializableContext.Runtime.States
{
    using System;
    using Abstract;
    using Core.Runtime.DataFlow.Interfaces;
    using Core.Runtime.Interfaces;
    using Core.Runtime.Rx;
    using Core.Runtime.ScriptableObjects;
    using Cysharp.Threading.Tasks;
    using UniRx;

    [Serializable]
    public abstract class AsyncContextStateAsset<TValue> : 
        LifetimeScriptableObject, 
        IAsyncContextState<TValue>
    {
        private bool                            _isActive;
        private UniTask<TValue>                 _taskHandle;
        private RecycleReactiveProperty<TValue> _value;
        

        public async UniTask<TValue> Execute(IContext value) {
            //state already active
            if (_isActive)
                return await _taskHandle;

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

            Reset();
            
        }

        public IReadOnlyReactiveProperty<TValue> Value => _value = (_value ?? new RecycleReactiveProperty<TValue>());

        public bool IsActive => _isActive;

        protected abstract UniTask<TValue> OnExecute(IContext context, ILifeTime executionLifeTime);

        protected virtual async UniTask OnComplete(TValue value,IContext context, ILifeTime lifeTime) { }
        
        protected virtual async UniTask OnExit() { }

        protected override void OnActivate() {
            base.OnActivate();
            _value = (_value ?? new RecycleReactiveProperty<TValue>());
        }
    }
}