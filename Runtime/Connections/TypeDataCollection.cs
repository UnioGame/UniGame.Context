
namespace UniModules.UniGame.Context.Runtime.Connections
{
    using System;
    using System.Runtime.CompilerServices;
    using Core.Runtime.DataFlow.Interfaces;
    using Core.Runtime.Interfaces;
    using UniCore.Runtime.Common;
    using UniCore.Runtime.DataFlow;
    using UniCore.Runtime.ObjectPool.Runtime;
    using UniCore.Runtime.ObjectPool.Runtime.Interfaces;
    using UniCore.Runtime.Rx.Extensions;
    using UniRx;

    public class TypeDataCollection<TData> : 
        ILifeTimeContext,
        IPoolable
    {
        protected ReactiveCollection<TData> _registeredItems = new ReactiveCollection<TData>();
        protected LifeTimeDefinition _lifeTime = new LifeTimeDefinition();

        public int Count => _registeredItems.Count;
        
        public ILifeTime LifeTime => _lifeTime;
        
        #region ipoolable
        
        public virtual void Release()
        {
            _lifeTime.Release();
            _registeredItems.Clear();
            
            OnRelease();
        }
        
        #endregion

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable Add(TData connection)
        {
            if (_registeredItems.Contains(connection))
                return Disposable.Empty;
            
            _registeredItems.Add(connection);
            
            var disposable = ClassPool.Spawn<DisposableAction>();
            disposable.Initialize(() => Remove(connection));
            
            OnBind(connection);
            
            return disposable.AddTo(LifeTime);
        }

        public void Remove(TData connection)
        {
            _registeredItems.Remove(connection);
        }

        protected virtual void OnBind(TData connection) { }

        protected virtual void OnRelease()
        {
        }
    }
    
}
