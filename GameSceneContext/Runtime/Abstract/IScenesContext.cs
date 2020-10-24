namespace UniModules.UniGame.Context.GameSceneContext.Runtime.Abstract {
    using System;
    using System.Collections.Generic;
    using UniRx;

    public interface IScenesContext
    {
        IEnumerable<IReadOnlySceneContext> SceneContexts { get; }

        /// <summary>
        /// always return context for current active scene
        /// </summary>
        IReadOnlySceneContext Active { get; }

        /// <summary>
        /// reactive active context
        /// </summary>
        IReadOnlyReactiveProperty<IReadOnlySceneContext> ActiveContext { get; }

        /// <summary>
        /// context changes thread
        /// </summary>
        IObservable<IReadOnlySceneContext> ContextChanges { get; }

        
        /// <summary>
        /// Get Scene context by scene handle
        /// </summary>
        IReadOnlySceneContext Get(int sceneHandle);

        SceneStatus GetStatus(int sceneHandle);

    }
}