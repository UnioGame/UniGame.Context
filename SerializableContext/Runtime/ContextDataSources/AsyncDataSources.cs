using UniCore.Runtime.ProfilerTools;

namespace UniModules.UniGame.SerializableContext.Runtime.ContextDataSources
{
    using System.Collections.Generic;
    using Context.Runtime.Abstract;
    using Core.Runtime.Interfaces;
    using Core.Runtime.ScriptableObjects;
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.ProfilerTools;
    using UniCore.Runtime.Rx.Extensions;
    using UniModules.UniContextData.Runtime.Interfaces;
    using UniModules.UniGame.AddressableTools.Runtime.Extensions;
    using UniModules.UniGame.SerializableContext.Runtime.Addressables;
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/GameSystem/Sources/AddressablesAsyncSources", fileName = nameof(AsyncDataSources))]
    public class AsyncDataSources : AsyncContextDataSource
    {
        #region inspector

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineEditor()]
#endif
        public List<ScriptableObject> sources = new List<ScriptableObject>();

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.LabelText("Async Sources")]
#endif
        public List<AssetReferenceDataSource> sourceAssets = new List<AssetReferenceDataSource>();
        
        #endregion
        
        public override async UniTask<IContext> RegisterAsync(IContext context)
        {
            foreach (var source in sources) {
                if(!(source is IAsyncContextDataSource asyncSource)) 
                    continue;
                asyncSource.RegisterAsync(context)
                    .Forget();
            }
            
            await UniTask.WhenAll(sourceAssets.Select(x => RegisterContexts(context, x)));

            return context;
        }
        
        private async UniTask<bool> RegisterContexts(IContext target,AssetReferenceDataSource sourceReference)
        {
            GameLog.Log($"RegisterContexts {name} {target.GetType().Name} LIFETIME CONTEXT");

            var sourceName = name;
            var lifetime   = target.LifeTime;
            
            lifetime.AddCleanUpAction(() => 
                GameLog.Log($"RegisterContexts {sourceName} {target.GetType().Name} END LIFETIME CONTEXT"));
            
            var source = await sourceReference.LoadAssetTaskAsync(lifetime);
            if (source == null) {
                GameLog.LogError($"Empty Data source found {sourceReference} GUID {sourceReference.AssetGUID}");
                return false;
            }

            if (source is LifetimeScriptableObject ltSO)
            {
                ltSO.AddTo(LifeTime);
            }

            await source.RegisterAsync(target);
            return true;

        }

        protected void OnDestroy()
        {
            Debug.LogError($"DESTROY {name}");
        }
    }
}
