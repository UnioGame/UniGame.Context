namespace UniModules.UniGameFlow.GameFlow.Runtime.Systems
{
    using System;
    using global::UniGame.GameFlow.Runtime.Interfaces;
    using global::UniGame.GameFlow.Runtime.Services;
    using UnityEngine.AddressableAssets;

    [Serializable]
    public class AssetReferenceServiceAsset : AssetReferenceT<ServiceDataSourceAsset<IGameService>>
    {
        public AssetReferenceServiceAsset(string guid) : base(guid)
        {
        }
    }
}