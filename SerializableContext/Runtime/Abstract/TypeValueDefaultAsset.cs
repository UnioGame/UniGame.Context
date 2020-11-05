namespace UniModules.UniGame.SerializableContext.Runtime.Abstract
{
    using System;
    using Context.SerializableContext.Runtime.Abstract;

    [Serializable]
    public class TypeValueDefaultAsset<TValue, TApiValue> :
        TypeValueAssetSource<TValue, TApiValue>
        where TValue :class, TApiValue, new()
    {
        protected override TValue CreateValue() {
            return defaultValue ?? new TValue();
        }
    }

    
    [Serializable]
    public abstract class TypeValueDefaultSource<TValue, TApiValue> :
        TypeValueSource<TValue,TApiValue>
        where TValue :class, TApiValue, new() where TApiValue : class
    {
        protected override TValue CreateValue() {
            return defaultValue ?? new TValue();
        }
    }
    
    [Serializable]
    public abstract class TypeValueDefaultSource<TValue> :
        TypeValueDefaultSource<TValue, TValue> 
        where TValue : class, new()
    {

    }
}