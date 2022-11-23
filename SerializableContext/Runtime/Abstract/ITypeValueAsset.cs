namespace UniModules.UniGame.SerializableContext.Runtime.Abstract
{
    using global::UniGame.Core.Runtime;

    public interface ITypeValueAsset<TValue,TApi> : 
        IDataValue<TValue, TApi>,
        ILifeTimeContext
        where TValue : TApi
    {
        
    }
}