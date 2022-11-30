using UniGame.Context.Runtime;

namespace UniModules.UniGame.SerializableContext.Runtime.Abstract
{
    public interface IAsyncValueSource<TValue> : IAsyncDataSource where TValue : class
    {
        TValue Value { get; }
        
        IAsyncValueSource<TValue> Initialize(ISourceValue<TValue> value, bool createInstance = true);
    }
}