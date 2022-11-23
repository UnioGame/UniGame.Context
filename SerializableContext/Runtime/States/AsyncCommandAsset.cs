namespace UniModules.UniGame.Context.SerializableContext.Runtime.States
{
    using global::UniGame.Core.Runtime;
    using global::UniGame.Core.Runtime.ScriptableObjects;
    using Cysharp.Threading.Tasks;

    public abstract class AsyncCommandAsset<TData, TValue> :
        LifetimeScriptableObject, 
        IAsyncCommand<TData,TValue>
    {
        public abstract UniTask<TValue> ExecuteAsync(TData value);
    }
}