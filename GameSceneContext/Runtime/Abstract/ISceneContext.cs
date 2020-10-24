namespace UniModules.UniGame.Context.GameSceneContext.Runtime.Abstract {
    using System;
    using Core.Runtime.Interfaces;

    public interface ISceneContext : IDisposable, ILifeTimeContext {
        /// <summary>
        /// always return context for current active scene
        /// </summary>
        IReadOnlyContext Active { get; }

        IReadOnlyContext Get(int sceneHandle);
        
        void     Release(int sceneHandle);

        SceneContextHandle GetSceneHandle(int sceneHandle);
    }
}