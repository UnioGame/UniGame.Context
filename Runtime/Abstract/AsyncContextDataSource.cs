namespace UniModules.UniGame.Context.Runtime.Abstract
{
    using Core.Runtime.Interfaces;
    using Core.Runtime.ScriptableObjects;
    using Cysharp.Threading.Tasks;
    using UniModules.UniContextData.Runtime.Interfaces;

    public abstract class AsyncContextDataSource : 
        LifetimeScriptableObject, 
        IAsyncContextDataSource

    {

        public abstract UniTask<IContext> RegisterAsync(IContext context);

        
    }
}
