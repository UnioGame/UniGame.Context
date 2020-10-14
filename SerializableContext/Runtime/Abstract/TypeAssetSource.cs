namespace UniModules.UniGame.SerializableContext.Runtime.Abstract
{
    using UnityEngine;


    public class TypeAssetSource<TValue,TApiValue> : 
        TypeValueSource<TValue,TApiValue>
        where TValue : Object, TApiValue
        where TApiValue : class
    {

    }
}