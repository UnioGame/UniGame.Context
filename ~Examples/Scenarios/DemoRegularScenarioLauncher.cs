using UnityEngine;

namespace UniModules.UniGame.Context.Scenarios.Scenario1 
{
    using System;
    using Core.Runtime.AsyncOperations;
    using Core.Runtime.Interfaces;
    using Cysharp.Threading.Tasks;
    using UniContextData.Runtime.Entities;
    using UniRx;

    public class DemoRegularScenarioLauncher : MonoBehaviour {

        [SerializeReference]
        public AsyncScenario<IAsyncCommand<IContext,Unit>,IContext> scenario = new AsyncScenario<IAsyncCommand<IContext,Unit>, IContext>();

        public int runners = 1;

        public float delay = 0;
        
        // Start is called before the first frame update
        private async void Start()
        {
            var context = new EntityContext();
            
            for (var i = 0; i < runners; i++) {

                scenario.Execute(context);

                await UniTask.Delay(TimeSpan.FromSeconds(delay));
            }
        }

    }
}
