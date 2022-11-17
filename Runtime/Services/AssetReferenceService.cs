﻿using UniModules.UniGame.AddressableTools.Runtime.AssetReferencies;

namespace Taktika.GameResources.Addressables
{
    using System;
    using UniModules.UniGame.SerializableContext.Runtime.Addressables;
    using UniGame.GameFlow.Runtime.Interfaces;
    using UniModules.UniGameFlow.GameFlow.Runtime.Services;
    using UniModules.UniGameFlow.GameFlow.Runtime.Systems;
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