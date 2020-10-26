namespace UniModules.UniGame.Context.SerializableContext.Runtime.Scenarios {
    using System;
    using Core.Runtime.Rx;

    [Serializable]
    public class ScenarioStatusProperty : RecycleReactiveProperty<ScenarioStatus> {

        public ScenarioStatusProperty(ScenarioStatus status) : base(status) {
            
        }

        public ScenarioStatusProperty() : base() {
            
        }
        
    }
}