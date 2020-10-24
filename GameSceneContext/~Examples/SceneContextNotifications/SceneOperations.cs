using UnityEngine;

namespace UniModules.UniGame.Context.GameSceneContext.SceneContextNotifications {
    using System.Collections.Generic;
    using Runtime;
    using UnityEngine.SceneManagement;

    public class SceneOperations : MonoBehaviour {
        
        private List<Scene> _loadedScenes = new List<Scene>();
        
        public string currentScene;
        
        public void Start() {
            
            currentScene = string.IsNullOrEmpty(currentScene) ? gameObject.scene.name  : currentScene;
            DontDestroyOnLoad(gameObject);
            
        }

        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void LoadAdditive() {
            SceneManager.LoadScene(currentScene, LoadSceneMode.Additive);
        }
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void Load() {
            SceneManager.LoadScene(currentScene, LoadSceneMode.Single);
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void Unload() {
            foreach (var scene in SceneManagerUtils.GetRuntimeScenes(currentScene)) {
                SceneManager.UnloadSceneAsync(scene);  
            }
        }

    }
}
