namespace UniGame.Context.Runtime
{
    using Cysharp.Threading.Tasks;
    using global::UniGame.Core.Runtime;

    public interface IAsyncDataSource
    {
        UniTask<IContext> RegisterAsync(IContext context);
    }
}
