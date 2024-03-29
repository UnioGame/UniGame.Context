﻿using UniCore.Runtime.ProfilerTools;
using UniModules.UniGame.Context.Runtime.Connections;

namespace UniModules.UniGame.Context.Runtime.Context
{
    using System;
    using global::UniGame.Core.Runtime;
    using UniRx;
    using UnityEngine;

    public static class ContextExtensions
    {

        public static IContextConnection Merge(this IContext context, IContext targetContext)
        {
            if (context is IContextConnection connector)
                return connector.Merge(targetContext);
                    
            connector = new ContextConnection();
            connector.Broadcast(targetContext);
            connector.Broadcast(context);
            return connector;
        }
        
        public static IContextConnection Merge(this IContextConnection context, IContext targetContext)
        {
            context.Broadcast(targetContext);
            return context;
        }
        
        public static IContextConnection Merge(this IContext source,params IContext[] targetContext)
        {
            var connector = new ContextConnection();
            foreach (var context in targetContext)
            {
                connector.Broadcast(context);
            }
            connector.Broadcast(source);
            return connector;
        }
        
        public static IObservable<Unit> ReceiveFirst<T>(this IContext targetContext, IMessageReceiver sourceContext) where T : class
        {
            return sourceContext.Receive<T>()
                .First()
                .Do(targetContext.Publish)
                .Do(x => GameLog.Log($"{typeof(T).Name} OnServiceLoaded",Color.magenta))
                .AsUnitObservable();
        }

        public static IObservable<T> ReceiveFirst<T>(this IContext sourceContext, Action<T> action) where T : class
        {
            return sourceContext.Receive<T>()
                .First()
                .Do(action);
        }

        public static IDisposable LogValue<T>(this IContext context)
        {
            return context.
                Receive<T>().
                Do(x => GameLog.Log($"{typeof(T).Name} CONTEXT Get {x.GetType().Name}", Color.red)).
                Subscribe();
        }
        
        public static IDisposable LogValue<T>(this IContext context,string id)
        {
            return context.
                Receive<T>().
                Do(x => GameLog.Log($"{id} CONTEXT Get {x.GetType().Name}", Color.red)).
                Subscribe();
        }
        
        public static IContext LogValue<T>(this IContext context,string id, ILifeTime lifeTime)
        {
            context.LogValue<T>(id).
                AddTo(lifeTime);
            return context;
        }
        
        public static IObservable<Unit> ReceiveFirst<T>(this IContext targetContext, IObservable<T> sourceContext) where T : class
        {
            return sourceContext.First()
                .Do(targetContext.Publish)
                .Do(x => GameLog.Log($"{typeof(T).Name} OnServiceLoaded",Color.magenta))
                .AsUnitObservable();
        }
    }
}