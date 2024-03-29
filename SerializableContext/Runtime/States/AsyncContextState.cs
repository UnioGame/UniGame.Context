﻿namespace UniModules.UniGame.Context.SerializableContext.Runtime.States
{
    using System;
    using Abstract;
    using Core.Runtime.AsyncOperations;
    using global::UniGame.Core.Runtime;

    [Serializable]
    public abstract class AsyncContextState : AsyncContextState<AsyncStatus>, IAsyncContextStateStatus
    {
        #region static data

        public static readonly IAsyncContextStateStatus CompleteState = new EmptyAsyncContextState(AsyncStatus.Succeeded);
        public static readonly IAsyncContextStateStatus FailedState   = new EmptyAsyncContextState(AsyncStatus.Faulted);
        public static readonly IAsyncContextStateStatus CanceledState = new EmptyAsyncContextState(AsyncStatus.Canceled);

        #endregion

        protected sealed override AsyncStatus GetInitialExecutionValue() => AsyncStatus.Pending;
    }


    [Serializable]
    public abstract class AsyncContextState<TValue> :
        AsyncState<IContext, TValue>,
        IAsyncContextState<TValue>
    {
    }
}