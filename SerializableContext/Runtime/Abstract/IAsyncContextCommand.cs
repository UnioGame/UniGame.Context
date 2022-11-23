namespace UniModules.UniGame.Context.SerializableContext.Runtime.Abstract
{
    using global::UniGame.Core.Runtime;

    public interface IAsyncContextCommand<TValue> : IAsyncCommand<IContext,TValue>
    {
        
    }
    
    public interface IAsyncContextCommand : IAsyncCommand<IContext,AsyncStatus>
    {
        
    }
}