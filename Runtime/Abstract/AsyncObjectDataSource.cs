using UnityEngine;

namespace UniModules.UniGame.Context.Runtime.Context
{
    using Core.Runtime.Interfaces;
    using Cysharp.Threading.Tasks;
    using UniContextData.Runtime.Interfaces;

    public abstract class AsyncObjectDataSource : MonoBehaviour, IAsyncContextDataSource
    {

        public abstract UniTask<IContext> RegisterAsync(IContext context);
    }
}
