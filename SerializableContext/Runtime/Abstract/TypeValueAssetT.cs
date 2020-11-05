namespace UniModules.UniGame.SerializableContext.Runtime.Abstract
{
    using System;
    using Context.SerializableContext.Runtime.Abstract;

    [Serializable]
    public abstract class TypeValueAsset<TValue> : TypeValueAssetSource<TValue,TValue> {}
    
    public abstract class TypeValueAsset<TValue,TApi> : TypeValueAssetSource<TValue,TApi>
        where TValue : TApi {}
}