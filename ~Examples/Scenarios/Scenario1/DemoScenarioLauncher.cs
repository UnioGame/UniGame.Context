using UnityEngine;

namespace UniModules.UniGame.Context.Scenarios.Scenario1 
{
    using System;
    using Cysharp.Threading.Tasks;
    using Runtime.Context;
    using SerializableContext.Runtime.Scenarios;
    using UniContextData.Runtime.Entities;

    public class DemoScenarioLauncher : MonoBehaviour {

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.InlineProperty]
        [Sirenix.OdinInspector.HideLabel]
#endif
        [SerializeReference]
        public AsyncContextScenario scenario = new AsyncContextScenario();

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
