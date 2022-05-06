using UnityEngine;

namespace UniModules.UniGame.Context.GameSceneContext.SceneContextNotifications {
    using System.Collections.Generic;
    using global::UniCore.Runtime.ProfilerTools;
    using Runtime;
    using UniRx;
    using UnityEngine.SceneManagement;

    public struct SceneDemoTimeData {
        public string name;
        public string scene;
        public string message;
    }
    
    public class SceneMessages : MonoBehaviour {

        private int messageCount = 0;
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ValueDropdown(nameof(GetHandles))]
#endif
        public int    targetSceneHandle;
        
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.ValueDropdown(nameof(GetNames))]
#endif
        public string targetSceneName;
        
        public bool   sendByName   = false;
        public bool   sendToAll    = true;
        public bool   sendByHandle = false;
        
        public List<SceneDemoInfo> loadedScenes = new List<SceneDemoInfo>();

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void UpdateScenesInfo() {
            loadedScenes.Clear();
            for (int i = 0; i < SceneManager.sceneCount; i++) {
                var scene = SceneManager.GetSceneAt(i);
                loadedScenes.Add(new SceneDemoInfo() {
                    handle = scene.handle,
                    name   = scene.name,
                    scene  = scene
                });
            }
        }

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.Button]
#endif
        public void SendMessage() {
            messageCount++;
            if (sendToAll) {
                var message = new SceneDemoTimeData() {
                    name    = "SEND_TO_ALL",
                    scene = "ALL",
                    message = $"Value {messageCount} : {Time.realtimeSinceStartup}" 
                };
                this.PublishToAllScenes(message);
            }

            if (sendByName && !string.IsNullOrEmpty(targetSceneName)) {
                var message = new SceneDemoTimeData() {
                    name    = "SEND_BY_NAME",
                    scene   = targetSceneName,
                    message = $"Value {messageCount} : {Time.realtimeSinceStartup}" 
                };
                this.PublishToScene(targetSceneName,message);
            }
            
            if (sendByHandle) {
                var message = new SceneDemoTimeData() {
                    name    = "SEND_BY_HANDLE",
                    scene   = targetSceneHandle.ToString(),
                    message = $"Value {messageCount} : {Time.realtimeSinceStartup}" 
                };
                this.PublishToScene(targetSceneHandle,message);
            }
        }

        private IEnumerable<int> GetHandles() {
            return SceneManagerUtils.GetScenesHandles();
        }

        public IEnumerable<string> GetNames() {
            return SceneManagerUtils.GetScenesNames();
        }

        private void Start() {
            this.ReceiveFromAnyScene<SceneDemoTimeData>().
                RxSubscribe(OnSceneMessageReceived).
                AddTo(this);
        }

        private void OnSceneMessageReceived(SceneDemoTimeData info) {
            GameLog.Log($"SCENE: {info.scene} NAME: {info.name} MESSAGE: {info.message}");
        }
    }
    
}
