﻿using UniCore.Runtime.ProfilerTools;

namespace UniModules.UniGame.SerializableContext.Runtime.AssetTypes
{
    using Context.Runtime.Context;
    using Core.Runtime.Interfaces;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniContextData.Runtime.Entities;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;
    using UnityEngine;

    [CreateAssetMenu(menuName = "UniGame/GameSystem/Assets/ContextContainerAsset", fileName = nameof(ContextContainerAsset))]
    public class ContextContainerAsset :
        TypeContainerAssetSource<EntityContext, IContext>
    {
        [SerializeField] private bool _createDefaultOnLoad = false;

        protected override void OnActivate()
        {
            base.OnActivate();

            if (_createDefaultOnLoad)
            {
                SetValue(new EntityContext());
            }

            this.Do(OnContextUpdated).Subscribe().AddTo(LifeTime);
        }

        private void OnContextUpdated(IContext context)
        {
            if (context == null)
                return;

            context.LifeTime.AddCleanUpAction(() => SetValue(null));

            context.LifeTime.AddCleanUpAction(
                () => GameLog.Log($"CONTEXT CONTAINER{name} CONTEXT FINISHED", Color.red));

            GameLog.Log($"CONTEXT CONTAINER {name} CONTEXT VALUE UPDATE {context}", Color.red);
        }
    }
}