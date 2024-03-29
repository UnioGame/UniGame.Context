﻿namespace UniModules.UniGame.Context.SerializableContext.Runtime.Scenarios
{
    using System;
    using Abstract;
    using Core.Runtime.AsyncOperations;
    using global::UniGame.Core.Runtime;

    [Serializable]
    public class AsyncContextScenario : 
        AsyncScenario<IAsyncContextCommand,IContext>,
        IAsyncContextCommand,
        IAsyncContextRollback,
        IAsyncScenario
    {

        
    }
}