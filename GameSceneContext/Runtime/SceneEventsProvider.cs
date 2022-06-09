namespace UniModules.UniGame.Context.GameSceneContext.Runtime {
    using System;
    using UniCore.Runtime.DataFlow;
    using UniRx;
    using UnityEngine.SceneManagement;

    public class SceneEventsProvider : ISceneEventsProvider 
    {
        private LifeTimeDefinition                         _lifeTime       = new LifeTimeDefinition();
        private Subject<Scene>                             _unloadSubject  = new Subject<Scene>();
        private Subject<(Scene scene, LoadSceneMode mode)> _loadingSubject = new Subject<(Scene, LoadSceneMode)>();
        private Subject<(Scene previous, Scene active)>    _activeSubject  = new Subject<(Scene previous, Scene active)>();

        public SceneEventsProvider() {
            Observable.FromEvent(
                x => SceneManager.sceneLoaded += OnSceneLoad,
                x => SceneManager.sceneLoaded -= OnSceneLoad).Subscribe().AddTo(_lifeTime);

            Observable.FromEvent(
                x => SceneManager.sceneUnloaded += OnSceneUnload,
                x => SceneManager.sceneUnloaded -= OnSceneUnload).Subscribe().AddTo(_lifeTime);

            Observable.FromEvent(
                x => SceneManager.activeSceneChanged += OnActiveSceneChanged,
                x => SceneManager.activeSceneChanged -= OnActiveSceneChanged).Subscribe().AddTo(_lifeTime);
        }

        public IObservable<Scene>                             Unloaded  => _unloadSubject;
        public IObservable<(Scene scene, LoadSceneMode mode)> Loaded    => _loadingSubject;
        public IObservable<(Scene previous, Scene active)>    Activated => _activeSubject;

        public void Dispose() => _lifeTime.Terminate();

        #region private methods

        private void OnSceneUnload(Scene scene) {
            _unloadSubject.OnNext(scene);
        }

        private void OnSceneLoad(Scene scene, LoadSceneMode mode) {
            _loadingSubject.OnNext((scene,mode));
        }

        private void OnActiveSceneChanged(Scene fromScene, Scene toScene) {
            _activeSubject.OnNext((fromScene,toScene));
        }

        #endregion
    }
}