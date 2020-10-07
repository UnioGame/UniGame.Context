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
}