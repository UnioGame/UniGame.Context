namespace UniModules.UniGame.SerializableContext.Runtime.AssetTypes
{
    using System;
    using Abstract;
    using Context.Runtime.Abstract;
    using Core.Runtime.Interfaces;
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.Rx;
    using UniRx;
    

    public class TypeContainerAssetSource<TValue> :
        TypeContainerAssetSource<TValue, TValue>
    {
    }

    public class TypeContainerAssetSource<TValue,TApi> : 
        AsyncContextDataSource, 
        IDataValue<TValue,TApi> 
        where TValue : TApi
    {
        #region inspector

        public bool continuousUpdate = true;
        
        #endregion
        
        private RecycleReactiveProperty<TValue> _value;
        
        public TApi Value => _value.Value;

        public bool HasValue => _value != null && _value.HasValue;

        public override async UniTask<IContext> RegisterAsync(IContext context)
        {
            if (continuousUpdate)
            {
                _value.Subscribe(context.Publish)
                    .AddTo(context.LifeTime);
                return context;
            }
            
            await UniTask.WaitWhile(() => HasValue == false);
            context.Publish(Value);
            return context;
        }

        public IDisposable Subscribe(IObserver<TApi> observer) =>
            _value.Subscribe(api => observer.OnNext(api), observer.OnError, observer.OnCompleted);
        
        public void SetValue(TValue value) => _value.Value = value;
      
        #region private methods
        
        protected override void OnActivate()
        {
            base.OnActivate();
            _value = new RecycleReactiveProperty<TValue>();
            LifeTime.AddCleanUpAction(_value.Release);
        }

        protected sealed override void OnReset()
        {
            base.OnReset();
            LifeTime?.AddCleanUpAction(_value.Release);
            ResetValue();
        }

        protected virtual void ResetValue()
        {
            
        }

        #endregion
        
    }
}
