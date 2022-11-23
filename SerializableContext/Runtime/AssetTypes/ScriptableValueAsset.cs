namespace UniModules.UniGame.Context.SerializableContext.Runtime.Abstract
{
    using global::UniCore.Runtime.Attributes;
    using UnityEngine;

    public abstract class ScriptableValueAsset<TValue> :
        ScriptableValueAsset<TValue, TValue> where TValue 
        : ScriptableValueAsset<TValue, TValue>
    {
        
    }

    public abstract class ScriptableValueAsset<TValue,TApi> : 
        AbstractValueAsset<TValue,TApi> 
        where TValue : ScriptableValueAsset<TValue,TApi>, TApi
    {
        
        [ReadOnlyValue] 
        private bool isInstance = false;

        protected override TValue CreateValue()
        {
            if (isInstance) return this as TValue;
            var value = ScriptableObject.Instantiate(this) as TValue;
            value.MarkAsInstance();
            return value;
        }

        protected void MarkAsInstance() => isInstance = true;

    }
}
