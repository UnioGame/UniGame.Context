namespace UniModules.UniGame.Context.Runtime.Connections
{
    using global::UniGame.Core.Runtime;
    using UniRx;

    public interface IContextConnection : 
        IConnection<IContext>,
        IDisposableContext
    {
        IReadOnlyReactiveProperty<bool> IsEmpty { get; }

        void Disconnect(IContext connection);
    }
}