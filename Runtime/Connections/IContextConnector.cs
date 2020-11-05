namespace UniModules.UniGame.Context.Runtime.Connections
{
    using System;
    using Core.Runtime.Interfaces;
    using UniRx;

    public interface IContextConnector : 
        ITypeDataConnector<IContext>,
        IMessageBroker,
        IDisposable
    {
        IContext Context { get; }
    }
}