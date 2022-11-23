namespace UniGame.Context.Runtime
{
    using Cysharp.Threading.Tasks;
    using global::UniGame.Core.Runtime;

    public interface IAsyncContextDataSource
    {
        UniTask<IContext> RegisterAsync(IContext context);
    }
}
