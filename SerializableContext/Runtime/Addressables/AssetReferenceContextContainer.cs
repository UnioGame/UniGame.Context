namespace UniModules.UniGame.SerializableContext.Runtime.Addressables
{
    using System;
    using AssetTypes;
    using UnityEngine.AddressableAssets;

    [Serializable]   
    public class AssetReferenceContextContainer : AssetReferenceT<ContextContainerAsset>
    {
        public AssetReferenceContextContainer(string guid) : base(guid) {}
    }
}
