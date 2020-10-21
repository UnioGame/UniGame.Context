using UniCore.Runtime.ProfilerTools;

namespace UniModules.UniGame.SerializableContext.Runtime.Abstract
{
    using Core.Runtime.DataFlow.Interfaces;
    using Core.Runtime.Interfaces;
    using Cysharp.Threading.Tasks;
    using UniCore.Runtime.ProfilerTools;
    using UniModules.UniCore.Runtime.DataFlow;
    using UnityEngine;

    public abstract class AssetValueSource: ScriptableObject,
        IAsyncSource, 
        IAsyncSourcePrototype
    {
        #region inspector

        private LifeTimeDefinition lifeTime;

        #endregion
        
        public ILifeTime LifeTime => lifeTime.LifeTime;

        public abstract UniTask<IContext> RegisterAsync(IContext context);

        public virtual IAsyncSource Create() => Instantiate(this);
        
        protected void OnEnable()
        {
            lifeTime = new LifeTimeDefinition();
        }

        protected void OnDisable()
        {
            //end of value LIFETIME
            lifeTime.Terminate();
            
            if(Application.isPlaying) 
                GameLog.Log($"{nameof(AssetValueSource)}: {GetType().Name} {name} : END OF LIFETIME");
        }
    }
}
