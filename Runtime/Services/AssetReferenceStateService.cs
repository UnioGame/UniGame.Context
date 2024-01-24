namespace UniModules.UniGameFlow.GameFlow.Runtime.Systems
{
    using System;
    using global::UniGame.GameFlow.Runtime.Services;
    using global::UniGame.Context.Runtime;

    [Serializable]
    public class AssetReferenceStateService : AssetReferenceScriptableObject<ServiceDataSourceAsset>
    {
        public AssetReferenceStateService(string guid) : base(guid)
        {
        }
    }
}