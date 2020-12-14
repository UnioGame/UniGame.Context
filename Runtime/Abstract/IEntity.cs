namespace UniModules.UniContextData.Runtime.Interfaces
{
    using UniGame.Core.Runtime.Interfaces;

    public interface IEntity : IDisposableContext
    {
        int Id { get; }
    }
}