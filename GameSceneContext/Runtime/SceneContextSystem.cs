namespace UniModules.UniGame.Context.GameSceneContext.Runtime 
{
    using System;
    using UniRx;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.SceneManagement;
    
    public static class SceneContextSystem 
    {
        private static ScenesContext scenesContext = new ScenesContext(new SceneEventsProvider());

        static SceneContextSystem() {

            //editor only scene behaviour
// #if UNITY_EDITOR
//             EditorApplication.playModeStateChanged += x => {
//                 if (x != PlayModeStateChange.ExitingPlayMode)
//                     return;
//                 scenesContext.Dispose();
//                 scenesContext = new ScenesContext(new SceneEventsProvider());
//             };
// #endif

        }

        [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
        public static void Initialize() {}

        public static IReadOnlySceneContext Active                               => scenesContext.Active;
        public static IReadOnlySceneContext GetActiveContext(this object source) => scenesContext.Active;

        public static IReadOnlySceneContext GetContextFor(this GameObject gameObject) => scenesContext.Get(gameObject.scene.handle);

        public static IReadOnlySceneContext GetContextFor(this Component component) => GetActiveContext(component.gameObject);

        public static IReadOnlySceneContext GetContextFor(this int sceneHandle) => scenesContext.Get(sceneHandle);

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
            scenesContext.Release(gameObject.scene.handle); 
        }

        public static void ReleaseContextFor(this Component component) {
            if (!component)
                return;
            ReleaseContextFor(component.gameObject);
        }
        
        #region notifications

        public static IObservable<IReadOnlySceneContext> NotifyActiveSceneContext(this object source) {
            return scenesContext.ActiveContext;
        }
        
        public static IObservable<IReadOnlySceneContext> NotifyActiveSceneContext(this object source,SceneStatus status) {
            return scenesContext.ActiveContext.Where(x => x.Status.Value == status);
        }
        
        public static IObservable<IReadOnlySceneContext> NotifyOnAllSceneContext(this object source) {
            return scenesContext.ContextChanges;
        }
        
        public static IObservable<IReadOnlySceneContext> NotifyOnAllSceneContext(this object source,SceneStatus status) {
            return scenesContext.ContextChanges.Where(x => x.Status.Value == status);
        }
        
        public static IObservable<IReadOnlySceneContext> NotifyOnSceneContext(this object source,string sceneName) 
        {
            return NotifyOnSceneContext(sceneName);
        }
        
        public static IObservable<IReadOnlySceneContext> NotifyOnSceneContext(this GameObject gameObject) 
        {
            return NotifyOnSceneContext(gameObject.scene.handle);
        }
        
        public static IObservable<IReadOnlySceneContext> NotifyOnSceneContext(this Component component) 
        {
            return NotifyOnSceneContext(component.gameObject);
        }
        
        public static IObservable<IReadOnlySceneContext> NotifyOnSceneContext(int handle) {

            var scene  = SceneManagerUtils.GetRuntimeScene(handle);
            var filter = scenesContext.
                ContextChanges.
                Where(x => x.Handle == handle);
            
            return scene.isLoaded ? 
                Observable.Return(scenesContext.Get(scene.handle)).
                    Concat(filter) : 
                filter;
        }

        public static IObservable<IReadOnlySceneContext> NotifyOnSceneContext(int handle,SceneStatus status) {
            return NotifyOnSceneContext(handle).
                Where(x => x.Status.Value == status);
        }

        public static IObservable<IReadOnlySceneContext> NotifyOnSceneContext(string sceneName) {

            var scene = SceneManagerUtils.GetRuntimeScene(sceneName);
            var filter = scenesContext.
                ContextChanges.
                Where(x => x.Name == sceneName);
            
            return scene.isLoaded ? 
                Observable.Return(scenesContext.Get(scene.handle)).
                    Concat(filter) : 
                filter;
        }

        public static IObservable<IReadOnlySceneContext> NotifyOnSceneContext(string sceneName,SceneStatus status) {
            return NotifyOnSceneContext(sceneName).
                Where(x => x.Status.Value == status);
        }


        #endregion
        
        #region messages

        public static IObservable<TValue> ReceiveFromScene<TValue>(int sceneHanle) {

            var sceneThread = NotifyOnSceneContext(sceneHanle, SceneStatus.Loaded).
                Select(x => x.Receive<TValue>()).
                Switch();
            return sceneThread;
        }
        
        public static IObservable<TValue> ReceiveFromScene<TValue>(string sceneName) {

            var sceneThread = NotifyOnSceneContext(sceneName, SceneStatus.Loaded).
                Select(x => x.Receive<TValue>()).
                Switch();
            return sceneThread;
        }

        public static void PublishToScene<TValue>(this object source,string name, TValue value) {
            foreach (var scene in SceneManagerUtils.GetRuntimeScenes(name)) {
                var context = scenesContext.Get(scene.handle);
                context.Publish(value);
            }
        }
        
        public static void PublishToScene<TValue>(this object source,int handle, TValue value) {
            var context = scenesContext.Get(handle);
            context.Publish(value);
        }
        
        #endregion

    }
}
