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
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using Debug = UnityEngine.Debug;
    using Object = UnityEngine.Object;

#if ODIN_INSPECTOR
    using Sirenix.OdinInspector;
#endif
    
#if UNITY_EDITOR
    using UniModules.Editor;
#endif
    
    [CreateAssetMenu(menuName = "UniGame/GameFlow/Sources/AddressableAsyncSources",
        fileName = nameof(AsyncDataSources))]
    public class AsyncDataSources : LifetimeScriptableObject, IAsyncDataSource
    {
        #region inspector

        public bool enabled = true;

#if ODIN_INSPECTOR
        [InlineEditor()] 
        [Searchable]
#endif
        public List<ScriptableObject> sources = new List<ScriptableObject>();
        
        [Space]
#if ODIN_INSPECTOR
        [LabelText("Async Sources")]
        [Searchable]
        [ListDrawerSettings(ListElementLabelName = "Name")]
#endif
        public List<AsyncSourceDescription> asyncSources = new List<AsyncSourceDescription>();

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

            var asyncValues = asyncSources
                .Select(x => RegisterContexts(context, x));
            
            await UniTask.WhenAll(syncSources.Concat(asyncValues));

            return context;
        }

        private async UniTask<bool> RegisterContexts(
            IContext target,
            AsyncSourceDescription sourceReference)
        {
            if(sourceReference.enabled == false)
                return true;
            
            var sourceName = name;
            var sourceValue = sourceReference.source;

            GameLog.Log($"SOURCE: RegisterContexts {sourceName} {target.GetType().Name} LIFETIME CONTEXT");

            var source = await sourceValue
                .LoadAssetTaskAsync(target.LifeTime)
                .ToSharedInstanceAsync(target.LifeTime);

            if (source is not IAsyncDataSource asyncSource) return false;

            var isAwaitLoading = sourceReference.awaitLoading;
            var registerTask = RegisterContexts(target, asyncSource);

            if (!isAwaitLoading)
            {
                registerTask
                    .AttachExternalCancellation(target.LifeTime.CancellationToken)
                    .Forget();

                return true;
            }

            var result = await registerTask;
            return result;
        }
        
        
        private async UniTask<bool> RegisterContexts(IContext target, IAsyncDataSource source)
        {
            var sourceName = name;

            GameLog.Log($"SOURCE: RegisterContexts {sourceName} {target.GetType().Name} LIFETIME CONTEXT");

            var sourceAsset = source as Object;
            var sourceAssetName = sourceAsset == null
                ? source.GetType().Namespace
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

            var cancellationTokenSource = new CancellationTokenSource();
            
#if DEBUG
            var timer = Stopwatch.StartNew();   
            timer.Restart();
#endif

            if (useTimeout && timeOutMs > 0)
            {
                HandleTimeout(sourceAssetName, cancellationTokenSource.Token)
                    .AttachExternalCancellation(cancellationTokenSource.Token)
                    .SuppressCancellationThrow()
                    .Forget();
            }

            await source.RegisterAsync(target)
                .AttachExternalCancellation(LifeTime.CancellationToken);

#if DEBUG
            var elapsed = timer.ElapsedMilliseconds;
            timer.Stop();
            GameLog.LogRuntime($"SOURCE: LOAD TIME {sourceAssetName} = {elapsed} ms");
#endif
            
            cancellationTokenSource.Cancel();
            cancellationTokenSource.Dispose();
            
            GameLog.LogRuntime($"SOURCE: {sourceName} : REGISTER SOURCE {sourceAssetName}", Color.green);
            
            return true;
        }

        private async UniTask HandleTimeout(string assetName, CancellationToken cancellationToken)
        {
            if (!useTimeout || timeOutMs <= 0)
                return;

            var assetSourceName = name;

            await UniTask.Delay(TimeSpan.FromMilliseconds(timeOutMs), cancellationToken: cancellationToken)
                .AttachExternalCancellation(cancellationToken);

            GameLog.LogError($"SOURCE: {assetSourceName} : REGISTER SOURCE TIMEOUT {assetName}");
        }

        protected void OnDestroy() => Debug.LogError($"DESTROY {name}");
        
    }

    [Serializable]
    public class AsyncSourceDescription : ISearchFilterable
    {
        public bool enabled = true;
        public bool awaitLoading = true;
        
        [DrawWithUnity]
        public AssetReferenceDataSource source;
        
        public string Name {

            get
            {
#if UNITY_EDITOR
                return source == null || source.editorAsset == null 
                    ? string.Empty
                    : source.editorAsset.name;
#endif
                return string.Empty;
            }
            
        }

        public bool IsMatch(string searchString)
        {
            if (string.IsNullOrEmpty(searchString)) return true;
            return Name.IndexOf(searchString, StringComparison.OrdinalIgnoreCase) >= 0;
        }
    }
}