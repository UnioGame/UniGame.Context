namespace UniModules.UniGame.Context.SerializableContext.Runtime.States
{
    using System;
    using Abstract;
    using Core.Runtime.Interfaces;
    using Cysharp.Threading.Tasks;

    [Serializable]
    public class AsyncContextVariantCommand : 
        IAsyncContextCommand,
        IAsyncRollback<IContext>,
        IAsyncEndPoint<IContext>,
        IAsyncEndPoint
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineProperty]
        [Sirenix.OdinInspector.HideLabel]
#endif
        public AsyncContextCommandValue variant = new AsyncContextCommandValue();

        public async UniTask<AsyncStatus> Execute(IContext value)
        {
            if (variant.HasValue == false)
                return AsyncStatus.Succeeded;
            return await variant.Value.Execute(value);
        }

        public async UniTask Rollback(IContext source)
        {
            if (variant.HasValue == false)
                return;
            var value = variant.Value;
            if (value is IAsyncRollback<IContext> rollback)
                await rollback.Rollback(source);
        }

        public async UniTask Exit(IContext data)
        {
            if (variant.HasValue == false)
                return;
            var value = variant.Value;
            switch (value)
            {
                case IAsyncEndPoint<IContext> dataEndPoint:
                    await dataEndPoint.Exit(data);
                    break;
                case IAsyncEndPoint endPoint:
                    await endPoint.Exit();
                    break;
            }
        }

        public async UniTask Exit()
        {
            if (variant.HasValue == false)
                return;
            var value = variant.Value;
            if (value is IAsyncEndPoint endPoint)
                await endPoint.Exit();
        }
    }
}