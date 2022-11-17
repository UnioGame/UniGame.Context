using UniCore.Runtime.ProfilerTools;
using UniModules.UniCore.Runtime.Common;
using UniModules.UniCore.Runtime.ObjectPool.Runtime;

namespace UniModules.UniGame.SerializableContext.Runtime.AssetTypes
{
    using Context.Runtime.Context;
    using Core.Runtime.Interfaces;
    using UniContextData.Runtime.Interfaces;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniContextData.Runtime.Entities;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/GameFlow/Sources/ContextDataSource", 
        fileName = nameof(ContextDataSource))]
    public class ContextDataSource : ValueContainerDataSource<IContext>
    {
        [SerializeField] private bool _createDefaultOnLoad = false;

        private DisposableAction _disposableAction;
        
        protected override void OnActivate()
        {
            base.OnActivate();

            if (_createDefaultOnLoad)
                SetValue(new EntityContext());

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

        protected override void ResetValue()
        {
            base.ResetValue();
            _disposableAction?.Complete();
            SetValue(null);
        }
    }
}