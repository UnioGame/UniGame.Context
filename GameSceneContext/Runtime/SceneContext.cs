﻿namespace UniModules.UniGame.Context.GameSceneContext.Runtime {
    using System;
    using Context.Runtime.Context;
    using global::UniGame.Core.Runtime;
    using Core.Runtime.Rx;
    using UniContextData.Runtime.Entities;
    using UniCore.Runtime.DataFlow;
    using UniRx;

    [Serializable]
    public class SceneContext : ISceneContext 
    {
        private readonly LifeTimeDefinition                   _lifeTime = new LifeTimeDefinition();
        private readonly EntityContext                        _context  = new EntityContext();
        private readonly ReactiveValue<SceneStatus> _status   = new ReactiveValue<SceneStatus>(SceneStatus.Unload);
        private readonly ReactiveValue<bool>        _isActive = new ReactiveValue<bool>(false);

        private readonly int       _sceneHandle;
        private          SceneInfo _sceneInfo;
        private          int       _handle;

        public SceneContext(int handle) {
            _sceneHandle = handle;
            UpdateSceneStatus();
        }

        public int BindingsCount => _context.BindingsCount;

        public int Handle => _handle;

        public string Name => _sceneInfo.name;

        public IReadOnlyReactiveProperty<SceneStatus> Status => _status;

        public IReadOnlyReactiveProperty<bool> IsActive => _isActive;

        public ILifeTime LifeTime => _lifeTime;

        #region base equals override

        public override int GetHashCode() => _sceneHandle;

        public bool Equals(SceneContext obj) {
            return _sceneHandle == obj._sceneHandle;
        }

        public bool Equals(IReadOnlySceneContext obj) {
            return _handle == obj.Handle;
        }

        public override bool Equals(object obj) {
            if (obj is SceneContext handle) {
                return handle._sceneHandle == _sceneHandle;
            }

            return base.Equals(obj);
        }

        public static bool operator ==(SceneContext obj1, IReadOnlySceneContext obj2) {
            return obj1?.Handle == obj2?.Handle;
        }

        public static bool operator !=(SceneContext obj1, IReadOnlySceneContext obj2) {
            return obj1?.Handle != obj2?.Handle;
        }

        #endregion


        public IDisposable Broadcast(IMessagePublisher connection) => _context.Broadcast(connection);

        public void Break(IMessagePublisher connection) {
            _context.Break(connection);
        }

        #region context api

        public void Dispose() => Release();

        public void Release() {
            _status.Value   = SceneStatus.Unload;
            _isActive.Value = false;
            _context.Release();
        }

        public IObservable<T> Receive<T>() => _context.Receive<T>();

        public void Publish<T>(T message) => _context.Publish(message);

        public bool HasValue => _context.HasValue;

        public object Get(Type type) => _context.Get(type);

        public TData Get<TData>() => _context.Get<TData>();

        public bool Contains<TData>() => _context.Contains<TData>();

        public bool Remove<TData>() => _context.Remove<TData>();

        #endregion

        public void UpdateSceneStatus() {
            _sceneInfo      = SceneManagerUtils.GetSceneInfo(_sceneHandle);
            _status.Value   = _sceneInfo.status;
            _isActive.Value = _sceneInfo.isActive;
        }
    }
}