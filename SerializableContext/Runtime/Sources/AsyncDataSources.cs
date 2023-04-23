﻿using UniCore.Runtime.ProfilerTools;

namespace UniGame.Context.Runtime.DataSources
{
    using Core.Runtime.Extension;
    using Core.Runtime;
    using Core.Runtime.ScriptableObjects;
    using Runtime;
    using AddressableTools.Runtime;
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [CreateAssetMenu(menuName = "UniGame/GameFlow/Sources/AddressableAsyncSources",
        fileName = nameof(AsyncDataSources))]
    public class AsyncDataSources : LifetimeScriptableObject, IAsyncDataSource
    {
        #region inspector

        public bool enabled = true;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineEditor()] [Sirenix.OdinInspector.Searchable]
#endif
        public List<ScriptableObject> sources = new List<ScriptableObject>();

        [Space]
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.LabelText("Async Sources")]
        [Sirenix.OdinInspector.Searchable]
#endif
        public List<AssetReferenceDataSource> sourceAssets = new List<AssetReferenceDataSource>();

        public bool useTimeout = true;

        public float timeOutMs = 60000;

        #endregion

        public async UniTask<IContext> RegisterAsync(IContext context)
        {
            if (enabled == false)
                return context;

            var syncSources = sources
                .Where(x => x is IAsyncDataSource)
                .Select(x => x.ToSharedInstance())
                .Select(x => (x as IAsyncDataSource))
                .Select(x => RegisterContexts(context,x));

            var asyncSources = sourceAssets
                .Select(x => RegisterContexts(context, x));
            
            await UniTask.WhenAll(syncSources.Concat(asyncSources));

            return context;
        }

        private async UniTask<bool> RegisterContexts(
            IContext target,
            AssetReferenceDataSource sourceReference)
        {
            var sourceName = name;

            GameLog.Log($"RegisterContexts {sourceName} {target.GetType().Name} LIFETIME CONTEXT");

            var source = await sourceReference
                .LoadAssetTaskAsync(target.LifeTime)
                .ToSharedInstanceAsync(target.LifeTime);

            var result = await RegisterContexts(target, source);

            return result;
        }
        
        
        private async UniTask<bool> RegisterContexts(IContext target, IAsyncDataSource source)
        {
            var sourceName = name;

            GameLog.Log($"RegisterContexts {sourceName} {target.GetType().Name} LIFETIME CONTEXT");

            var sourceAsset = source as Object;
            var sourceAssetName = sourceAsset == null
                ? string.Empty
                : sourceAsset.name;

            switch (source)
            {
                case null:
                    GameLog.LogError($"Empty Data source found {sourceName} GUID {sourceAssetName}");
                    return false;
                case LifetimeScriptableObject lifetimeScriptableObject:
                    lifetimeScriptableObject.AddTo(LifeTime);
                    break;
            }

            var cancellationSource = new CancellationTokenSource();

            if (useTimeout && timeOutMs > 0)
            {
                HandleTimeout(sourceAssetName, cancellationSource.Token)
                    .AttachExternalCancellation(cancellationSource.Token)
                    .SuppressCancellationThrow()
                    .Forget();
            }

            await source.RegisterAsync(target)
                .AttachExternalCancellation(LifeTime.CancellationToken);

            cancellationSource.Cancel();
            cancellationSource.Dispose();

            GameLog.Log($"{sourceName} : REGISTER SOURCE {sourceAssetName}", Color.green);

            return true;
        }

        private async UniTask HandleTimeout(string assetName, CancellationToken cancellationToken)
        {
            if (!useTimeout || timeOutMs <= 0)
                return;

            var assetSourceName = name;

            await UniTask.Delay(TimeSpan.FromMilliseconds(timeOutMs), cancellationToken: cancellationToken)
                .AttachExternalCancellation(cancellationToken);

            GameLog.LogError($"{assetSourceName} : REGISTER SOURCE TIMEOUT {assetName}");
        }

        protected void OnDestroy() => Debug.LogError($"DESTROY {name}");
    }
}