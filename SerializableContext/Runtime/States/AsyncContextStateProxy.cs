namespace UniModules.UniGame.Context.SerializableContext.Runtime.States {
    using System;
    using Core.Runtime.DataFlow.Interfaces;
    using Core.Runtime.Interfaces;
    using Cysharp.Threading.Tasks;

    public class AsyncContextStateProxy<TValue> : AsyncContextState<TValue>
    {
        private readonly Func<IContext, ILifeTime, UniTask<TValue>> onExecute;
        private readonly Func<TValue, IContext, ILifeTime, UniTask> onComplete;
        private readonly Func<UniTask>                              onExit;

        public AsyncContextStateProxy(
            Func<IContext, ILifeTime, UniTask<TValue>> onExecute,
            Func<TValue, IContext, ILifeTime, UniTask> onComplete,
            Func<UniTask> onExit) {
            this.onExecute  = onExecute;
            this.onComplete = onComplete;
            this.onExit     = onExit;
        }

        protected override async UniTask OnComplete(TValue value, IContext context, ILifeTime lifeTime) {
            if (onComplete == null)
                return;
            await onComplete(value,context,lifeTime);
        }
        
        protected override async UniTask<TValue> OnExecute(IContext context, ILifeTime lifeTime) {
            if (onExecute == null)
                return default;
            return await onExecute(context,lifeTime);
        }

        protected override async UniTask OnExit() {
            if (onExit == null)
                return;
            await onExit();
        }
    }
    
}
