namespace UniModules.UniGame.Context.GameSceneContext.Runtime 
{
    using System;
    using Core.Runtime.Interfaces;
    using UniRx;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.SceneManagement;

    public static class SceneContextSystem 
    {
        private static SceneContext sceneContext = new SceneContext();


        static SceneContextSystem() {

#if UNITY_EDITOR
            EditorApplication.playModeStateChanged += x => sceneContext.Dispose();
#endif

        }
        
        
        public static IReadOnlyContext Active                               => sceneContext.Active;
        public static IReadOnlyContext GetActiveContext(this object source) => sceneContext.Active;

        public static IReadOnlyContext GetContextFor(this GameObject gameObject) => sceneContext.Get(gameObject.scene.handle);

        public static IReadOnlyContext GetContextFor(this Component component) => GetActiveContext(component.gameObject);

        public static IReadOnlyContext GetContextFor(this int sceneHandle) => sceneContext.Get(sceneHandle);

        public static TValue ReleaseWithScene<TValue>(this TValue value, int handle)
            where TValue : IDisposable 
        {
            var context = GetContextFor(handle);
            var lifeTime = context;
            lifeTime.LifeTime.AddDispose(value);
            return value;
        }

        public static TValue ReleaseWithScene<TValue>(this TValue value, Scene scene)
            where TValue : IDisposable {
            return ReleaseWithScene(value, scene.handle);
        }

        public static void ReleaseContextFor(this GameObject gameObject) {
            if (!gameObject) return;
            sceneContext.Release(gameObject.scene.handle); 
        }

        public static void ReleaseContextFor(this Component component) {
            if (!component)
                return;
            ReleaseContextFor(component.gameObject);
        }

        public static IObservable<SceneContextHandle> SubscribeOnSceneContext(this object source,string sceneName) 
        {
            return SubscribeOnSceneContext(sceneName);
        }
        
        public static IObservable<SceneContextHandle> SubscribeOnSceneContext(this GameObject gameObject) 
        {
            return SubscribeOnSceneContext(gameObject.scene.name);
        }
        
        public static IObservable<SceneContextHandle> SubscribeOnSceneContext(this Component component) 
        {
            return SubscribeOnSceneContext(component.gameObject);
        }
        
        public static IObservable<SceneContextHandle> SubscribeOnSceneContext(string sceneName) {

            var scene  = SceneManagerUtils.GetRuntimeScene(sceneName);
            var filter = sceneContext.
                SceneContextAction.
                Where(x => x.sceneName == sceneName);
            
            return scene.isLoaded ? 
                Observable.
                    Return(sceneContext.GetSceneHandle(scene.handle)).
                    Concat(filter) : 
                filter;
        }

        public static IObservable<SceneContextHandle> SubscribeOnSceneContext(string sceneName,SceneContextStatus status) {

            return SubscribeOnSceneContext(sceneName).
                Where(x => x.status == status);

        }

        

    }


    public static class SceneManagerUtils {
        
        public static Scene GetRuntimeScene(string sceneName) 
        {
            for (int i = 0; i < SceneManager.sceneCount; i++) {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.name == sceneName)
                    return scene;
            }    
            return new Scene();
        }
        
        public static Scene GetRuntimeScene(int handle) 
        {
            for (int i = 0; i < SceneManager.sceneCount; i++) {
                var scene = SceneManager.GetSceneAt(i);
                if (scene.handle == handle)
                    return scene;
            }    
            return new Scene();
        }
        
    }
    
}
