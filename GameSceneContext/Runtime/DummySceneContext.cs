namespace UniModules.UniGame.Context.GameSceneContext.Runtime {
    using System;
    using System.Collections.Generic;
    using Core.Runtime.DataFlow.Interfaces;
    using Core.Runtime.Interfaces;
    using UniCore.Runtime.DataFlow;
    using UniRx;

    public class DummySceneContext : IContext {

        private IReadOnlyDictionary<Type, IValueContainerStatus> editorValues;
        private LifeTimeDefinition                               lifeTime;

        public DummySceneContext() {
            editorValues = new Dictionary<Type, IValueContainerStatus>(0);
            lifeTime     = new LifeTimeDefinition();
            lifeTime.Terminate();
        }
        
        public void Release() {}

        public void Publish<T>(T message) {}

        public IObservable<T> Receive<T>() => Observable.Empty<T>();

        public bool HasValue => false;

        public TData Get<TData>() => default(TData);

        public bool Contains<TData>() => false;

        public bool Remove<TData>() => false;

        public IReadOnlyDictionary<Type, IValueContainerStatus> EditorValues => editorValues;

        public void Dispose() { }

        public ILifeTime LifeTime => lifeTime;
    
    }
}