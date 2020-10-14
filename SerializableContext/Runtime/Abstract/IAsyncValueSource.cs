using UniModules.UniContextData.Runtime.Interfaces;

namespace UniModules.UniGame.SerializableContext.Runtime.Abstract
{
    public interface IAsyncValueSource<TValue> : IAsyncContextDataSource where TValue : class
    {
        TValue Value { get; }
        
        IAsyncValueSource<TValue> Initialize(ISourceValue<TValue> value, bool createInstance = true);
    }
}