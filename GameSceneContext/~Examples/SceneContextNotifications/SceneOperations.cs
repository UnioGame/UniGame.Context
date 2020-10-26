using UnityEngine;

namespace UniModules.UniGame.Context.GameSceneContext.SceneContextNotifications {
    using System;
    using System.Collections.Generic;
    using Runtime;
    using UnityEngine.SceneManagement;
    using Random = UnityEngine.Random;

    [Serializable]
    public class SceneDemoInfo {
        public Scene  scene;
        public string name;
        public int handle;
    }
    
    public class SceneOperations : MonoBehaviour {
        
        private List<Scene> _loadedScenes = new List<Scene>();
        
        public string currentScene;
        
        public List<SceneDemoInfo> loadedScenes = new List<SceneDemoInfo>();
        
        public void Start() {
            
            currentScene = string.IsNullOrEmpty(currentScene) ? gameObject.scene.name  : currentScene;
            DontDestroyOnLoad(gameObject);
            
        }
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void UpdateScenesInfo() {
            var scenes = SceneManagerUtils.GetRuntimeScenes(currentScene);
            loadedScenes.Clear();
            foreach (var scene in scenes) {
                loadedScenes.Add(new SceneDemoInfo() {
                    handle = scene.handle,
                    name = scene.name,
                    scene = scene
                });
            }
        }
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void LoadAdditive() {
            SceneManager.LoadScene(currentScene, LoadSceneMode.Additive);
            UpdateScenesInfo();
        }
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void UpdateActiveScene() {
            var scene = SceneManager.GetSceneAt(Random.Range(0, SceneManager.sceneCount));
            SceneManager.SetActiveScene(scene);
        }
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void Load() {
            SceneManager.LoadScene(currentScene, LoadSceneMode.Single);
            UpdateScenesInfo();
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void Unload() {
            foreach (var scene in SceneManagerUtils.GetRuntimeScenes(currentScene)) {
                SceneManager.UnloadSceneAsync(scene);  
            }
            UpdateScenesInfo();
        }

        
        
    }
}
