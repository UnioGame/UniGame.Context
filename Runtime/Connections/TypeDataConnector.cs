
namespace UniGame.UniNodes.NodeSystem.Runtime.Connections
{
    using System;
    using System.Runtime.CompilerServices;
    using UniModules.UniCore.Runtime.Common;
    using UniModules.UniCore.Runtime.DataFlow;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime;
    using UniModules.UniCore.Runtime.ObjectPool.Runtime.Interfaces;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniRx;

    public class TypeDataConnector<TConnection> : 
        ITypeDataConnector<TConnection> ,
        ILifeTimeContext,
        IPoolable
    {
        protected ReactiveCollection<TConnection> _registeredItems = new ReactiveCollection<TConnection>();
        protected LifeTimeDefinition _lifeTime = new LifeTimeDefinition();

        public int ConnectionsCount => _registeredItems.Count;
        
        public ILifeTime LifeTime => _lifeTime;
        
        #region ipoolable
        
        public virtual void Release()
        {
            _lifeTime.Release();
            _registeredItems.Clear();
        }
        
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable Bind(TConnection connection)
        {
            if (!_registeredItems.Contains(connection))
                _registeredItems.Add(connection);
            
            var disposable = ClassPool.Spawn<DisposableAction>();
            disposable.Initialize(() => Disconnect(connection));
            
            OnBind(connection);
            
            return disposable.AddTo(LifeTime);
        }

        public void Disconnect(TConnection connection)
        {
            _registeredItems.Remove(connection);
        }

        protected virtual void OnBind(TConnection connection) { }
    }
    
}
