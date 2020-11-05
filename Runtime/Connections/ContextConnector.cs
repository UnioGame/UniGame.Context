namespace UniModules.UniGame.Context.Runtime.Connections 
{
    using System;
    using Context;
    using Core.Runtime.Interfaces;
    using UniCore.Runtime.Rx.Extensions;
    using UniRx;

    public class ContextConnector : 
        TypeDataConnector<IContext> , 
        IContextConnector
    {
        private EntityContext _cachedContext = new EntityContext();

        public ContextConnector() {
            _registeredItems.
                ObserveRemove().
                Subscribe(x => x.Value?.Disconnect(_cachedContext));
        }

        public void Dispose() => Release();
        
        public void Publish<T>(T message) 
        {
            foreach (var context in _registeredItems) {
                context.Publish(message);
            }
        }
        
        #region context api

        public IDisposable Bind(IMessagePublisher connection) => _cachedContext.Bind(connection);

        public void  Disconnect(IMessagePublisher connection) => _cachedContext.Disconnect(connection);

        public bool  HasValue     => _cachedContext.HasValue;
        public TData Get<TData>() => _cachedContext.Get<TData>();

        public bool Contains<TData>() => _cachedContext.Contains<TData>();

        public bool Remove<TData>() => _cachedContext.Remove<TData>();
        
        #endregion
        

        public IObservable<T> Receive<T>() {

            //check exists
            if (_cachedContext.Contains<T>()) {
                return _cachedContext.Receive<T>();
            }
            
            //create stream
            foreach (var context in _registeredItems) {
                AddContextReceiver<T>(context);
            }

            _registeredItems.ObserveAdd().
                Subscribe(x => AddContextReceiver<T>(x.Value));
            
            return _cachedContext.Receive<T>();
        }

        private void AddContextReceiver<T>(IContext context)
        {
            if(context == null || context.LifeTime.IsTerminated)
                return;
            
            if (context.Contains<T>()) {
                var value = context.Get<T>();
                _cachedContext.Publish(value);
            }

            _registeredItems.ObserveRemove().
                Where(x => x.Value == context).
                Subscribe(x => UpdateValue<T>()).
                AddTo(_lifeTime);
            
            context.Bind(_cachedContext).AddTo(_lifeTime);
        }

        private void UpdateValue<T>() {
            
            foreach (var context in _registeredItems) {
                if (!context.Contains<T>()) {
                    continue;
                }

                var value = context.Get<T>();
                _cachedContext.Publish(value);
                return;
            }
            
            _cachedContext.Remove<T>();
        }
        
        protected sealed override void OnBind(IContext connection) {
            connection.LifeTime.AddCleanUpAction(() => Disconnect(connection));
        }

    }
}