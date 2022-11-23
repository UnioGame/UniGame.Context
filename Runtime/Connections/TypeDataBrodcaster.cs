
namespace UniModules.GameFlow.Runtime.Connections
{
    using System;
    using System.Runtime.CompilerServices;
    using UniModules.UniGame.Context.Runtime.Connections;
    using global::UniGame.Core.Runtime;
    using UniRx;
    
    public class TypeDataBrodcaster : 
        TypeDataCollection<IMessagePublisher>,
        IManagedBroadcaster<IMessagePublisher> ,
        IMessagePublisher
    {

        public int BindingsCount => Count;
        
        #region IContextData interface

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Publish<TData>(TData value) {
            for (var i = 0; i < _registeredItems.Count; i++) 
            {
                var context = _registeredItems[i];
                context.Publish(value);
            }
        }

        #endregion

        public IDisposable Broadcast(IMessagePublisher connection) => Add(connection);

        public void Break(IMessagePublisher connection) => Remove(connection);
    }
    
}
