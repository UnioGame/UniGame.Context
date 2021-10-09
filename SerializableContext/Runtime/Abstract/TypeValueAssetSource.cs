using UniModules.UniGame.SerializableContext.Runtime.Abstract;

namespace UniModules.UniGame.Context.SerializableContext.Runtime.Abstract
{
    using System;
    using Core.Runtime.DataFlow.Interfaces;
    using Core.Runtime.ScriptableObjects;
    using global::UniCore.Runtime.ProfilerTools;
    using UniCore.Runtime.Common;
    using UnityEngine;

    [Serializable]
    public abstract class TypeValueAssetSource<TValue,TApiValue> :
        LifetimeScriptableObject,
        ITypeValueAsset<TValue,TApiValue>
        where TValue : TApiValue
    {
        #region inspector

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.HideLabel]
#endif
        public TValue defaultValue = default(TValue);
        
        #endregion

        private TypeValueAssetSource<TValue, TApiValue> source;

        private ContextValue<TApiValue> contextValue = new ContextValue<TApiValue>();

        protected TValue _activeValue;
        
        public TApiValue Value {

            get {
                if (contextValue.IsValueType == false && contextValue.Value == null) {
                    SetValue(CreateValue());
                }
                return contextValue.Value;
            }
            
        }

        public bool HasValue => contextValue.HasValue;
        
        #region public methods
        
        public IDisposable Subscribe(IObserver<TApiValue> observer) => contextValue.Subscribe(observer);

        public void SetValue(TValue value)
        {
            _activeValue = value;
            ApplyValue(value);
        }
        
        #endregion

        protected sealed override void OnActivate()
        {
            contextValue = new ContextValue<TApiValue>();
            LifeTime.AddDispose(contextValue);
            
            SetValue(CreateValue());
            OnInitialize(LifeTime);
        }


        protected override void OnReset()
        {
            SetValue(CreateValue());
            LifeTime.AddDispose(contextValue);
            LifeTime.AddCleanUpAction(OnResetAction);
        }
        
        /// <summary>
        /// Apply Value to context
        /// </summary>
        /// <param name="value"></param>
        protected virtual void ApplyValue(TApiValue value) => contextValue.SetValue(value);

        /// <summary>
        /// Default context value
        /// </summary>
        /// <returns></returns>
        protected virtual TValue CreateValue() => defaultValue;

        /// <summary>
        /// initialize default value state
        /// </summary>
        protected virtual void OnInitialize(ILifeTime lifetime) {}

        private void OnResetAction()
        {
            if (!Application.isPlaying)
                return;
            GameLog.Log($"TypeValueAsset: {GetType().Name} {name} : VALUE {Value} END OF LIFETIME",Color.red);
        }

    }
}
