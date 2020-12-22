namespace UniModules.UniGame.Context.Runtime.Connections
{
    using System;
    using Context;
    using Core.Runtime.Interfaces;
    using Core.Runtime.Rx;
    using UniCore.Runtime.Rx.Extensions;
    using UniRx;

    public class ContextConnection :
        TypeDataCollection<IContext>,
        IContextConnection,
        IResetable
    {
        private readonly EntityContext                 _cachedContext = new EntityContext();
        private readonly RecycleReactiveProperty<bool> _isEmpty       = new RecycleReactiveProperty<bool>(true);

        public ContextConnection() => Reset();

        #region properties

        public int BindingsCount => Count;

        public IReadOnlyReactiveProperty<bool> IsEmpty => _isEmpty;

        public bool HasValue => _cachedContext.HasValue;

        #endregion

        public IDisposable Broadcast(IMessagePublisher connection) => _cachedContext.Broadcast(connection);

        public void Break(IMessagePublisher connection) => _cachedContext.Break(connection);

        public void Disconnect(IContext connection) => base.Remove(connection);

        public IDisposable Connect(IContext source) => Add(source);

        public bool Remove<TData>() => _cachedContext.Remove<TData>();

        public TData Get<TData>() => _cachedContext.Get<TData>();

        public bool Contains<TData>() => _cachedContext.Contains<TData>();

        public void Reset()
        {
            Release();

            _registeredItems
                .ObserveRemove()
                .Subscribe(x => x.Value?.Break(_cachedContext))
                .AddTo(LifeTime);

            _registeredItems
                .ObserveCountChanged()
                .Select(x => x == 0)
                .Subscribe(x => _isEmpty.Value = x)
                .AddTo(LifeTime);

            LifeTime.AddCleanUpAction(Reset);
        }

        public void Dispose() => Release();

        public void Publish<T>(T message) => _cachedContext.Publish(message);

        public IObservable<T> Receive<T>()
        {
            //check exists
            if (_cachedContext.Contains<T>())
            {
                return _cachedContext.Receive<T>();
            }

            //create stream
            foreach (var context in _registeredItems)
            {
                AddContextReceiver<T>(context);
            }

            _registeredItems
                .ObserveAdd()
                .Subscribe(x => AddContextReceiver<T>(x.Value))
                .AddTo(_lifeTime);

            return _cachedContext.Receive<T>();
        }

        private void AddContextReceiver<T>(IContext context)
        {
            if (context == null || context.LifeTime.IsTerminated)
                return;

            if (context.Contains<T>())
            {
                var value = context.Get<T>();
                Publish(value);
            }

            _registeredItems
                .ObserveRemove()
                .Where(x => Equals(x.Value, context))
                .Subscribe(x => UpdateValue<T>())
                .AddTo(_lifeTime);

            context.Broadcast(_cachedContext)
                .AddTo(_lifeTime);
        }

        private void UpdateValue<T>()
        {
            for (var i = _registeredItems.Count - 1; i >= 0; i--)
            {
                var context = _registeredItems[i];
                if (!context.Contains<T>())
                {
                    continue;
                }

                var value = context.Get<T>();
                Publish(value);
                return;
            }

            _cachedContext.RemoveSilent<T>();
        }

        protected override void OnBind(IContext connection)
        {
            connection.LifeTime.AddCleanUpAction(() => Remove(connection));
        }

        protected override void OnRelease()
        {
            _cachedContext.Release();
        }
    }
}