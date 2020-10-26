﻿namespace UniModules.UniGame.Context.GameSceneContext.Runtime {
    using System;
    using Core.Runtime.DataFlow.Interfaces;
    using Core.Runtime.Rx;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Interfaces;
    using UniContextData.Runtime.Entities;
    using UniCore.Runtime.DataFlow;
    using UniRx;

    [Serializable]
    public class SceneContext :
        ISceneContext ,
        IConnector<IMessagePublisher>
    {
        
        private readonly LifeTimeDefinition            _lifeTime = new LifeTimeDefinition();
        private readonly EntityContext                 _context  = new EntityContext();
        private readonly RecycleReactiveProperty<SceneStatus> _status   = new RecycleReactiveProperty<SceneStatus>(SceneStatus.Unload);
        private readonly RecycleReactiveProperty<bool> _isActive = new RecycleReactiveProperty<bool>(false);
        
        private readonly int       _sceneHandle;
        private          SceneInfo _sceneInfo;
        private          int       _handle;

        public SceneContext(int handle) 
        {
            _sceneHandle  = handle;
            UpdateSceneStatus();
        }

        public int    Handle => _handle;

        public string Name => _sceneInfo.name;

        public IReadOnlyReactiveProperty<SceneStatus> Status => _status;
        
        public IReadOnlyReactiveProperty<bool> IsActive => _isActive;

        public ILifeTime LifeTime => _lifeTime;

        #region base equals override

        public override int            GetHashCode() => _sceneHandle;
     
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

        
        public IDisposable Bind(IMessagePublisher connection) => _context.Bind(connection);

        
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