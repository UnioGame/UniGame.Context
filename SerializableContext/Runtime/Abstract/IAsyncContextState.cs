namespace UniModules.UniGame.Context.SerializableContext.Runtime.Abstract
{
    using Core.Runtime.Interfaces;

    public interface IAsyncContextState<TValue>  : IAsyncState<IContext,TValue> 
    {
        
    }
    
    public interface IAsyncContextStateStatus  : IAsyncState<IContext,AsyncStatus> 
    {
        
    }
    
    public interface IAsyncContextState  : IAsyncState<IContext> 
    {
        
    }

}