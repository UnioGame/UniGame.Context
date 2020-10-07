namespace UniModules.UniGame.SerializableContext.Runtime.Abstract
{
    using Core.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.Interfaces;

    public interface ITypeValueAsset<TValue,TApi> : 
        IDataValue<TValue, TApi>,
        ILifeTimeContext
        where TValue : TApi
    {
    }
}