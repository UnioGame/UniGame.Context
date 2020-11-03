namespace UniModules.UniGame.Context.GameSceneContext.Runtime {
    using System;
    using System.Collections.Generic;
    using Core.Runtime.DataFlow.Interfaces;
    using Core.Runtime.Interfaces;
    using UniCore.Runtime.DataFlow;
    using UniRx;

    public class DummyReadOnlySceneContext : ISceneContext {

        private IReadOnlyDictionary<Type, IValueContainerStatus> editorValues;
        private LifeTimeDefinition                               lifeTime;
        private int                                              handle;
        private IReadOnlyReactiveProperty<bool>                  isActive;

        public DummyReadOnlySceneContext() {
            editorValues = new Dictionary<Type, IValueContainerStatus>(0);
            lifeTime     = new LifeTimeDefinition();
            lifeTime.Terminate();
        }
        
        public void Release() {}

        public void Publish<T>(T message) {}

        public IObservable<T> Receive<T>() => Observable.Empty<T>();

        public bool HasValue => false;

        public IReadOnlyReactiveProperty<bool> IsActive { get; } = Observable.Empty<bool>(false).ToReactiveProperty();

        public IReadOnlyReactiveProperty<SceneStatus> Status { get; } = Observable.Empty<SceneStatus>(SceneStatus.Unload).ToReactiveProperty();

        public TData                                  Get<TData>() => default(TData);

        public bool Contains<TData>() => false;

        public bool Remove<TData>() => false;

        public IReadOnlyDictionary<Type, IValueContainerStatus> EditorValues => editorValues;

        public void Dispose() { }

        public void UpdateSceneStatus() {
        }

        public ILifeTime LifeTime => lifeTime;

        public int    Handle => Int32.MaxValue;

        public string Name => string.Empty;

        public IDisposable Bind(IMessagePublisher connection) => Disposable.Empty;

        public int ConnectionsCount => 0;

        public void Disconnect(IMessagePublisher connection) {
            
        }
    }
}