using UniModules.UniGame.AddressableTools.Runtime.AssetReferencies;
using UniModules.UniGame.AddressableTools.Runtime.Extensions;
using UniModules.UniGame.SerializableContext.Runtime.Addressables;

namespace UniModules.UniGame.SerializableContext.Runtime.Sources
{
    using System.Collections.Generic;
    using Addressables;
    using Context.Runtime.Abstract;
    using Core.Runtime.Interfaces;
    using Cysharp.Threading.Tasks;
    using UnityEngine;
    using UnityEngine.U2D;

    [CreateAssetMenu(menuName = "UniGame/GameSystem/Sources/SpriteAtlasesSource", fileName = nameof(SpriteAtlasesSource))]
    public class SpriteAtlasesSource : AsyncContextDataSource
    {
        
        public List<AssetReferenceSpriteAtlas> Atlases = new List<AssetReferenceSpriteAtlas>();
        
        public override async UniTask<IContext> RegisterAsync(IContext context)
        {
            var atlases = await UniTask.WhenAll(Atlases.Select(x => x.LoadAssetTaskAsync(LifeTime)))
                .AttachExternalCancellation(LifeTime.TokenSource);
            context.Publish(atlases);
            return context;
        }
    }
}
