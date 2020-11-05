
namespace UniGame.UniNodes.NodeSystem.Runtime.Connections
{
    using System.Runtime.CompilerServices;
    using UniModules.UniGame.Context.Runtime.Connections;
    using UniRx;
    
    public class TypeDataBrodcaster : 
        TypeDataConnector<IMessagePublisher>,
        IMessagePublisher
    {

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

    }
    
}
