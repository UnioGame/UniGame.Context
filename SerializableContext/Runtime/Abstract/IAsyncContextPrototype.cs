using Cysharp.Threading.Tasks;
using UniModules.UniCore.Runtime.Interfaces;

namespace UniModules.UniGame.SerializableContext.Runtime.Abstract
{
    public interface IAsyncContextPrototype<TValue>
    {
        UniTask<TValue> Create(IContext context);
    }
}