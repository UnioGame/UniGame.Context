using UnityEngine;

namespace UniModules.UniGame.Context.Scenarios.Scenario1 
{
    using System;
    using Core.Runtime.AsyncOperations;
    using Core.Runtime.Interfaces;
    using Cysharp.Threading.Tasks;
    using Runtime.Context;
    using UniContextData.Runtime.Entities;
    using UniRx;

    public class DemoRegularScenarioLauncher : MonoBehaviour {

        [SerializeReference]
        public AsyncScenario<IAsyncCommand<IContext,AsyncStatus>,IContext> scenario = new AsyncScenario<IAsyncCommand<IContext,AsyncStatus>, IContext>();

        public int runners = 1;

        public float delay = 0;
        
        // Start is called before the first frame update
        private async void Start()
        {
            var context = new EntityContext();
            
            for (var i = 0; i < runners; i++) {

                scenario.ExecuteAsync(context);

                await UniTask.Delay(TimeSpan.FromSeconds(delay));
            }
        }

    }
}
