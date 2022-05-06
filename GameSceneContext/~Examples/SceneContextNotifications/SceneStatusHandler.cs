using UnityEngine;

namespace UniModules.UniGame.Context.GameSceneContext.SceneContextNotifications {
    using global::UniCore.Runtime.ProfilerTools;
    using Runtime;
    using UniRx;

    public class SceneStatusHandler : MonoBehaviour
    {
        // Start is called before the first frame update
        private void Start()
        {
            DontDestroyOnLoad(gameObject);

            Connect();
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        private void Connect() {
            this.NotifyOnAllSceneContext().
                RxSubscribe(LoadSceneContext).
                AddTo(this);
        }
        
        private void LoadSceneContext(IReadOnlySceneContext sceneContext) {
            
            GameLog.Log($"SCENE Name : {sceneContext.Name} \nhandle : {sceneContext.Handle} \nstatus : {sceneContext.Status.Value} \nisActive {sceneContext.IsActive.Value}");
            
        }
    }
}
