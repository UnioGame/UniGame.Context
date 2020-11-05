namespace UniModules.UniGame.Context.SerializableContext.Runtime.Abstract
{
    using Core.Runtime.Interfaces;

    public interface IAsyncContextCommand<TValue> : IAsyncCommand<IContext,TValue>
    {
        
    }
    
    public interface IAsyncContextCommand : IAsyncCommand<IContext,AsyncStatus>
    {
        
    }
}