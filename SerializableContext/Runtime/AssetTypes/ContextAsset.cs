using UniCore.Runtime.ProfilerTools;
using UniModules.UniGame.Context.Runtime.Context;
using UniModules.UniGame.SerializableContext.Runtime.Abstract;

namespace UniGame.Context.Runtime
{
    using global::UniGame.Core.Runtime;
    using Cysharp.Threading.Tasks;
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/GameFlow/Data/ContextAsset" , fileName = nameof(ContextAsset))]
    public class ContextAsset : 
        ClassValueAsset<EntityContext,IContext>, 
        IContextDataSource,
        IAsyncDataSource
    {
        public virtual void Register(IContext context)
        {
            context.Publish(Value);
        }

        public virtual async UniTask<IContext> RegisterAsync(IContext context)
        {
            context.Publish(Value);
            return context;
        }

        protected override void OnInitialize(ILifeTime lifeTime)
        {
#if UNITY_EDITOR
            lifeTime.AddCleanUpAction(() => GameLog.Log($"{nameof(ContextAsset)} {defaultValue?.GetType().Name} DISPOSE"));
#endif
            lifeTime.AddDispose(defaultValue);
        }

    }
}
