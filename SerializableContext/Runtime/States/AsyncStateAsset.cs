namespace UniModules.UniGame.Context.SerializableContext.Runtime.States
{
    using System;
    using global::UniGame.Core.Runtime;
    using Cysharp.Threading.Tasks;

    [Serializable]
    public abstract class AsyncStateAsset<TData,TValue> : 
        AsyncCommandAsset<TData,TValue>, 
        IAsyncStateCommand<TData,TValue>,
        IAsyncState<TData,TValue> ,
        IAsyncCompletion<TValue, TData>,
        IAsyncEndPoint<TData>  ,
        IAsyncRollback<TData>  
    {
        private AsyncStateProxyValue<TData, TValue> _asyncStateProxyValue;
        
        
        public bool IsActive => _asyncStateProxyValue.IsActive;

        
        #region public methods

        public sealed override async UniTask<TValue> ExecuteAsync(TData value) => await _asyncStateProxyValue.ExecuteAsync(value);

        public async UniTask ExitAsync() => await _asyncStateProxyValue.ExitAsync();

        public virtual UniTask CompleteAsync(TValue value, TData data, ILifeTime lifeTime) => UniTask.CompletedTask;

        public virtual UniTask ExitAsync(TData data) => UniTask.CompletedTask;

        public virtual UniTask Rollback(TData source) => UniTask.CompletedTask;

        public virtual UniTask<TValue> ExecuteStateAsync(TData value) => UniTask.FromResult<TValue>(default);

        #endregion

        protected override void OnActivate()
        {
            base.OnActivate();
            _asyncStateProxyValue?.ExitAsync();
            
            LifeTime.AddCleanUpAction(() => _asyncStateProxyValue.ExitAsync());
            _asyncStateProxyValue = 
                _asyncStateProxyValue ?? 
                new AsyncStateProxyValue<TData, TValue>(this,this,this,this);
        }

    }
}