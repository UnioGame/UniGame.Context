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
        
        #region public properties

        public IContext Context => _cachedContext;
        
        #endregion

        public void Dispose() => Release();

        public void Publish<T>(T message) => _cachedContext.Publish(message);

        public IObservable<T> Receive<T>() {

            //check exists
            if (_cachedContext.Contains<T>()) {
                return _cachedContext.Receive<T>();
            }
            
            //create stream
            foreach (var context in _registeredItems) {
                AddContextReceiver<T>(context);
            }

            _registeredItems.
                ObserveAdd().
                Subscribe(x => AddContextReceiver<T>(x.Value)).
                AddTo(_lifeTime);
            
            return _cachedContext.Receive<T>();
        }

        private void AddContextReceiver<T>(IContext context)
        {
            if(context == null || context.LifeTime.IsTerminated)
                return;
            
            if (context.Contains<T>()) {
                var value = context.Get<T>();
                Publish(value);
            }

            _registeredItems.ObserveRemove().
                Where(x => x.Value == context).
                Subscribe(x => UpdateValue<T>()).
                AddTo(_lifeTime);
            
            context.Bind(_cachedContext).
                AddTo(_lifeTime);
        }

        private void UpdateValue<T>()
        {
            for (var i = _registeredItems.Count - 1; i >= 0 ; i--)
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

            _cachedContext.Remove<T>();
        }
        
        protected sealed override void OnBind(IContext connection) {
            connection.LifeTime.AddCleanUpAction(() => Disconnect(connection));
        }

    }
}