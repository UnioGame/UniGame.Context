namespace UniModules.UniGame.Context.SerializableContext.Runtime.Abstract
{
    using System;
    using Cysharp.Threading.Tasks;
    using UniModules.UniCore.Runtime.Interfaces;
    
    public interface IAsyncContextPrototype<TValue> : IDisposable
    {
        UniTask<TValue> Create(IContext context);
    }
}