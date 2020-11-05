namespace UniModules.UniGame.Context.Runtime.Connections
{
    using Core.Runtime.Interfaces;

    public interface IContextConnector : ITypeDataConnector<IContext> , IContext
    {
    }
}