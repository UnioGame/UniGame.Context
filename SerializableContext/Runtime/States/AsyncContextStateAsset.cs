namespace UniModules.UniGame.Context.SerializableContext.Runtime.States
{
    using System;
    using Core.Runtime.Interfaces;

    
    
    [Serializable]
    public abstract class AsyncContextStateAsset : 
        AsyncContextStateAsset<AsyncStatus>
    {
        
    }
    
    [Serializable]
    public abstract class AsyncContextStateAsset<TValue> : 
        AsyncStateAsset<IContext, TValue>
    {
        
    }
    
}