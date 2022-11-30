namespace UniModules.UniGame.Context.SerializableContext.Runtime.Abstract
{
    using UniModules.UniGame.SerializableContext.Runtime.Abstract;
    using System;
    using global::UniGame.Core.Runtime;
    using global::UniGame.Core.Runtime.ScriptableObjects;
    using UniCore.Runtime.Common;

    [Serializable]
    public abstract class AbstractValueAsset<TValue,TApiValue> :
        LifetimeScriptableObject,
        ITypeValueAsset<TValue,TApiValue>
        where TValue : TApiValue
    {
        private ReactiveValue<TApiValue> _reactiveValue = new ReactiveValue<TApiValue>();
        
        public TApiValue Value {

            get {
                if (_reactiveValue.IsValueType == false && _reactiveValue.Value == null)
                    SetValue(CreateValue());
                return _reactiveValue.Value;
            }
        }

        public bool HasValue => _reactiveValue.HasValue;
        
        #region public methods
        
        public IDisposable Subscribe(IObserver<TApiValue> observer) => _reactiveValue.Subscribe(observer);

        public void SetValue(TValue value) => ApplyValue(value);

        #endregion

        protected sealed override void OnActivate()
        {
            _reactiveValue = new ReactiveValue<TApiValue>();
            LifeTime.AddDispose(_reactiveValue);
            
            SetValue(CreateValue());
            OnInitialize(LifeTime);
        }

        /// <summary>
        /// Apply Value to context
        /// </summary>
        /// <param name="value"></param>
        protected virtual void ApplyValue(TApiValue value) => _reactiveValue.SetValue(value);

        /// <summary>
        /// Default context value
        /// </summary>
        /// <returns></returns>
        protected abstract TValue CreateValue();

        /// <summary>
        /// initialize default value state
        /// </summary>
        protected virtual void OnInitialize(ILifeTime lifetime) {}
    }
    
    
    
}
