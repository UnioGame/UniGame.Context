namespace UniGame.Context.Runtime
{
    using System;
    using UnityEngine.AddressableAssets;

    [Serializable]   
    public class AssetReferenceContextContainer : AssetReferenceT<ContextDataSource>
    {
        public AssetReferenceContextContainer(string guid) : base(guid) {}
    }
}
