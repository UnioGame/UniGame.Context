namespace UniModules.UniContextData.Runtime.Interfaces
{
    using UniGame.Core.Runtime.Interfaces;

    public interface IEntity : IContext
    {
        int Id { get; }
    }
}