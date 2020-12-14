
namespace UniGame.UniNodes.NodeSystem.Runtime.Connections
{
    using System;
    using System.Runtime.CompilerServices;
    using UniModules.UniGame.Context.Runtime.Connections;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UniRx;
    
    public class TypeDataBrodcaster : 
        TypeDataCollection<IMessagePublisher>,
        IManagedBinder<IMessagePublisher> ,
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

        public IDisposable Bind(IMessagePublisher connection) => Add(connection);

        public void Break(IMessagePublisher connection) => Remove(connection);
    }
    
}
