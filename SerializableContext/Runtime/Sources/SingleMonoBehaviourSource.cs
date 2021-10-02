using UniModules.UniGame.Core.Runtime.Extension;

namespace UniModules.UniGame.SerializableContext.Runtime.Components
{
    using System;
    using Context.Runtime.Abstract;
    using Core.Runtime.Interfaces;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    public class SingleMonoBehaviourSource<TObject> : SingleMonoBehaviourSource<TObject,TObject>
        where TObject : MonoBehaviour
    {
    }

    public class SingleMonoBehaviourSource<TObject, TApi> : AsyncContextDataSource
        where TObject : MonoBehaviour, TApi 
        where TApi : class
    {
        [SerializeField] 
        private TObject _prefab;
        [SerializeField]
        private bool _dontDestroy = false;

        private static TObject _instance;

        public TObject Asset => _instance;

        public override async UniTask<IContext> RegisterAsync(IContext context)
        {
            if (_instance == null || _instance.gameObject == null)
            {
                var go = Instantiate(_prefab.gameObject).DestroyWith(LifeTime);
                _instance = go.GetComponent<TObject>();
                if (_dontDestroy)
                {
                    DontDestroyOnLoad(_instance);
                }
            }

            var instanceAsset = await OnInstanceReceive(_instance,context);
            
            context.Publish<TApi>(instanceAsset);

            return context;
        }

        protected override void OnReset()
        {
            base.OnReset();
            if (_instance != null && _instance.gameObject != null)
            {
                (_instance as IDisposable)?.Dispose();
                GameObject.Destroy(_instance.gameObject);
            }
            _instance = null;

        }

        protected virtual async UniTask<TApi> OnInstanceReceive(TObject asset,IContext context)
        {
            return asset;
        }
    }
}