namespace UniModules.UniGame.SerializableContext.Runtime.Abstract
{
    using System;

    [Serializable]
    public class DefaultValueAsset<TValue> : 
        DefaultValueAsset<TValue, TValue> 
        where TValue :class, new(){ }
}