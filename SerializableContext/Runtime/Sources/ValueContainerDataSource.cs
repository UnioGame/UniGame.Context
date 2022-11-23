namespace UniGame.Context.Runtime
{
    using UniGame.Core.Runtime.ScriptableObjects;
    using UnityEngine;
    using System;
    using global::UniGame.Core.Runtime;
    using Cysharp.Threading.Tasks;
    using UniModules.UniGame.Core.Runtime.Rx;
    using UniRx;
    
    public class ValueContainerDataSource<TValue> : 
        ValueContainerDataSource<TValue, TValue> { }

    public class ValueContainerDataSource<TValue,TApi> : 
        LifetimeScriptableObject,
        IDataValue<TValue,TApi> ,
        IAsyncContextDataSource
        where TValue : TApi
    {
        #region inspector

        [Tooltip("if true - publish each value update into registered context")]
        public bool bindToValueUpdate = true;
        
        #endregion
        
        private RecycleReactiveProperty<TValue> _value = new RecycleReactiveProperty<TValue>();
        
        public TApi Value => _value.Value;

        public bool HasValue => _value is { HasValue: true};

        public UniTask<IContext> RegisterAsync(IContext context)
        {
            var stream = bindToValueUpdate
                ? _value
                : _value.First();
            
            stream.Subscribe(context.Publish)
                .AddTo(context.LifeTime);
            
            return UniTask.FromResult(context);
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

        #endregion
        
    }
}
