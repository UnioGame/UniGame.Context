namespace UniModules.UniGame.Context.GameSceneContext.Runtime {
    using Core.Runtime.Interfaces;
    using UniRx;

    public interface IReadOnlySceneContext : IReadOnlyContext
    {
        int                             Handle { get; }

        string                                 Name   { get; }
        
        IReadOnlyReactiveProperty<bool> IsActive { get; }

        IReadOnlyReactiveProperty<SceneStatus> Status { get; }
    }

    public interface ISceneContext : 
        IReadOnlySceneContext, 
        IContext {
        
        
        void UpdateSceneStatus();

    }
}