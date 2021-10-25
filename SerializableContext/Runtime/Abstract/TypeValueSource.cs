using System;
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
        
        private AsyncValueGate<TApiValue> _assetGate;

        protected IAsyncFactory<IContext,TApiValue> Prototype => _assetGate ??= new AsyncValueGate<TApiValue>(this, isProtected, isShared);

        protected override void OnReset()
        {
            base.OnReset();
            _assetGate?.Dispose();
            _assetGate = new AsyncValueGate<TApiValue>(this,isProtected,isShared);
        }

        public async UniTask<TApiValue> CreateAsync(IContext context)
        {
            var value = await Prototype.Create(context);
            return value;
        }

        public async UniTask<IContext> RegisterAsync(IContext context)
        {
            var value = await Prototype.Create(context);

            if (value is IAsyncContextDataSource dataSource)
            {
                await dataSource.RegisterAsync(context)
                    .AttachExternalCancellation(LifeTime.TokenSource);
            }
            else
            {
                context.Publish(value);
            }

            await OnRegisterAction(context, value);
            
            return context;
        }

        public virtual UniTask<IAsyncSourceValue<TApiValue>> Create(IContext context)
        {
            var value = Instantiate(this);
            //bind child lifetime to asset source
            value.AddTo(LifeTime);

            return UniTask.FromResult<IAsyncSourceValue<TApiValue>>(value);
        }

        public virtual UniTask<TApiValue> CreateValue(IContext context) => UniTask.FromResult<TApiValue>(Value);

        protected virtual UniTask OnRegisterAction(IContext context, TApiValue apiValue) { return UniTask.CompletedTask; }
        
    }

    public class TypeValueSource<TValue> : 
        TypeValueSource<TValue, TValue> 
        where TValue : class, new() { }
}
