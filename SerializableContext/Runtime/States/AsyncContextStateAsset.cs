﻿namespace UniModules.UniGame.Context.SerializableContext.Runtime.States
{
    using System;
    using global::UniGame.Core.Runtime;

    
    
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