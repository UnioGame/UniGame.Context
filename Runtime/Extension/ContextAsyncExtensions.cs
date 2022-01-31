using System;
using UniModules.UniCore.Runtime.Extension;
using UniGame.Addressables.Reactive;

namespace UniModules.UniGame.CoreModules.UniGame.Context.Runtime.Extension
{
    using Cysharp.Threading.Tasks;
    using UniModules.UniGame.Core.Runtime.DataFlow.Interfaces;
    using UniModules.UniGame.Core.Runtime.Interfaces;
    using UnityEngine;
    using Component = System.ComponentModel.Component;

    public static class ContextAsyncExtensions
    {
        public static async UniTask<TValue> ReceiveFirstAsync<TValue>(this IReadOnlyContext context)
        {
            return await context.ReceiveFirstAsync<TValue>(context.LifeTime);
        }

        public static async UniTask<TValue> ReceiveFirstAsync<TValue>(this IReadOnlyContext context, ILifeTime lifeTime)
        {
            if (context == null) return default;
            
            if (context.Contains<TValue>())
                return context.Get<TValue>();
            
            return await context.Receive<TValue>().AwaitFirstAsync(lifeTime);
        }

        public static async UniTask<TValue> ReceiveFirstAsync<TValue>(this IReadOnlyContext context, IObservable<TValue> observable)
        {
            if (context == null) return default;
            return await observable.AwaitFirstAsync(context.LifeTime);
        }

        public static async UniTask<TValue> ReceiveFirstAsync<TValue>(this ILifeTime lifeTime, IObservable<TValue> observable)
        {
            if (lifeTime == null) return default;
            return await observable.AwaitFirstAsync(lifeTime);
        }

        public static async UniTask<TComponent> ReceiveFirstFromSceneAsync<TComponent>(this IContext context, bool register = true)
            where TComponent : Object
        {
            TComponent result   = null;
            var        lifeTime = context.LifeTime;

            if (context.Contains<TComponent>())
                return context.Get<TComponent>();
            
            while (lifeTime.IsTerminated == false)
            {
                result = Object.FindObjectOfType<TComponent>();
                if (result != null)
                    break;

                await UniTask.Yield();
            }

            if(register)
                context.Publish(result);
    
            return result;
        }
    }
}