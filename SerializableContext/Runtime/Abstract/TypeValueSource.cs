using UniModules.UniContextData.Runtime.Interfaces;

namespace UniModules.UniGame.SerializableContext.Runtime.Abstract
{
    using Context.SerializableContext.Runtime.Abstract;
    using Core.Runtime.Interfaces;
    using Cysharp.Threading.Tasks;
    using UniModules.UniCore.Runtime.Rx.Extensions;

    public class TypeValueSource<TValue,TApiValue> : 
        TypeValueAssetSource<TValue, TApiValue>,
        IAsyncContextDataSource,
        IAsyncSourceValue<TApiValue>
        where TValue : class, TApiValue
        where TApiValue : class
    {
        #region inspector
        
        /// <summary>
        /// create instance of SO to prevent original data changes
        /// </summary>
        public bool isProtected = true;

        /// <summary>
        /// if true: any Register call get same ref to value
        /// </summary>
        public bool isShared = true;
        
        #endregion

                
        private AsyncValueGate<TApiValue> assetGate;

        protected IAsyncContextPrototype<TApiValue> Prototype =>
            assetGate = assetGate ?? 
                        new AsyncValueGate<TApiValue>(this, isProtected, isShared).AddTo(LifeTime);

        protected override void OnReset()
        {
            base.OnReset();
            assetGate = null;
        }

        public async UniTask<IContext> RegisterAsync(IContext context)
        {
            var value = await Prototype.Create(context);
            if (value is IAsyncContextDataSource dataSource)
            {
                await dataSource.RegisterAsync(context);
            }
            else
            {
                context.Publish(value);
            }

            await OnRegisterAction(context, value);
            
            return context;
        }

        public virtual async UniTask<IAsyncSourceValue<TApiValue>> Create(IContext context)
        {
            var value = Instantiate(this);
            //bind child lifetime to asset source
            value.AddTo(LifeTime);

            return value;
        }

        public virtual async UniTask<TApiValue> CreateValue(IContext context) => Value;

        protected virtual async UniTask OnRegisterAction(IContext context, TApiValue apiValue) {
            
        }
        
    }

    public class TypeValueSource<TValue> : 
        TypeValueSource<TValue, TValue> 
        where TValue : class, new() { }
}
