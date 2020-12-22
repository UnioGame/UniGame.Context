namespace UniModules.UniGame.Context.GameSceneContext.Runtime {
    using Core.Runtime.Interfaces;
    using UniRx;

    public interface IReadOnlySceneContext : IMessageContext
    {
        int                             Handle { get; }

        string                                 Name   { get; }
        
        IReadOnlyReactiveProperty<bool> IsActive { get; }

        IReadOnlyReactiveProperty<SceneStatus> Status { get; }
    }

    public interface ISceneContext : 
        IReadOnlySceneContext, 
        IManagedBroadcaster<IMessagePublisher>,
        IContext {
        
        
        void UpdateSceneStatus();

    }
}