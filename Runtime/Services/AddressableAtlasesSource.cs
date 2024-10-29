namespace UniGame.GameFlow.Runtime.Services.Common
{
    using global::UniGame.AddressableTools.Runtime;
    using AddressableTools.Runtime.SpriteAtlases;
    using AddressableTools.Runtime.SpriteAtlases.Abstract;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using Cysharp.Threading.Tasks;
    using global::UniGame.Core.Runtime;

    [CreateAssetMenu(menuName = "UniGame/Services/Atlases/Service", fileName = "Addressable Atlases Service")]
    public class AddressableAtlasesSource : ServiceDataSourceAsset<IAddressableAtlasService>
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity]
#endif
        public AssetReferenceT<AddressableSpriteAtlasAsset> configuration;

        protected override async UniTask<IAddressableAtlasService> CreateServiceInternalAsync(IContext context)
        {
            var config = await configuration.LoadAssetTaskAsync(LifeTime);
            config.Initialize();
            config.AddTo(LifeTime);

            var service = AddressableSpriteAtlasAsset.AtlasService;
            
            config.AddTo(LifeTime);

            return service;
        }
    }
}
