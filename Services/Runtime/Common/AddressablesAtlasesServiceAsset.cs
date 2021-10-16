namespace UniModules.UniGameFlow.GameFlow.Runtime.Services.Common
{
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UniModules.UniGame.AddressableTools.Runtime.Extensions;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using Cysharp.Threading.Tasks;
    using UniGame.AddressableTools.Runtime.SpriteAtlases;
    using UniGame.Core.Runtime.Interfaces;

    [CreateAssetMenu(menuName = "UniGame/GameSystem/AddressablesAtlasesService",fileName = "AddressablesAtlasesServiceSource")]
    public class AddressablesAtlasesServiceAsset : ServiceDataSourceAsset<IAddressableAtlasService>
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity]
#endif
        public AssetReferenceT<AddressableSpriteAtlasConfiguration> configuration;

        public bool unloadConfigurationOnReset = true;
        
        protected override async UniTask<IAddressableAtlasService> CreateServiceInternalAsync(IContext context)
        {
            var config = await configuration.LoadAssetTaskAsync(LifeTime);
            config.Initialize();
            config.AddTo(LifeTime);

            var service = config.AtlasService;
            if (!unloadConfigurationOnReset) return service;
            
            config.AddTo(LifeTime);
            service.AddTo(LifeTime);

            return service;
        }
    }
}
