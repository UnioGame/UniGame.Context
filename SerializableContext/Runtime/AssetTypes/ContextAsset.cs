using UniCore.Runtime.ProfilerTools;

namespace UniModules.UniGame.SerializableContext.Runtime.AssetTypes
{
    using Abstract;
    using Core.Runtime.DataFlow.Interfaces;
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniContextData.Runtime.Entities;
    using UniModules.UniContextData.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.Interfaces;
    
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/GameSystem/Assets/Context" , fileName = nameof(ContextAsset))]
    public class ContextAsset : 
        TypeValueDefaultAsset<EntityContext,IContext>, 
        IContextDataSource,
        IAsyncContextDataSource
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
