using Cysharp.Threading.Tasks;
using UniModules.UniCore.Runtime.Interfaces;
using UniModules.UniGame.Context.SerializableContext.Runtime.Abstract;

namespace UniModules.UniGame.SerializableContext.Runtime.Abstract
{
    using UniModules.UniContextData.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.Interfaces.Rx;
    using UniModules.UniGame.Core.Runtime.Interfaces;

    public interface ISourceValue<TApiValue> : 
        IObservableValue<TApiValue>, 
        IPrototype<ISourceValue<TApiValue>>,
        IAsyncContextDataSource
    {
    }
    
    public interface IAsyncSourceValue<TApiValue> : 
        IObservableValue<TApiValue>, 
        IAsyncContextPrototype<IAsyncSourceValue<TApiValue>>
    {
        UniTask<TApiValue> CreateValue(IContext context);
    }
    
    
}