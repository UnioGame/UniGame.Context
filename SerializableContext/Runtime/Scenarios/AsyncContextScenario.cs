namespace UniModules.UniGame.Context.SerializableContext.Runtime.Scenarios
{
    using System;
    using System.Collections.Generic;
    using Core.Runtime.DataFlow.Interfaces;
    using Core.Runtime.Interfaces;
    using Cysharp.Threading.Tasks;
    using States;
    using UnityEngine;

    public enum ScenarioStatus : byte
    {
        None,
        Complete,
        Failed,
    }

    [Serializable]
    public class AsyncContextScenario : AsyncContextState<ScenarioStatus>,IAsyncScenario
    {
        [SerializeReference]
        private List<IAsyncScenario> scenarios = new List<IAsyncScenario>();
        
        #region constructor

        public AsyncContextScenario() { }

        public AsyncContextScenario(IEnumerable<IAsyncScenario> nodes) {
            scenarios.AddRange(nodes);
        }
        
        #endregion

        protected override async UniTask<ScenarioStatus> OnExecute(IContext context, ILifeTime executionLifeTime) {

            var cancellationToken = executionLifeTime.AsCancellationSource();
            var isCancelled       = false;
            var lastIndex         = 0;
            
            for (var i = 0; i < scenarios.Count; i++) {
                var asyncScenario = scenarios[i];
                var status        = await asyncScenario.Execute(context).WithCancellation(cancellationToken.Token);
                if (status != ScenarioStatus.Failed) {
                    continue;
                }

                lastIndex   = i;
                isCancelled = true;
                break;
            }

            for (var i = lastIndex; i >=0 && isCancelled; i--) {
                if (scenarios[i] is IAsyncRollback<IContext> contextRollback)
                    await contextRollback.Rollback();
                if (scenarios[i] is IAsyncRollback rollback)
                    await rollback.Rollback();
            }

            return isCancelled ? ScenarioStatus.Failed : ScenarioStatus.Complete;
        }

    }
}