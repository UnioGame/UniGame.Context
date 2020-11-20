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

        public sealed override async UniTask<TValue> ExecuteAsync(TData value) => await _asyncStateProxyValue.ExecuteAsync(value);

        public async UniTask ExitAsync() => await _asyncStateProxyValue.ExitAsync();

        public bool IsActive => _asyncStateProxyValue.IsActive;
        
        #endregion

        #region IAsyncCompletion

        public virtual async UniTask CompleteAsync(TValue value, TData data, ILifeTime lifeTime) { }
        
        #endregion

        public async virtual UniTask ExitAsync(TData data) { }

        public async virtual UniTask Rollback(TData source) { }
        
        protected override void OnActivate()
        {
            base.OnActivate();
            _asyncStateProxyValue?.ExitAsync();
            
            LifeTime.AddCleanUpAction(() => _asyncStateProxyValue.ExitAsync());
            _asyncStateProxyValue = _asyncStateProxyValue ?? new AsyncStateProxyValue<TData, TValue>(this,this,this);
        }

    }
}