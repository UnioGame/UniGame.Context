
namespace UniGame.Context.Runtime
{
    using UniGame.Core.Runtime.ScriptableObjects;
    using global::UniGame.Core.Runtime;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public class SingleMonoBehaviourSource<TObject> : SingleMonoBehaviourSource<TObject,TObject>
        where TObject : MonoBehaviour
    {
    }

    public class SingleMonoBehaviourSource<TObject, TApi> : LifetimeScriptableObject, IAsyncDataSource
        where TObject : MonoBehaviour, TApi 
        where TApi : class
    {
        [SerializeField] 
        private TObject _prefab;
        [SerializeField]
        private bool _dontDestroy = false;
        [SerializeField]
        private bool _ownGameObjectLifeTime = true;

        #region static data

        private static TObject _instance;

        #endregion
        
        public TObject Asset => _instance;

        public async UniTask<IContext> RegisterAsync(IContext context)
        {
            if (_instance == null || _instance.gameObject == null)
            {
                var go = Instantiate(_prefab.gameObject);
                
                if(_ownGameObjectLifeTime)
                    go.DestroyWith(LifeTime);
                if (_dontDestroy)
                    DontDestroyOnLoad(_instance.gameObject);
                
                _instance = go.GetComponent<TObject>();
            }

            var instanceAsset = await OnInstanceReceive(_instance,context);
            
            context.Publish<TApi>(instanceAsset);

            return context;
        }

        protected virtual UniTask<TApi> OnInstanceReceive(TObject asset,IContext context)
        {
            return UniTask.FromResult<TApi>(asset);
        }
        
#if UNITY_EDITOR
        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
        protected static void OnCleanUp()
        {
            _instance = null;
        }
#endif
    }
}