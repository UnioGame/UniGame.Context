using UnityEngine;

namespace UniModules.UniGame.Context.GameSceneContext.SceneContextNotifications {
    using global::UniCore.Runtime.ProfilerTools;
    using Runtime;
    using UniRx;
    using UnityEngine.SceneManagement;

    public class SceneContextNotifications : MonoBehaviour
    {
        public void Start() {
            
            this.SubscribeOnSceneContext().
                Subscribe(OnSceneContextChanged).
                AddTo(this);
            
            DontDestroyOnLoad(gameObject);
            
        }

        public void Update() {
            
        }

        public void Action() {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }


        private void OnSceneContextChanged(SceneContextHandle handle) {
            GameLog.Log($"SCENE: {handle.sceneName} : {handle.sceneHandle} : {handle.status}",Color.blue);
        }
    }
}
