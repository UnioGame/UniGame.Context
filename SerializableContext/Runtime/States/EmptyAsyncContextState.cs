namespace UniModules.UniGame.Context.SerializableContext.Runtime.States
{
    using System;
    using Abstract;
    using Core.Runtime.DataFlow.Interfaces;
    using Core.Runtime.Interfaces;
    using Cysharp.Threading.Tasks;

    [Serializable]
    public class EmptyAsyncContextState : IAsyncContextStateStatus
    {
        private readonly AsyncStatus _result;
        
        public EmptyAsyncContextState(AsyncStatus result) => _result = result;

        public UniTask<AsyncStatus> ExecuteAsync(IContext value) => UniTask.FromResult(_result);

        public UniTask ExitAsync() => UniTask.CompletedTask;

        public ILifeTime LifeTime => UniCore.Runtime.DataFlow.LifeTime.TerminatedLifetime;
       
        public bool IsActive => false;
    }
}