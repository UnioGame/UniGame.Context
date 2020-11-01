namespace UniModules.UniGame.Context.SerializableContext.Runtime.Scenarios
{
    using System;
    using System.Collections.Generic;
    using Core.Runtime.AsyncOperations;
    using Core.Runtime.Interfaces;

    [Serializable]
    public class AsyncContextScenario : 
        AsyncScenario<IAsyncContextCommand,IContext>,
        IAsyncContextRollback,
        IAsyncScenario
    {
        #region constructor

        public AsyncContextScenario() : base() { }

        public AsyncContextScenario(IEnumerable<IAsyncContextCommand> nodes) : 
            base(nodes)
        {
            
        }
        
        #endregion
        
    }
}