namespace UniGame.Context.Runtime
{
    using global::UniGame.Core.Runtime;

    public interface IContextDataSource
    {
        void Register(IContext context);
    }
}