namespace UniModules.UniGame.SerializableContext.Runtime.Abstract
{
    using System;

    [Serializable]
    public class TypeValueDefaultAsset<TValue> : 
        TypeValueDefaultAsset<TValue, TValue> 
        where TValue :class, new(){ }
}