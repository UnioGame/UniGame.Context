using Cysharp.Threading.Tasks;
using UniModules.UniGame.Context.SerializableContext.Runtime.Scenarios;
using UniModules.UniGame.Core.Runtime.Interfaces;
using UniRx;

namespace UniModules.UniGame.Context.Scenarios.Scenario1 {
    using System;
    using SerializableContext.Runtime.Abstract;
    using UnityEngine;

    [Serializable]
    public class AsyncStatusCommand : IAsyncContextCommand, IAsyncContextRollback 
    {
        public float       cancelAfter;
        public AsyncStatus result = AsyncStatus.Canceled;
        
        public async UniTask<AsyncStatus> ExecuteAsync(IContext value) {

            if(cancelAfter <=0)
                return result;
            
            await UniTask.Delay(TimeSpan.FromSeconds(cancelAfter));
            
            return result;
        }

        public async UniTask Rollback(IContext source) {
            
            Debug.Log($"{nameof(AsyncStatusCommand)} Command Cancelled");

        }
    }
}
