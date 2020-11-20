namespace UniModules.UniGame.Context.SerializableContext.Runtime.States
{
    using System;
    using Abstract;
    using Core.Runtime.AsyncOperations;
    using Core.Runtime.Interfaces;

    
    [Serializable]
    public abstract class AsyncContextState : AsyncContextState<AsyncStatus>
    {
        protected sealed override AsyncStatus GetInitialExecutionValue() => AsyncStatus.Pending;
    }
    
    [Serializable]
    public abstract class AsyncContextState<TValue> : 
        AsyncState<IContext,TValue>, 
        IAsyncContextState<TValue>
    {
    }
}