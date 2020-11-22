namespace UniModules.UniGame.Context.SerializableContext.Runtime.States
{
    using System;
    using Abstract;
    using Core.Runtime.Interfaces;

    [Serializable]
    public class AsyncContextStateProxy : AsyncStateProxyValue<IContext, AsyncStatus>, IAsyncContextState
    {

        public AsyncContextStateProxy(
            IAsyncStateCommand<IContext, AsyncStatus> command = null,
            IAsyncCompletion<AsyncStatus, IContext> onComplete = null,
            IAsyncEndPoint<IContext> endPoint = null,
            IAsyncRollback<IContext> onRollback = null) : base(command, onComplete, endPoint, onRollback)
        {
        }

        protected sealed override AsyncStatus GetInitialExecutionValue() => AsyncStatus.Pending;
    }

}