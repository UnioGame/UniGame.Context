﻿
namespace UniModules.GameFlow.Runtime.Connections
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using UniModules.UniCore.Runtime.Common;
    using global::UniGame.Runtime.ObjectPool;
    using global::UniGame.Core.Runtime.ObjectPool;
    using global::UniGame.Core.Runtime;
    using UniRx;

    
    
    public class TypeDataReceiver : 
        IPoolable, 
        IBroadcaster<IMessagePublisher>,
        IMessagePublisher
    {
        private List<IMessagePublisher> _registeredItems = new List<IMessagePublisher>();
        private int count = 0;

        public int ConnectionsCount => count;
        
        #region ipoolable
        
        public virtual void Release()
        {
            _registeredItems.Clear();
            UpdateCounter();
        }
        
        #endregion
        
        #region IContextData interface

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish<TData>(TData value)
        {
            for (var i = 0; i < count; i++)
            {
                var context = _registeredItems[i];
                context.Publish(value);
            }
        }

        #endregion


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public IDisposable Broadcast(IMessagePublisher connection)
        {
            if (!_registeredItems.Contains(connection))
                _registeredItems.Add(connection);
            
            var disposable = ClassPool.Spawn<DisposableAction>();
            disposable.Initialize(() => Disconnect(connection));
            
            UpdateCounter();
            return disposable;
        }

        public void Disconnect(IMessagePublisher connection)
        {
            _registeredItems.Remove(connection);
            UpdateCounter();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UpdateCounter()
        {
            count = _registeredItems.Count;
        }
    }
    
}
