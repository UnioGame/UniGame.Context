using UniCore.Runtime.ProfilerTools;

namespace UniGame.Context.Runtime.DataSources
{
    using Core.Runtime.Extension;
    using Core.Runtime;
    using Core.Runtime.ScriptableObjects;
    using Runtime;
    using AddressableTools.Runtime;
    using System;
    using System.Collections.Generic;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using Object = UnityEngine.Object;

    [CreateAssetMenu(menuName = "UniGame/GameFlow/Sources/AddressableAsyncSources",
        fileName = nameof(AsyncDataSources))]
    public class AsyncDataSources : LifetimeScriptableObject, IAsyncContextDataSource
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

            foreach (var source in sources)
            {
                if (source is not IAsyncContextDataSource asyncSource)
                    continue;
                asyncSource.ToSharedInstance(LifeTime)
                    .RegisterAsync(context)
                    .Forget();
            }

            await UniTask.WhenAll(sourceAssets
                .Select(x => RegisterContexts(context, x)));

            return context;
        }

        private async UniTask<bool> RegisterContexts(IContext target,
            AssetReferenceDataSource sourceReference)
        {
            var sourceName = name;

            GameLog.Log($"RegisterContexts {sourceName} {target.GetType().Name} LIFETIME CONTEXT");

            var source = await sourceReference
                .LoadAssetTaskAsync(LifeTime)
                .ToSharedInstanceAsync();

            var sourceAsset = source as Object;
            var sourceAssetName = sourceAsset == null
                ? string.Empty
                : sourceAsset.name;

            switch (source)
            {
                case null:
                    GameLog.LogError($"Empty Data source found {sourceReference} GUID {sourceReference.AssetGUID}");
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
                .AttachExternalCancellation(LifeTime.TokenSource);

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