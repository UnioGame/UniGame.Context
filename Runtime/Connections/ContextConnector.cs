namespace UniModules.UniGame.Context.Runtime.Connections 
{
    using System;
    using Context;
    using Core.Runtime.Interfaces;
    using global::UniGame.UniNodes.NodeSystem.Runtime.Connections;
    using UniContextData.Runtime.Entities;
    using UniCore.Runtime.Rx.Extensions;
    using UniRx;

    public class ContextConnector : 
        TypeDataConnector<IContext> ,
        IMessageBroker,
        IDisposable
    {
        private EntityContext _cachedContext = new EntityContext();

        public ContextConnector() {
            _registeredItems.
                ObserveRemove().
                Subscribe(x => x.Value?.Disconnect(_cachedContext));
        }
        
        protected sealed override void OnBind(IContext connection) {
            connection.LifeTime.AddCleanUpAction(() => Disconnect(connection));
        }

        public void Publish<T>(T message) 
        {
            foreach (var context in _registeredItems) {
                context.Publish(message);
            }
        }

        public IObservable<T> Receive<T>() {

            //check exists
            if (_cachedContext.Contains<T>()) {
                return _cachedContext.Receive<T>();
            }
            
            //create stream
            foreach (var context in _registeredItems) {
                AddContextReceiver<T>(context).
                    AddTo(_lifeTime);
            }

            _registeredItems.ObserveAdd().
                Subscribe(x => AddContextReceiver<T>(x.Value).AddTo(_lifeTime));
            
            return _cachedContext.Receive<T>();
        }

        private IDisposable AddContextReceiver<T>(IContext context)
        {
            if(context == null || context.LifeTime.IsTerminated)
                return Disposable.Empty;
            
            if (context.Contains<T>()) {
                var value = context.Get<T>();
                _cachedContext.Publish(value);
            }

            _registeredItems.ObserveRemove().
                Where(x => x.Value == context).
                Subscribe(x => UpdateValue<T>()).
                AddTo(_lifeTime);
            
            return context.Bind(_cachedContext);
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

        public void Dispose() => Release();
    }
}