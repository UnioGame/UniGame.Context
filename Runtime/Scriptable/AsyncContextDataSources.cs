namespace UniModules.UniGame.SerializableContext.Runtime.Scriptable
{
    using System.Collections.Generic;
    using Core.Runtime.Interfaces;
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.ObjectPool.Runtime;
    using UniCore.Runtime.ObjectPool.Runtime.Extensions;
    using UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Context.Runtime.Abstract;
    using UniRx;
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/GameFlow/Sources/AsyncSources", fileName = nameof(AsyncContextDataSources))]
    public class AsyncContextDataSources : 
        AsyncContextDataSource
    {
        [SerializeReference]
        public List<AsyncContextDataSource> sources = new List<AsyncContextDataSource>();

        protected override void OnActivate()
        {
            base.OnActivate();
            OnReset();
        }

        public override async UniTask<IContext> RegisterAsync(IContext context)
        {
            await UniTask.WhenAll(sources.Select(x => x.RegisterAsync(context).AttachExternalCancellation(LifeTime.TokenSource)));
            
            return context;
        }

        protected override void OnReset()
        {
            base.OnReset();
            foreach (var source in sources)
            {
                source.AddTo(LifeTime);
            }
        }
    }
}
