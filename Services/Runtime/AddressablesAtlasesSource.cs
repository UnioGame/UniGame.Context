using UniModules.UniGame.CoreModules.UniGame.AddressableTools.Runtime.Extensions;
using UniModules.UniGame.CoreModules.UniGame.AddressableTools.Runtime.SpriteAtlases;
using UniModules.UniGame.CoreModules.UniGame.AddressableTools.Runtime.SpriteAtlases.Abstract;

namespace UniModules.UniGameFlow.GameFlow.Runtime.Services.Common
{
    using UniModules.UniCore.Runtime.Rx.Extensions;
    using UnityEngine;
    using UnityEngine.AddressableAssets;
    using Cysharp.Threading.Tasks;
    using UniGame.Core.Runtime.Interfaces;

    [CreateAssetMenu(menuName = "UniGame/GameSystem/" + nameof(AddressablesAtlasesSource),fileName = nameof(AddressablesAtlasesSource))]
    public class AddressablesAtlasesSource : ServiceDataSourceAsset<IAddressableAtlasService>
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
