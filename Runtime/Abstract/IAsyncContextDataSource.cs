namespace UniModules.UniContextData.Runtime.Interfaces
{
    using Cysharp.Threading.Tasks;
    using UniGame.Core.Runtime.Interfaces;

    public interface IAsyncContextDataSource
    {
        UniTask<IContext> RegisterAsync(IContext context);
    }
}
