using UnityEngine;

namespace UniModules.UniGame.SerializableContext.Runtime.Abstract
{
    using System;
    using Context.SerializableContext.Runtime.Abstract;

    /// <summary>
    /// value Source with default constructor
    /// </summary>
    [Serializable]
    public class DefaultValueAsset<TValue, TApiValue> : AbstractValueAsset<TValue, TApiValue>
        where TValue :class, TApiValue, new()
    {
        #region inspector

        [SerializeField]
        public bool alwaysCreateNewValue = false;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.HideIf(nameof(alwaysCreateNewValue))]
#endif
        [SerializeField]
        public TValue defaultValue = default(TValue);
        
        #endregion
        
        protected sealed override TValue CreateValue()
        {
            if (alwaysCreateNewValue) return new TValue();
            return defaultValue ?? new TValue();
        }
    }

}