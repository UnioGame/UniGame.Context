namespace UniModules.UniGame.Context.SerializableContext.Runtime.States
{
    using System;
    using Abstract;
    using Core.Runtime.Common;
    using Core.Runtime.Interfaces;

    [Serializable]
    public class AsyncContextCommandValue : 
        VariantValue<IAsyncContextCommand,AsyncContextStateAsset,IAsyncCommand<IContext,AsyncStatus>>
    {
        
    }
}