﻿using UniModules.UniGame.CoreModules.UniGame.AddressableTools.Runtime.Extensions;

namespace UniModules.UniGame.SerializableContext.Runtime.ContextDataSources
{
    using System;
    using Addressables;
    using Context.Runtime.Abstract;
    using Core.Runtime.Interfaces;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/GameSystem/Sources/AddressableContextSource", fileName = nameof(AsyncContextSource))]
    public class AsyncContextSource : AsyncContextDataSource
    {

        public ContextAssetReference contextAsset;

        public override async UniTask<IContext> RegisterAsync(IContext context)
        {
            var contextReference = await contextAsset.LoadAssetTaskAsync(LifeTime);
            await contextReference.RegisterAsync(context);

            if (contextAsset.Asset is IDisposable disposable)
                LifeTime.AddDispose(disposable);
            
            LifeTime.AddCleanUpAction(contextAsset.ReleaseAsset);
            
            return context;
        }

    }
}
