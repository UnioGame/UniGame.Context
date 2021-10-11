using UniCore.Runtime.ProfilerTools;
using UniModules.UniGame.Core.Runtime.Extension;

namespace UniModules.UniGame.SerializableContext.Runtime.Components
{
    using System;
    using Context.Runtime.Abstract;
    using Core.Runtime.Interfaces;
    using Cysharp.Threading.Tasks;
    using UniModules.UniGame.AddressableTools.Runtime.Extensions;
    
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class SingleAssetReferenceMonoBehaviourSource<TObject, TApi> : AsyncContextDataSource
        where TObject : MonoBehaviour, TApi
    {
        [SerializeField] 
        private AssetReferenceGameObject _prefabReference;

        private static TObject _instance;

        public TObject Asset => _instance;

        public sealed override async UniTask<IContext> RegisterAsync(IContext context)
        {
            GameLog.Log($"{GetType().Name} TRY REGISTER {typeof(TApi).Name}",Color.blue);

            context.LifeTime.AddCleanUpAction(() => GameLog.Log($"Context {context.GetType().Name} {GetType().Name} LIFETIME FINISHED",Color.red));
            
            var asset = await _prefabReference
                .LoadGameObjectAssetTaskAsync<TObject>(context.LifeTime)
                .AttachExternalCancellation(LifeTime.TokenSource);
            
            lock (this) {
                if (!_instance) {
                    if (!asset) {
                        GameLog.LogError($"{GetType().Name} NULL resource load from {_prefabReference}");
                        return context;
                    }
                    _instance = Instantiate(asset).GetComponent<TObject>();
                    _instance.gameObject.DestroyWith(LifeTime);
                }
            }
            
            if (!_instance) {
                GameLog.LogError($"{GetType().Name} NULL TObject {nameof(TObject)} load from {_prefabReference}");
                return context;
            }

            var targetAsset = await OnInstanceReceive(_instance,context)
                .AttachExternalCancellation(LifeTime.TokenSource);
            
            context.Publish<TApi>(targetAsset);

            OnInstanceRegistered(_instance, context);

            return context;
        }

        protected virtual UniTask<TApi> OnInstanceReceive(TObject asset,IContext context) => UniTask.FromResult<TApi>(asset);
        
        protected virtual void OnInstanceRegistered(TObject asset, IContext context) { }

    }
}