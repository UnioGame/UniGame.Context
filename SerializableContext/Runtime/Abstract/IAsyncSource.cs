namespace UniModules.UniGame.SerializableContext.Runtime.Abstract
{
    using Core.Runtime.Interfaces;
    using UniModules.UniContextData.Runtime.Interfaces;

    public interface IAsyncSource : IAsyncContextDataSource, ILifeTimeContext
    {
    }
}