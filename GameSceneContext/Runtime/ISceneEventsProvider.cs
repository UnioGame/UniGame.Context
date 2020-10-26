namespace UniModules.UniGame.Context.GameSceneContext.Runtime {
    using System;
    using UnityEngine.SceneManagement;

    public interface ISceneEventsProvider : IDisposable {
        IObservable<Scene>                             Unloaded  { get; }
        IObservable<(Scene scene, LoadSceneMode mode)> Loaded    { get; }
        IObservable<(Scene previous, Scene active)>    Activated { get; }
    }
}