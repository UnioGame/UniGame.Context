namespace UniModules.UniGame.Context.SerializableContext.Runtime.States
{
    using Core.Runtime.Interfaces;
    using Core.Runtime.ScriptableObjects;
    using Cysharp.Threading.Tasks;

    public abstract class AsyncCommandAsset<TData, TValue> :
        LifetimeScriptableObject, 
        IAsyncCommand<TData,TValue>
    {
        public abstract UniTask<TValue> Execute(TData value);
    }
}