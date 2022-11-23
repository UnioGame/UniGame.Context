using System;
using UniModules.UniGame.Context.SerializableContext.Runtime.Abstract;
using UnityEngine;

namespace Modules.UniModules.UniGame.CoreModules.UniGame.Context.SerializableContext.Runtime.AssetTypes
{
    [Serializable]
    public class ValueAsset<TValue> : ValueAsset<TValue, TValue>
    {
        
    }

    [Serializable]
    public class ValueAsset<TValue,TApi> : AbstractValueAsset<TValue,TApi>
        where TValue : TApi
    {
        
        #region inspector

        [Tooltip("if value empty it will be create on first call")]
        public bool useDefaultValue = true;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.HideLabel]
        [Sirenix.OdinInspector.ShowIf(nameof(useDefaultValue))]
#endif
        public TValue defaultValue = default(TValue);

        #endregion
        
        
        protected sealed override TValue CreateValue()
        {
            return defaultValue;
        }
    }

}