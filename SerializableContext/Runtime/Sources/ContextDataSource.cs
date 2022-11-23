using UniModules.UniGame.Context.Runtime.Context;

namespace UniGame.Context.Runtime
{
    using UniModules.UniCore.Runtime.Common;
    using UniGame.Runtime.ObjectPool;
    using global::UniGame.Core.Runtime;
    using UniCore.Runtime.ProfilerTools;
    using UniRx;
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/GameFlow/Sources/ContextDataSource", 
        fileName = nameof(ContextDataSource))]
    public class ContextDataSource : ValueContainerDataSource<IContext>
    {
        [SerializeField] 
        private bool _createDefaultOnLoad = false;

        private bool _ownContextLifeTime = true;
        
        private DisposableAction _disposableAction;
        
        protected override void OnActivate()
        {
            base.OnActivate();

            if (_createDefaultOnLoad)
            {
                var context = new EntityContext();
                if (_ownContextLifeTime)
                    context.AddTo(LifeTime);
                SetValue(context);
            }

            this.Do(OnContextUpdated)
                .Subscribe()
                .AddTo(LifeTime);
        }

        private void OnContextUpdated(IContext context)
        {
            if (context == null)
                return;

            _disposableAction?.Complete();
            _disposableAction = ClassPool.Spawn<DisposableAction>();
            _disposableAction.Initialize(() => SetValue(null));
            
            context.LifeTime.AddDispose(_disposableAction);
            
#if UNITY_EDITOR
            context.LifeTime.AddCleanUpAction(() => GameLog
                .Log($"CONTEXT CONTAINER {Name} CONTEXT FINISHED", Color.red));
#endif
            
            GameLog.Log($"CONTEXT CONTAINER {name} CONTEXT VALUE UPDATE {context}", Color.red);
        }
    }
}