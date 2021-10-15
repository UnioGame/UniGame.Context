namespace UniModules.UniGameFlow.GameFlow.Runtime.Services.Common
{
    using Cysharp.Threading.Tasks;
    using global::UniGame.UniNodes.GameFlow.Runtime;
    using UniGame.AddressableTools.Runtime.SpriteAtlases;
    using UniGame.AddressableTools.Runtime.SpriteAtlases.Abstract;
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniRx;
    using UnityEngine.SceneManagement;

    public class AddressablesAtlasesService : GameService, IAddressablesAtlasesService
    {
        private IAddressableSpriteAtlasHandler _addressableSpriteAtlasHandler;
        
        public AddressablesAtlasesService(IAddressableSpriteAtlasHandler atlasHandler)
        {
            _addressableSpriteAtlasHandler = atlasHandler;
            Observable.FromEvent(
                x => SceneManager.activeSceneChanged += OnSceneChanged,
                x => SceneManager.activeSceneChanged -= OnSceneChanged).
                Subscribe().
                AddTo(LifeTime);
            
            Complete();
        }

        private void OnSceneChanged(Scene fromScene, Scene toScene)
        {
            if (fromScene.path == toScene.path)
                return;
        }
        
    }
}
