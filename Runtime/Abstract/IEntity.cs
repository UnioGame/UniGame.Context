namespace UniGame.Context.Runtime
{
    using global::UniGame.Core.Runtime;

    public interface IEntity : IDisposableContext
    {
        int Id { get; }
    }
}