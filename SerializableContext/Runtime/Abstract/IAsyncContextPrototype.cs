namespace UniModules.UniGame.Context.SerializableContext.Runtime.Abstract
{
    using System;
    using Core.Runtime.Interfaces;
    using Cysharp.Threading.Tasks;

    public interface IAsyncContextPrototype<TValue> : IDisposable
    {
        UniTask<TValue> Create(IContext context);
    }
}