namespace UniModules.UniGame.Context.SerializableContext.Runtime.Scenarios
{
    using System;
    using Abstract;
    using Core.Runtime.AsyncOperations;
    using Core.Runtime.Interfaces;

    [Serializable]
    public class AsyncContextScenario : 
        AsyncScenario<IAsyncContextCommand,IContext>,
        IAsyncContextCommand,
        IAsyncContextRollback,
        IAsyncScenario
    {

        
    }
}