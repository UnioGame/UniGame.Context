namespace UniGame.Context.Runtime
{
    using UniCore.Runtime.ProfilerTools;
    using AddressableTools.Runtime;
    using UniGame.Core.Runtime.ScriptableObjects;
    using System;
    using global::UniGame.Core.Runtime;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class SingleAssetReferenceMonoBehaviourSource<TObject, TApi> 
        : LifetimeScriptableObject, IAsyncDataSource
        where TObject : MonoBehaviour, TApi
    {
        [SerializeField] 
        private AssetReferenceGameObject _prefabReference;

        private static TObject _instance;

        public TObject Asset => _instance;

        public async UniTask<IContext> RegisterAsync(IContext context)
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

                
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        protected static void OnCleanUp()
        {
            _instance = null;
        }
#endif
    }
}