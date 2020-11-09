namespace UniModules.UniGame.Context.Runtime.Connections
{
    using System;
    using Core.Runtime.Interfaces;

    public interface IContextConnector : 
        ITypeDataConnector<IContext>,
        IMessageContext,
        IDisposable
    {
    }
}