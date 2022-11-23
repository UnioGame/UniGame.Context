using UniGame.Core.Runtime.Extension;
using UniGame.Core.Runtime.ScriptableObjects;

namespace UniGame.Context.Runtime.DataSources
{
    using AddressableTools.Runtime;
    using System;
    using Core.Runtime;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/GameFlow/Sources/AddressableContextSource", fileName = nameof(AsyncContextSource))]
    public class AsyncContextSource : LifetimeScriptableObject, 
        IAsyncContextDataSource
    {
        public ContextAssetReference contextAsset;

        public async UniTask<IContext> RegisterAsync(IContext context)
        {
            var contextReference = await contextAsset
                .LoadAssetTaskAsync<ContextAsset>(LifeTime)
                .ToSharedInstanceAsync(LifeTime);
            
            await contextReference.RegisterAsync(context);

            if (contextAsset.Asset is IDisposable disposable)
                LifeTime.AddDispose(disposable);
            
            LifeTime.AddCleanUpAction(contextAsset.ReleaseAsset);
            
            return context;
        }

    }
}
