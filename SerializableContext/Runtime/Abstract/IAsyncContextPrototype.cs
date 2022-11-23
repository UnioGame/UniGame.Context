namespace UniModules.UniGame.Context.SerializableContext.Runtime.Abstract
{
    using System;
    using global::UniGame.Core.Runtime;

    public interface IAsyncContextPrototype<TResult> 
        : IDisposable, IAsyncFactory<IContext,TResult>
    {
    }
}