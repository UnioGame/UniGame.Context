namespace UniGame.Context.Runtime
{
    using System;
    using Core.Runtime.ScriptableObjects;

    [Serializable]
    public class AssetReferenceDataSource<TAsset> : AssetReferenceScriptableObject<TAsset,IAsyncDataSource> 
        where TAsset : LifetimeScriptableObject
    {
        public AssetReferenceDataSource(string guid) : base(guid) {}
        
    }
    
    [Serializable]
    public class AssetReferenceDataSource : AssetReferenceDataSource<LifetimeScriptableObject> 
    {
        public AssetReferenceDataSource(string guid) : base(guid) {}
        
    }
}
