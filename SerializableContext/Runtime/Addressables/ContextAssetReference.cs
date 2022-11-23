namespace UniGame.Context.Runtime
{
    using System;

#if ODIN_INSPECTOR
    [Sirenix.OdinInspector.DontApplyToListElements]
    #endif
    [Serializable]   
    public class ContextAssetReference : AssetReferenceDataSource<ContextAsset>
    {
        public ContextAssetReference(string guid) : base(guid) {}
    }
}
