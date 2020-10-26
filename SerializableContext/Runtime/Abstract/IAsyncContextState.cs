namespace UniModules.UniGame.Context.SerializableContext.Runtime.Abstract
{
    using Core.Runtime.Interfaces;

    public interface IAsyncContextState<TValue>  : IAsyncState<TValue,IContext> 
    {
        
    }
    
    public interface IAsyncContextState  : IAsyncState<IContext> 
    {
        
    }
}