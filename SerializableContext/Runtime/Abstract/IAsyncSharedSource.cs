using Cysharp.Threading.Tasks;
using UniGame.Context.Runtime;
using UniGame.Core.Runtime;

namespace UniGame.Rx.Runtime
{
    public interface IAsyncSharedSource
    {
        UniTask<IAsyncDataSource> GetSource<TAsset>(IContext context);
    }
}