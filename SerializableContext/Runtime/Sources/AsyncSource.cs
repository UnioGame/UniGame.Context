using Cysharp.Threading.Tasks;
using UniGame.Core.Runtime;
using UniGame.Core.Runtime.ScriptableObjects;

namespace UniGame.Context.Runtime
{
    public abstract class AsyncSource : LifetimeScriptableObject, IAsyncContextDataSource
    {
        public abstract UniTask<IContext> RegisterAsync(IContext context);
    }
}