using UnityEngine;

namespace UniModules.UniGame.Context.Runtime.Context
{
    using global::UniGame.Core.Runtime;
    using Cysharp.Threading.Tasks;
    using global::UniGame.Context.Runtime;

    public abstract class AsyncObjectDataSource : MonoBehaviour, IAsyncContextDataSource
    {

        public abstract UniTask<IContext> RegisterAsync(IContext context);
    }
}
