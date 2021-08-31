namespace UniModules.UniGameFlow.GameFlow.Runtime.Services.Common
{
    using UniModules.UniGame.AddressableTools.Runtime.Extensions;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using Cysharp.Threading.Tasks;
    using UniGame.AddressableTools.Runtime.SpriteAtlases;
    using UniGame.AddressableTools.Runtime.SpriteAtlases.Abstract;
    using UniGame.Core.Runtime.Interfaces;

    [CreateAssetMenu(menuName = "UniGame/GameSystem/AddressablesAtlasesService",fileName = nameof(AddressablesAtlasesService))]
    public class AddressablesAtlasesServiceAsset : ServiceDataSourceAsset<IAddressablesAtlasesService>
    {
#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.DrawWithUnity]
#endif
        public AssetReferenceT<AddressableSpriteAtlasConfiguration> configuration;
        
        protected override async UniTask<IAddressablesAtlasesService> CreateServiceInternalAsync(IContext context)
        {
            var config = await configuration.LoadAssetTaskAsync(LifeTime);
            await config.Execute();
            
            var service = new AddressablesAtlasesService(config);
            
            context.Publish<IAddressablesAtlasesLoader>(service);
            context.Publish<IAddressableSpriteAtlasHandler>(config);
            
            return service;
        }
    }
}
