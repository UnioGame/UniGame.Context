namespace UniGame.GameResources.Addressables
{
    using System;
    using UniGame.Context.Runtime;
    using UniGame.GameFlow.Runtime.Interfaces;
    using UniModules.UniGameFlow.GameFlow.Runtime.Services;
    using UnityEngine;

    [Serializable]
    public class AssetReferenceService<TService>: 
        AssetReferenceScriptableObject<ScriptableObject, ServiceDataSourceAsset<TService>>
        where TService : class, IGameService
    {
        public AssetReferenceService(string guid) : base(guid)
        {
        }
    }
}