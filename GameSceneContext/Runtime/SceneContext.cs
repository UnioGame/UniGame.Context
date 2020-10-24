namespace UniModules.UniGame.Context.GameSceneContext.Runtime {
    using System;
    using System.Collections.Generic;
    using Abstract;
    using Core.Runtime.DataFlow.Interfaces;
    using Core.Runtime.Extension;
    using Core.Runtime.Interfaces;
    using UniContextData.Runtime.Entities;
    using UniCore.Runtime.DataFlow;
    using UniCore.Runtime.Rx.Extensions;
    using UniRx;
    using UnityEngine.SceneManagement;

    [Serializable]
    public class SceneContext : ISceneContext 
    {
                
        private static DummySceneContext dummySceneContext = new DummySceneContext();

        private LifeTimeDefinition _lifeTime;
        private Dictionary<int,IContext> _sceneContexts;
        private Subject<SceneContextHandle> _sceneSubject;

        public SceneContext() {
            
            _lifeTime      = new LifeTimeDefinition();
            _sceneContexts = new Dictionary<int, IContext>(8);
            
            _sceneSubject = new Subject<SceneContextHandle>().AddTo(_lifeTime);

            //release all existing contexts
            _lifeTime.AddCleanUpAction(() => {
                _sceneContexts.ForEach(x => x.Value.Release());
                _sceneContexts.Clear();
            });
            
            //bind with scene events
            Initialize();
        }

        /// <summary>
        /// we must clean all refs before die
        /// </summary>
        ~SceneContext() => _lifeTime.Release();

        #region properties
        
        /// <summary>
        /// scene context lifetime
        /// </summary>
        public ILifeTime LifeTime => _lifeTime;

        /// <summary>
        /// always return context for current active scene
        /// </summary>
        public IReadOnlyContext Active { get; private set; }

        /// <summary>
        /// changes in current active SceneContexts ot statuses
        /// </summary>
        public IObservable<SceneContextHandle> SceneContextAction => _sceneSubject;

        #endregion

        public void Dispose() {
            _lifeTime.Release();
            GC.SuppressFinalize(this);
        }

        public IReadOnlyContext Get(int sceneHandle) => Find(sceneHandle);
        
        public void Release(int sceneHandle) => Find(sceneHandle).Release();

        public SceneContextHandle GetSceneHandle(int sceneHandle) {

            var context  = Find(sceneHandle);
            var status = context == dummySceneContext ? SceneContextStatus.Unload :
                context == Active ? SceneContextStatus.Active : SceneContextStatus.Loaded;
            
            var handle = new SceneContextHandle(sceneHandle,context,status);
            return handle;
        }
        
        
        #region private methods
        
        #region scene events

        private void OnSceneUnload(Scene scene) => Remove(scene);
        
        private void OnSceneLoad(Scene scene,LoadSceneMode mode) {
            var handle                   = scene.handle;
            var context                  = GetOrCreate(handle);
            if (Active == null) Active = context;
            var status                   = Active == context ? SceneContextStatus.Active : SceneContextStatus.Loaded;
            FireSceneStatus(status,handle);
        }
        
        private void OnActiveSceneChanged(Scene fromScene, Scene toScene) {
            Active = GetOrCreate(toScene.handle);
            FireSceneStatus(SceneContextStatus.Active,toScene.handle);
        }

        #endregion

        private void Initialize() {
            Connect();
            Active = Find(SceneManager.GetActiveScene().handle);
        }

        private void Connect() {
            
            Observable.FromEvent(
                    x => SceneManager.sceneLoaded += OnSceneLoad, 
                    x => SceneManager.sceneLoaded -= OnSceneLoad).
                Subscribe().
                AddTo(_lifeTime);
            
            Observable.FromEvent(
                    x => SceneManager.sceneUnloaded += OnSceneUnload, 
                    x => SceneManager.sceneUnloaded -= OnSceneUnload).
                Subscribe().
                AddTo(_lifeTime);
            
            Observable.FromEvent(
                    x => SceneManager.activeSceneChanged += OnActiveSceneChanged, 
                    x => SceneManager.activeSceneChanged -= OnActiveSceneChanged).
                Subscribe().
                AddTo(_lifeTime);
            
        }

        private void FireSceneStatus(SceneContextStatus status, int sceneHandle) {

            var context      = Find(sceneHandle);
            var statusHandle = new SceneContextHandle(sceneHandle,context,status);
            context.Publish(statusHandle);
            _sceneSubject.OnNext(statusHandle);
        }
        
        private IContext Find(int sceneHandle) {
            if (_sceneContexts.TryGetValue(sceneHandle, out var context))
                return context;
            return dummySceneContext;
        }

        private IContext GetOrCreate(int sceneHandle) {
            var context = Find(sceneHandle);
            if (context != dummySceneContext)
                return context;
            context                     = new EntityContext();
            _sceneContexts[sceneHandle] = context;
            return context;
        }

        private void Remove(Scene scene) => Remove(scene.handle);
        
        private void Remove(int sceneHandle) {
            var context     = Find(sceneHandle);
                        
            FireSceneStatus(SceneContextStatus.Unload,sceneHandle);

            context.Dispose();
            _sceneContexts.Remove(sceneHandle);
        }
        
        #endregion
    }
}