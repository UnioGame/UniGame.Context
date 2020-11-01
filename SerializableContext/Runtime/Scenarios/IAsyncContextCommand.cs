namespace UniModules.UniGame.Context.SerializableContext.Runtime.Scenarios {
    using Core.Runtime.Interfaces;
    using UniRx;

    public interface IAsyncContextCommand : IAsyncCommand<IContext, Unit> {
        
    }
}