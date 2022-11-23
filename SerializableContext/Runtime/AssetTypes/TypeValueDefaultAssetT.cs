namespace UniModules.UniGame.SerializableContext.Runtime.Abstract
{
    using System;

    [Serializable]
    public class DefaultValueAsset<TValue> : 
        ClassValueAsset<TValue, TValue> 
        where TValue :class, new(){ }
}