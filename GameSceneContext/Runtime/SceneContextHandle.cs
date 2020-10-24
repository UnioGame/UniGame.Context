namespace UniModules.UniGame.Context.GameSceneContext.Runtime {
    using Core.Runtime.Interfaces;

    public struct SceneContextHandle 
    {
        public readonly int                sceneHandle;
        public readonly IReadOnlyContext   sceneContext;
        public readonly SceneContextStatus status;
        public readonly string             sceneName;

        public SceneContextHandle(int handle, IReadOnlyContext context, SceneContextStatus contextStatus) {
            sceneHandle  = handle;
            sceneContext = context;
            status       = contextStatus;
            
            var scene = SceneManagerUtils.GetRuntimeScene(handle);
            sceneName    = scene.isLoaded ? scene.name : string.Empty;
        }

        public override int GetHashCode() => sceneHandle;

        public bool Equals(SceneContextHandle obj) {
            return sceneHandle == obj.sceneHandle;
        }
        
        public override bool Equals(object obj) {
            if (obj is SceneContextHandle handle) {
                return handle.sceneHandle == sceneHandle;
            }

            return base.Equals(obj);
        }
    }
}