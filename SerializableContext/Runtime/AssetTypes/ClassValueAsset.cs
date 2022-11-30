using Cysharp.Threading.Tasks;
using UniGame.Context.Runtime;
using UniGame.Core.Runtime;
using UniGame.Core.Runtime.Extension;
using UnityEngine;

namespace UniModules.UniGame.SerializableContext.Runtime.Abstract
{
    using System;
    using Context.SerializableContext.Runtime.Abstract;

    /// <summary>
    /// value Source with default constructor
    /// </summary>
    [Serializable]
    public class ClassValueAsset<TValue, TApiValue> 
        : AbstractValueAsset<TValue, TApiValue>,IAsyncDataSource
        where TValue :class, TApiValue, new()
    {
        #region inspector

        [SerializeField]
        public bool publishOriginValue = true;
        
        [SerializeField]
        public bool alwaysCreateNewValue = false;

#if ODIN_INSPECTOR
        [Sirenix.OdinInspector.HideIf(nameof(alwaysCreateNewValue))]
        [Sirenix.OdinInspector.InlineProperty]
        [Sirenix.OdinInspector.HideLabel]
        [Sirenix.OdinInspector.BoxGroup("value")]
#endif
        [SerializeField]
        public TValue defaultValue = default(TValue);
        
        #endregion

        #region public properties
        
        public TValue GetValue() => Value as TValue;
        
        #endregion
        
        public async UniTask<IContext> RegisterAsync(IContext context)
        {
            var sharedAsset = this.ToSharedInstance();

            var api = await OnRegisterAsync(sharedAsset.GetValue(), context);
            
            context.Publish(api);
            
            return context;
        }
        
        protected virtual UniTask<TApiValue> OnRegisterAsync(TValue value, IContext context)
        {
            return UniTask.FromResult<TApiValue>(value);
        }
        
        protected sealed override TValue CreateValue()
        {
            if (alwaysCreateNewValue) return new TValue();
            defaultValue = defaultValue ?? new TValue();
            return defaultValue;
        }

    }

}