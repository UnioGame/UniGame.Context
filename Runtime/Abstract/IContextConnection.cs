namespace UniModules.UniGame.Context.Runtime.Connections
{
    using System;
    using Core.Runtime.Interfaces;
    using UniRx;

    public interface IContextConnection : 
        IConnection<IContext>,
        IDisposableContext
    {
        IReadOnlyReactiveProperty<bool> IsEmpty { get; }

        void Disconnect(IContext connection);
    }
}