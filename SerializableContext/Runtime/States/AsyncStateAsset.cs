namespace UniModules.UniGame.Context.SerializableContext.Runtime.States
{
    using System;
    using Core.Runtime.DataFlow.Interfaces;
    using Core.Runtime.Interfaces;
    using Cysharp.Threading.Tasks;

    [Serializable]
    public abstract class AsyncStateAsset<TData,TValue> : 
        AsyncCommandAsset<TData,TValue>, 
        IAsyncState<TData,TValue> ,
        IAsyncCompletion<TValue, TData>,
        IAsyncEndPoint<TData>  ,
        IAsyncRollback<TData>  
    {
        private AsyncStateProxyValue<TData, TValue> _asyncStateProxyValue;
        
        #region IAsyncContextState

        public sealed override async UniTask<TValue> Execute(TData value) => await _asyncStateProxyValue.Execute(value);

        public async UniTask Exit() => await _asyncStateProxyValue.Exit();

        public bool IsActive => _asyncStateProxyValue.IsActive;
        
        #endregion

        #region IAsyncCompletion

        public virtual async UniTask Complete(TValue value, TData data, ILifeTime lifeTime) { }
        
        #endregion

        public async virtual UniTask Exit(TData data) { }

        public async virtual UniTask Rollback(TData source) { }
        
        protected override void OnActivate()
        {
            base.OnActivate();
            _asyncStateProxyValue?.Exit();
            
            LifeTime.AddCleanUpAction(() => _asyncStateProxyValue.Exit());
            _asyncStateProxyValue = _asyncStateProxyValue ?? new AsyncStateProxyValue<TData, TValue>(this,this,this);
        }

    }
}