using UniModules.UniContextData.Runtime.Interfaces;

namespace UniModules.UniGame.SerializableContext.Runtime.Abstract
{
    public interface IAsyncAssetSourceValue<TValue> : IAsyncContextDataSource where TValue : class
    {
        TValue Value { get; }
        
        IAsyncAssetSourceValue<TValue> Initialize(ISourceValue<TValue> value, bool createInstance = true);
    }
}