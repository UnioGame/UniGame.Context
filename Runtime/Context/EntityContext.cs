﻿namespace UniModules.UniGame.Context.Runtime.Context
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.CompilerServices;
    using System.Threading;
    using Core.Runtime.DataFlow;
    using Cysharp.Threading.Tasks;
    using global::UniGame.Core.Runtime;
    using global::UniModules.GameFlow.Runtime.Connections;
    using UniCore.Runtime.Common;
    using UniCore.Runtime.DataFlow;
    using UniRx;
    using UnityEngine;

    [Serializable]
    public class EntityContext :
        IDisposableContext
    {
        private TypeData _data;
        private LifeTimeDefinition _lifeTime;
        private TypeDataBrodcaster _broadcaster;
        private int _id;

        public EntityContext()
        {
            //context data container
            _data = new TypeData();
            //context lifetime
            _lifeTime = new LifeTimeDefinition();
            _broadcaster = new TypeDataBrodcaster();
            _id = Unique.GetId();

            Release();
        }

        #region connection api

        public int BindingsCount => _broadcaster.Count;

        public void Break(IMessagePublisher connection)
        {
            _broadcaster.Remove(connection);
        }

        public IDisposable Broadcast(IMessagePublisher connection)
        {
            if (ReferenceEquals(connection, this))
                return Disposable.Empty;

            var disposable = _broadcaster.Broadcast(connection);
            return disposable;
        }

        #endregion

        #region public properties

        public int Id => _id;

        public ILifeTime LifeTime => _lifeTime.LifeTime;

        public bool HasValue => _data.HasValue;

        #endregion

        #region public methods

        public bool Contains<TData>() => _data.Contains<TData>();

        public virtual TData Get<TData>() => _data.Get<TData>();

        public virtual object Get(Type type) => _data.Get(type);
        
        public bool Remove<TData>() => _data.Remove<TData>();

        public void RemoveSilent<TData>() => _data.RemoveSilent<TData>();

        public void Release()
        {
            _lifeTime.Release();
            _lifeTime.AddCleanUpAction(_data.Release);
            _lifeTime.AddCleanUpAction(_broadcaster.Release);
        }

        public virtual void Dispose() => Release();

        #region rx

        public void Publish<T>(T message)
        {
            CheckLifeTimeValue(message);
            _data.Publish(message);
            _broadcaster.Publish(message);
        }
        
        public void PublishForce<T>(T message)
        {
            CheckLifeTimeValue(message);
            _data.PublishForce(message);
            _broadcaster.Publish(message);
        }

        public IObservable<T> Receive<T>()
        {
            return _data.Receive<T>();
        }

        #endregion

        #endregion

        #region private methods

        private void CheckLifeTimeValue<T>(T value)
        {
            var lifeTime = value.GetLifeTime();

            if (lifeTime.IsTerminatedLifeTime())
                return;

            lifeTime.AddCleanUpAction(() =>
            {
                var valueData = Get<T>();
                if (ReferenceEquals(valueData, value))
                    Remove<T>();
            });
        }

        #endregion
        
        #region Unity Editor Api

#if UNITY_EDITOR

        public IReadOnlyDictionary<Type, IValueContainerStatus> EditorValues => _data.EditorValues;

#endif

        #endregion
    }

    public static class GameContext
    {
        public static IContext Context;
        //public static CancellationTokenSource CancellationTokenSource = new CancellationTokenSource();
        
        public static async UniTask<IContext> GetContextAsync(CancellationToken token = default)
        {
            if (Context != null)
                return Context;

            await UniTask.WaitWhile(() => Context == null,cancellationToken:token);
            return Context;
        }
    }
}